using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
    //
    Vector3 currentMovement = Vector3.zero;
    float desiredYRotation = 0f;
    bool inTrigger = false;
    bool laserCharged = true;

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
    }
    //
    void Update() {
        //DEV CODE - DELETE BEFORE FINAL BUILD
        string devOutput = "\tDEBUG - Update\n====================\n";
        devOutput += $"Inside trigger? {inTrigger}\n";

        //*Input Handling*
        //Thrust
        if (thrustAction.ReadValue<float>() != 0) {
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
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += "No thrust\n";
        }
        //
        //Fire
        if (fireAction.ReadValue<float>() != 0 && laserCharged) {
            laserCharged = false;

            Vector3 spawnPosition = projectileSpawn.position;
            Quaternion spawnRotation = transform.rotation;

            devOutput += $"Spawning projectile at position: {spawnPosition} with rotation: {spawnRotation.eulerAngles}\n";
            devOutput += $"Velocity Mag of ship: {characterController.velocity.magnitude}\n";

            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

            projectile.GetComponent<PlayerProjectileController>().SetInitialSpeed(characterController.velocity.magnitude);

            StartCoroutine(laserRecharge());
            Destroy(projectile, 7f);
        }
        //Test if fire button not pressed
        else if (fireAction.ReadValue<float>() == 0) {
            //DEV CODE - DELETE BEFORE FINAL BUILD
            devOutput += "Fire button not pressed\n";
        }
        //
        //Turn      
        desiredYRotation += turnAction.ReadValue<float>() * rotationSpeed * 10 * Time.deltaTime;
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
        devOutput += $"Ship Veloctiy: {currentMovement}\n";
        devOutput += $"Ship Rot: {transform.rotation.eulerAngles}\n";
        devOutput += "====================\n";
        if (gameManager.debugMode && gameManager.updateDebug) {
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
        //DEV CODE - DELETE BEFORE FINAL BUILD
        devOutput += "====================\n";
        if (gameManager.debugMode && gameManager.triggerDebug) {
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
        if (gameManager.debugMode && gameManager.triggerDebug) {
            Debug.Log(devOutput);
        }
    }

    //**COROUTINES**
    IEnumerator laserRecharge() {
        yield return new WaitForSeconds(laserRechargeTime);
        laserCharged = true;
    }

}

