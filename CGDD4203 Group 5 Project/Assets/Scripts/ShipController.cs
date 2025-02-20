using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class ShipController : MonoBehaviour
{
    public static ShipController current;
    [SerializeField] public ShipStatistics stats;

    //**PROPERTIES**
    [Header("Movement Settings")]
    [SerializeField] float rotationSpeed;
    [SerializeField] int speedLimit;
    [SerializeField] float autoBrakingStrength = 0.5f; // TODO: move to ship stats
    //
    [Header("Events")]
    public UnityEvent<float> onThrust;
    public UnityEvent onGunFired;
    [Header("Laser Settings")]
    [SerializeField] float laserRechargeTime;
    //
    [Header("References")]
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] Transform projectileSpawn2;
    [SerializeField] Transform projectileParent;
    [SerializeField] HUDController hudController;
    [SerializeField] Button btnLTurn;
    [SerializeField] Button btnRTurn;
    [SerializeField] Button btnThrust;
    [SerializeField] Button btnFire;
    //
    PlayerInput playerInput;
    CharacterController characterController;
    //
    InputAction thrustAction;
    InputAction turnAction;
    InputAction fireAction;
    InputAction accelAction;
    //DEV CODE - DELETE BEFORE FINAL BUILD
    InputAction breakAction;
    InputAction spawnAction;
    bool spawnedThisPress;
    //
    Vector3 currentMovement = Vector3.zero;
    float desiredYRotation = 0f;
    int score = 0;
    bool inTrigger = false;
    bool laserCharged = true;
    bool isInvulnerable = false;

    //**PROPERTIES**
    public bool IsInvulnerable { get => isInvulnerable; }

    public Vector3 Velocity { get => characterController.velocity; }


    //**UNITY METHODS**
    void OnEnable()
    {
        current = this;
    }
    void Awake()
    {
        // Cache references
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        //Initialize Statistics (tf-3, lrt-1s, hp = 100)
        // stats = new ShipStatistics();

        // Initialize the input actions
        thrustAction = playerInput.actions["Thrust"];
        turnAction = playerInput.actions["Turn"];
        fireAction = playerInput.actions["Fire"];
        //DEV CODE - DELETE BEFORE FINAL BUILD
        breakAction = playerInput.actions["devBreak"];
        spawnAction = playerInput.actions["devSpawnEnemy"];
        accelAction = playerInput.actions["Accelerometer"];


        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
        }
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
        }

    }
    //

    private (float min, float max) runningAvgThrustTiltBounds = (0.4f, .6f);
    private float lastThrustAttitudeInput = 0.5f;
    private bool lastThrustAttitudeInputResult = false;

    void FixedUpdate()
    {
        //DEV CODE - DELETE BEFORE FINAL BUILD
        string devOutput = "\tDEBUG - Update\n====================\n";
        devOutput += $"Inside trigger? {inTrigger}\n";
        // print(UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue());
        // if (AttitudeSensor.current != null)
        //     print(AttitudeSensor.current.attitude.ReadValue());
        // print(accelAction.ReadValue<Vector3>());

        devOutput += $"Health: {stats.ShieldPower / (float)stats.ShieldPowerMax}\n";

        //Update HUD
        hudController.SetScoreValue(score);
        hudController.SetHealthBar(stats.ShieldPower / (float)stats.ShieldPowerMax);

        //*Input Handling*
        //Thrust
        float thrustInput = thrustAction.ReadValue<float>();
        if (AttitudeSensor.current != null)
        {
            float thrustAttitudeInput = Mathf.Clamp01(Vector3.Dot(AttitudeSensor.current.attitude.ReadValue() * Vector3.forward, Vector3.forward));

            // TODO: Remind me to clean this up this later...
            // if (thrustAttitudeInput > runningAvgThrustTiltBounds.max + 0.3f)
            // {
            //     runningAvgThrustTiltBounds.max = Mathf.Lerp(thrustAttitudeInput, runningAvgThrustTiltBounds.max, 1 - Mathf.Exp(-10f * Time.fixedDeltaTime));
            //     runningAvgThrustTiltBounds.max = Mathf.Min(runningAvgThrustTiltBounds.min + 0.3f, runningAvgThrustTiltBounds.max);
            // }
            // if (thrustAttitudeInput < runningAvgThrustTiltBounds.min - 0.3f)
            // {
            //     runningAvgThrustTiltBounds.min = Mathf.Lerp(thrustAttitudeInput, runningAvgThrustTiltBounds.min, 1 - Mathf.Exp(-10f * Time.fixedDeltaTime));
            //     runningAvgThrustTiltBounds.min = Mathf.Max(runningAvgThrustTiltBounds.max - 0.3f, runningAvgThrustTiltBounds.min);
            // }
            // float center = (runningAvgThrustTiltBounds.min + runningAvgThrustTiltBounds.max) / 2;
            float center = 0.75f;
            bool debounce = Mathf.Abs(thrustAttitudeInput - lastThrustAttitudeInput) < 0.1f;
            bool attInputResult = debounce ? (thrustAttitudeInput > center) : lastThrustAttitudeInputResult;
            thrustInput += attInputResult ? 1f : 0f;
            // print($"thrustAttitudeInput: {thrustAttitudeInput}, runningAvgThrustTiltBounds.min: {runningAvgThrustTiltBounds.min} max {runningAvgThrustTiltBounds.max} center {center}");
            lastThrustAttitudeInput = thrustAttitudeInput;
            lastThrustAttitudeInputResult = attInputResult;
        }

        if (thrustInput != 0)
        {

            ChangeButtonColor(btnThrust, new Color(245 / 255f, 245 / 255f, 245 / 255f));

            Vector3 thrust = transform.rotation * Vector3.forward * stats.ThrustForce * .01f;
            currentMovement += thrust;

            // Particles & Other FX
            onThrust.Invoke(1.0f);
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += $"Thrust applied in direction: {Mathf.Round(thrust.x * 1000) / 1000},{Mathf.Round(thrust.z * 1000) / 1000}\n";
        }
        else
        {
            ChangeButtonColor(btnThrust, new Color(200 / 255f, 200 / 255f, 200 / 255f));
            currentMovement -= autoBrakingStrength * characterController.velocity * Time.fixedDeltaTime;
            // Particles & Other FX
            onThrust.Invoke(0f);
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += "No thrust\n";
        }
        //
        //Fire
        if (fireAction.ReadValue<float>() != 0 && laserCharged)
        {
            //Change button colors
            ChangeButtonColor(btnFire, new Color(245 / 255f, 245 / 255f, 245 / 255f));

            laserCharged = false;

            Vector3 spawnPosition = projectileSpawn.position;
            Vector3 spawnPosition2 = projectileSpawn2.position;
            Quaternion spawnRotation = transform.rotation;

            devOutput += $"Spawning projectile at position: {spawnPosition} with rotation: {spawnRotation.eulerAngles}\n";
            devOutput += $"Velocity Mag of ship: {characterController.velocity.magnitude}\n";

            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation, projectileParent);
            GameObject projectile2 = Instantiate(projectilePrefab, spawnPosition2, spawnRotation, projectileParent);

            PlayerProjectileController playerProjectileController = projectile.GetComponent<PlayerProjectileController>();
            PlayerProjectileController playerProjectileController1 = projectile2.GetComponent<PlayerProjectileController>();
            playerProjectileController.Speed = speedLimit * 6f;
            playerProjectileController1.Speed = speedLimit * 6f;
            playerProjectileController.velocity += characterController.velocity;
            playerProjectileController1.velocity += characterController.velocity;
            onGunFired.Invoke();
            
            StartCoroutine(laserRecharge());
            Destroy(projectile2, 5f);
            Destroy(projectile, 5f);
        }
        //Test if fire button not pressed
        else if (fireAction.ReadValue<float>() == 0)
        {
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += "Fire button not pressed\n";

            //Turn on button
            ChangeButtonColor(btnFire, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }
        //
        //Turn      

        float turnInput = turnAction.ReadValue<float>();

        if (AttitudeSensor.current != null)
        {
            float turnAttitudeInput = Vector3.Dot(AttitudeSensor.current.attitude.ReadValue() * Vector3.up, Vector3.up);
            if (Mathf.Abs(turnAttitudeInput) < 0.2f) { turnAttitudeInput = 0; } // Deadzone
            // print($"turnAttitudeInput: {turnAttitudeInput}");
            turnInput += turnAttitudeInput * turnAttitudeInput * Mathf.Sign(turnAttitudeInput); // Curve input
        }
        if (turnInput < 0)
        {
            //Turn on button
            ChangeButtonColor(btnLTurn, new Color(245 / 255f, 245 / 255f, 245 / 255f));
            //Turn off button
            ChangeButtonColor(btnRTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }
        else if (turnInput > 0)
        {
            //Turn on button
            ChangeButtonColor(btnRTurn, new Color(245 / 255f, 245 / 255f, 245 / 255f));
            //Turn off button
            ChangeButtonColor(btnLTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }
        else
        {
            //Turn off buttons
            ChangeButtonColor(btnLTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
            ChangeButtonColor(btnRTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }

        desiredYRotation += turnInput * rotationSpeed * 10 * Time.deltaTime;
        //DEV CODE - DELETE BEFORE FINAL BUILD
        devOutput += $"Turn Input: {turnAction.ReadValue<float>()}\n";

        //Lerp ship rotation
        float currentRotationY = transform.rotation.eulerAngles.y;
        float smoothedRotationY = Mathf.LerpAngle(currentRotationY, desiredYRotation, Time.fixedDeltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, smoothedRotationY, 0);

        //Limit speed
        currentMovement.x = Mathf.Clamp(currentMovement.x, -speedLimit, speedLimit);
        currentMovement.z = Mathf.Clamp(currentMovement.z, -speedLimit, speedLimit);

        //Snap ship to 0 Y coord
        currentMovement.y += -currentMovement.y;

        //Move ship controller
        characterController.Move(currentMovement * Time.deltaTime);

        //DEV CODE - DELETE BEFORE FINAL BUILD
        if (breakAction.ReadValue<float>() != 0)
        {
            Debug.Break();
        }

        if (spawnAction.ReadValue<float>() != 0 && !spawnedThisPress)
        {
            gameManager.SpawnEnemy();
            spawnedThisPress = true;
        }
        else if (spawnAction.ReadValue<float>() == 0)
        {
            spawnedThisPress = false;
        }

        devOutput += $"Ship Veloctiy: {currentMovement}\n";
        devOutput += $"Ship Rot: {transform.rotation.eulerAngles}\n";
        devOutput += "====================\n";
        if (gameManager.ShipDebug)
        {
            Debug.Log(devOutput);
        }
    }
    //
    private void OnTriggerEnter(Collider other)
    {
        //DEV CODE - DELETE BEFORE FINAL BUILD
        string devOutput = "\tDEBUG - Trigger Enter\n====================\n";

        if (other.gameObject.tag == "Wall" && !inTrigger)
        {
            inTrigger = true;
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += $"Wall Collision\n";
            devOutput += $"Wall dimensions: ({other.transform.localScale.x},{other.transform.localScale.z})\n";

            Vector3 newPosition = Vector3.zero;
            if (other.transform.localScale.x > other.transform.localScale.z)
            {
                if (other.transform.position.z > 0)
                {
                    //DEV CODE - DELETE BEFORE FINAL BUILD
                    devOutput += "Hit the North wall\n";
                }
                else
                {
                    //DEV CODE - DELETE BEFORE FINAL BUILD
                    devOutput += "Hit the South wall\n";
                }


                newPosition = new Vector3(transform.position.x, 0, -transform.position.z < 0 ? -(gameManager.LevelSize.y / 2 - (transform.localScale.z * 1.7f)) : (gameManager.LevelSize.y / 2 - (transform.localScale.z * 1.7f)));
            }
            else
            {

                if (other.transform.position.x > 0)
                {
                    //DEV CODE - DELETE BEFORE FINAL BUILD
                    devOutput += "Hit the East wall\n";
                }
                else
                {
                    //DEV CODE - DELETE BEFORE FINAL BUILD
                    devOutput += "Hit the West wall\n";
                }

                newPosition = new Vector3(transform.position.x < 0 ? (gameManager.LevelSize.x / 2 - (transform.localScale.x * 1.7f)) : -(gameManager.LevelSize.x / 2 - (transform.localScale.x * 1.7f)), 0, transform.position.z);
            }
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += $"Flipped location: {newPosition}\n";

            Vector3 displacement = newPosition - transform.position;
            characterController.Move(displacement);
        }
        else if (other.gameObject.tag == "Asteroid" && !isInvulnerable)
        {

            //Split asteroid
            other.GetComponent<AstroidController>().Split(2);

            //adjust health (DEV: lvl1 = 5%, lvl2 = 10%, lvl3 = 20%)
            int asteroidSize = other.GetComponent<AstroidController>().Size;
            int damage;

            if (asteroidSize == 1)
            {
                damage = (int)(stats.ShieldPowerMax * .05f);
            }
            else if (asteroidSize == 2)
            {
                damage = (int)(stats.ShieldPowerMax * .1f);
            }
            else
            {
                damage = (int)(stats.ShieldPowerMax * .2f);
            }

            //Debug.Log($"Damage dealt: {damage}");

            //Apply stat data
            stats.ApplyStatisticsMod(new ShipStatisticModifierData(-damage, 0, 0));
        }


        //DEV CODE - DELETE BEFORE FINAL BUILD
        devOutput += "====================\n";
        if (gameManager.WallDebug)
        {
            Debug.Log(devOutput);
        }
    }
    //
    private void OnTriggerExit(Collider other)
    {
        //DEV CODE - DELETE BEFORE FINAL BUILD
        string devOutput = "\tDEBUG - Trigger Exit\n====================\n";
        inTrigger = false;
        //DEV CODE - DELETE BEFORE FINAL BUILD
        devOutput += "====================\n";
        if (gameManager.WallDebug)
        {
            Debug.Log(devOutput);
        }
    }

    //**UTILITY METHODS**
    private void ChangeButtonColor(Button btn, Color newColor)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = newColor;
        btn.colors = cb;
    }

    public UnityEvent onHealthReduced;
    public void UpdateHealth(int amount)
    {
        if (amount < 0)
        {
            //Go invulnerable
            StartCoroutine(Invulnerability(3, 1));
        }

        onHealthReduced.Invoke();
        stats.ApplyStatisticsMod(new ShipStatisticModifierData(-amount, 0, 0));
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        score = Mathf.Max(score, 0);
    }
    //**COROUTINES**
    IEnumerator laserRecharge()
    {
        yield return new WaitForSeconds(stats.FireRate);
        laserCharged = true;
    }

    IEnumerator Invulnerability(float duration, float pulseFrequency)
    {
        //Set flag
        isInvulnerable = true;
        if (gameManager.ShipDebug)
        {
            Debug.Log("Player ship is now invulnerable");
        }

        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();

        // Store the initial colors of the materials
        Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
        foreach (var renderer in meshes)
        {
            foreach (var material in renderer.materials)
            {
                if (!originalColors.ContainsKey(material))
                {
                    originalColors[material] = material.color;
                }
            }
        }

        float elapsedTime = 0f;
        float halfCycle = pulseFrequency / 2f;


        while (elapsedTime < duration)
        {
            // Calculate the interpolation factor based on ping-pong effect
            float t = Mathf.PingPong(elapsedTime, halfCycle) / halfCycle;

            // Apply the color interpolation to all materials
            foreach (var renderer in meshes)
            {
                foreach (var material in renderer.materials)
                {
                    Color originalColor = originalColors[material];
                    Color targetColor = Color.red;
                    material.color = Color.Lerp(originalColor, targetColor, t);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore original colors
        foreach (MeshRenderer renderer in meshes)
        {
            foreach (Material material in renderer.materials)
            {
                if (originalColors.ContainsKey(material))
                {
                    material.color = originalColors[material];
                }
            }
        }

        isInvulnerable = false;
        if (gameManager.ShipDebug)
        {
            Debug.Log("Player ship is now vulnerable");
        }
    }
}

