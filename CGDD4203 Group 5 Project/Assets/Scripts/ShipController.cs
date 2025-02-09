using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class ShipController : MonoBehaviour {
    //**PROPERTIES**
    [Header("Movement Settings")]
    [SerializeField] float thrustForce;
    [SerializeField] float rotationSpeed;
    [SerializeField] int speedLimit;
    //
    [Header("Laser Settings")]
    [SerializeField] float laserRechargeTime;
    //
    [Header("References")]
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] Transform projectileParent;
    [SerializeField] HUDController hudController;
    [SerializeField] Button btnLTurn;
    [SerializeField] Button btnRTurn;
    [SerializeField] Button btnThrust;
    [SerializeField] Button btnFire;
    //
    PlayerInput playerInput;
    CharacterController characterController;
    ParticleSystem flameParticles;
    //
    InputAction thrustAction;
    InputAction turnAction;
    InputAction fireAction;
    //DEV CODE - DELETE BEFORE FINAL BUILD
    InputAction breakAction;
    InputAction spawnAction;
    bool spawnedThisPress;
    //
    Vector3 currentMovement = Vector3.zero;
    float desiredYRotation = 0f;
    int score = 0;
    int maxHealth = 100;
    int health = 100;
    bool inTrigger = false;
    bool laserCharged = true;
    bool isInvulnerable = false;

    //**FIELDS**
    public bool IsInvulnerable { get => isInvulnerable; }


    //**UNITY METHODS**
    void Awake() {
        // Cache references
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        flameParticles = GetComponentInChildren<ParticleSystem>();

        // Initialize the input actions
        thrustAction = playerInput.actions["Thrust"];
        turnAction = playerInput.actions["Turn"];
        fireAction = playerInput.actions["Fire"];
        //DEV CODE - DELETE BEFORE FINAL BUILD
        breakAction = playerInput.actions["devBreak"];
        spawnAction = playerInput.actions["devSpawnEnemy"];
    }
    //
    void Update() {
        //DEV CODE - DELETE BEFORE FINAL BUILD
        string devOutput = "\tDEBUG - Update\n====================\n";
        devOutput += $"Inside trigger? {inTrigger}\n";


        devOutput += $"Health: {health / (float) maxHealth}\n";

        //Update HUD
        hudController.SetScoreValue(score);
        hudController.SetHealthBar(health / (float) maxHealth);

        //*Input Handling*
        //Thrust
        if (thrustAction.ReadValue<float>() != 0) {

            ChangeButtonColor(btnThrust, new Color(245 / 255f, 245 / 255f, 245 / 255f));

            Vector3 thrust = (transform.rotation * Vector3.forward).normalized * thrustForce * .01f;
            currentMovement += thrust;

            //Particles
            flameParticles.Play();
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += $"Thrust applied in direction: {Mathf.Round(thrust.x * 1000) / 1000},{Mathf.Round(thrust.z * 1000) / 1000}\n";
        }
        else {
            //Particles
            flameParticles.Stop();

            ChangeButtonColor(btnThrust, new Color(200 / 255f, 200 / 255f, 200 / 255f));

            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += "No thrust\n";
        }
        //
        //Fire
        if (fireAction.ReadValue<float>() != 0 && laserCharged) {
            //Change button colors
            ChangeButtonColor(btnFire, new Color(245 / 255f, 245 / 255f, 245 / 255f));

            laserCharged = false;

            Vector3 spawnPosition = projectileSpawn.position;
            Quaternion spawnRotation = transform.rotation;

            devOutput += $"Spawning projectile at position: {spawnPosition} with rotation: {spawnRotation.eulerAngles}\n";
            devOutput += $"Velocity Mag of ship: {characterController.velocity.magnitude}\n";

            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation, projectileParent);

            projectile.GetComponent<PlayerProjectileController>().Speed = Mathf.Max(7f, characterController.velocity.magnitude * 1.2f);

            StartCoroutine(laserRecharge());
            Destroy(projectile, 7f);
        }
        //Test if fire button not pressed
        else if (fireAction.ReadValue<float>() == 0) {
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += "Fire button not pressed\n";

            //Turn on button
            ChangeButtonColor(btnFire, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }
        //
        //Turn      

        float input = turnAction.ReadValue<float>();

        if (input < 0) {
            //Turn on button
            ChangeButtonColor(btnLTurn, new Color(245 / 255f, 245 / 255f, 245 / 255f));
            //Turn off button
            ChangeButtonColor(btnRTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }
        else if (input > 0) {
            //Turn on button
            ChangeButtonColor(btnRTurn, new Color(245 / 255f, 245 / 255f, 245 / 255f));
            //Turn off button
            ChangeButtonColor(btnLTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }
        else {
            //Turn off buttons
            ChangeButtonColor(btnLTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
            ChangeButtonColor(btnRTurn, new Color(200 / 255f, 200 / 255f, 200 / 255f));
        }

        desiredYRotation += input * rotationSpeed * 10 * Time.deltaTime;
        //DEV CODE - DELETE BEFORE FINAL BUILD
        devOutput += $"Turn Input: {turnAction.ReadValue<float>()}\n";

        //Lerp ship rotation
        float currentRotationY = transform.rotation.eulerAngles.y;
        float smoothedRotationY = Mathf.LerpAngle(currentRotationY, desiredYRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, smoothedRotationY, 0);

        //Limit speed
        currentMovement.x = Mathf.Clamp(currentMovement.x, -speedLimit, speedLimit);
        currentMovement.z = Mathf.Clamp(currentMovement.z, -speedLimit, speedLimit);

        //Snap ship to 0 Y coord
        currentMovement.y += -currentMovement.y;

        //Move ship controller
        characterController.Move(currentMovement * Time.deltaTime);

        //DEV CODE - DELETE BEFORE FINAL BUILD
        if (breakAction.ReadValue<float>() != 0) {
            Debug.Break();
        }

        if (spawnAction.ReadValue<float>() != 0 && !spawnedThisPress) {
            gameManager.SpawnEnemy();
            spawnedThisPress = true;
        }
        else if (spawnAction.ReadValue<float>() == 0) {
            spawnedThisPress = false;
        }

        devOutput += $"Ship Veloctiy: {currentMovement}\n";
        devOutput += $"Ship Rot: {transform.rotation.eulerAngles}\n";
        devOutput += "====================\n";
        if (gameManager.ShipDebug) {
            Debug.Log(devOutput);
        }
    }
    //
    private void OnTriggerEnter(Collider other) {
        //DEV CODE - DELETE BEFORE FINAL BUILD
        string devOutput = "\tDEBUG - Trigger Enter\n====================\n";

        if (other.gameObject.tag == "Wall" && !inTrigger) {
            inTrigger = true;
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += $"Wall Collision\n";
            devOutput += $"Wall dimensions: ({other.transform.localScale.x},{other.transform.localScale.z})\n";

            Vector3 newPosition = Vector3.zero;
            if (other.transform.localScale.x > other.transform.localScale.z) {
                if (other.transform.position.z > 0) {
                    //DEV CODE - DELETE BEFORE FINAL BUILD
                    devOutput += "Hit the North wall\n";
                }
                else {
                    //DEV CODE - DELETE BEFORE FINAL BUILD
                    devOutput += "Hit the South wall\n";
                }


                newPosition = new Vector3(transform.position.x, 0, -transform.position.z < 0 ? -(gameManager.LevelSize.y / 2 - (transform.localScale.z * 1.7f)) : (gameManager.LevelSize.y / 2 - (transform.localScale.z * 1.7f)));
            }
            else {

                if (other.transform.position.x > 0) {
                    //DEV CODE - DELETE BEFORE FINAL BUILD
                    devOutput += "Hit the East wall\n";
                }
                else {
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
        else if (other.gameObject.tag == "Asteroid" && !isInvulnerable) {

            //Split asteroid
            other.GetComponent<AstroidController>().Split(2);

            //adjust health (DEV: lvl1 = 5%, lvl2 = 10%, lvl3 = 20%)
            int asteroidSize = other.GetComponent<AstroidController>().Size;
            int damage;

            if (asteroidSize == 1) {
                damage = (int) (maxHealth * .05f);
            }
            else if (asteroidSize == 2) {
                damage = (int) (maxHealth * .1f);
            }
            else {
                damage = (int) (maxHealth * .2f);
            }

            //Debug.Log($"Damage dealt: {damage}");
            UpdateHealth(-damage);
        }


        //DEV CODE - DELETE BEFORE FINAL BUILD
        devOutput += "====================\n";
        if (gameManager.WallDebug) {
            Debug.Log(devOutput);
        }
    }
    //
    private void OnTriggerExit(Collider other) {
        //DEV CODE - DELETE BEFORE FINAL BUILD
        string devOutput = "\tDEBUG - Trigger Exit\n====================\n";
        inTrigger = false;
        //DEV CODE - DELETE BEFORE FINAL BUILD
        devOutput += "====================\n";
        if (gameManager.WallDebug) {
            Debug.Log(devOutput);
        }
    }

    //**UTILITY METHODS**
    private void ChangeButtonColor(Button btn, Color newColor) {
        ColorBlock cb = btn.colors;
        cb.normalColor = newColor;
        btn.colors = cb;
    }
    //
    public void UpdateHealth(int amount) {
        if (amount < 0) {
            //Go invulnerable
            StartCoroutine(Invulnerability(3, 1));
        }

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }
    //
    public void UpdateScore(int amount) {
        score += amount;
        score = Mathf.Max(score, 0);
    }
    //**COROUTINES**
    IEnumerator laserRecharge() {
        yield return new WaitForSeconds(laserRechargeTime);
        laserCharged = true;
    }

    IEnumerator Invulnerability(float duration, float pulseFrequency) {
        //Set flag
        isInvulnerable = true;
        if (gameManager.ShipDebug) {
            Debug.Log("Player ship is now invulnerable");
        }

        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();

        // Store the initial colors of the materials
        Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
        foreach (var renderer in meshes) {
            foreach (var material in renderer.materials) {
                if (!originalColors.ContainsKey(material)) {
                    originalColors[material] = material.color;
                }
            }
        }

        float elapsedTime = 0f;
        float halfCycle = pulseFrequency / 2f;


        while (elapsedTime < duration) {
            // Calculate the interpolation factor based on ping-pong effect
            float t = Mathf.PingPong(elapsedTime, halfCycle) / halfCycle;

            // Apply the color interpolation to all materials
            foreach (var renderer in meshes) {
                foreach (var material in renderer.materials) {
                    Color originalColor = originalColors[material];
                    Color targetColor = Color.red;
                    material.color = Color.Lerp(originalColor, targetColor, t);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore original colors
        foreach (MeshRenderer renderer in meshes) {
            foreach (Material material in renderer.materials) {
                if (originalColors.ContainsKey(material)) {
                    material.color = originalColors[material];
                }
            }
        }

        isInvulnerable = false;
        if (gameManager.ShipDebug) {
            Debug.Log("Player ship is now vulnerable");
        }
    }
}

