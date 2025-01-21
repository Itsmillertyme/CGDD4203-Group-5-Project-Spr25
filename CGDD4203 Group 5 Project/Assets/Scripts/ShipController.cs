using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ShipController : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] float thrustForce = 10f;
    [SerializeField] float rotationSpeed = 5f;

    [Header("References")]
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawn;

    [SerializeField] bool debugMode;
    CharacterController characterController;

    Vector3 thrustDirection = Vector3.zero;
    Vector3 shipVelocity = Vector3.zero;
    float desiredRotationY = 0f;

    InputAction thrustAction;
    InputAction turnAction;
    InputAction fireAction;

    void Awake() {
        // Cache the CharacterController.
        characterController = GetComponent<CharacterController>();

        // Initialize the input actions
        thrustAction = inputActionAsset.FindAction("Thrust");
        turnAction = inputActionAsset.FindAction("Turn");
        fireAction = inputActionAsset.FindAction("Fire");
    }

    void OnEnable() {
        // Subscribe to input actions
        thrustAction.performed += OnThrust;
        thrustAction.canceled += OnThrust;
        turnAction.performed += OnTurn;
        fireAction.performed += OnFire;
    }

    void OnDisable() {
        // Unsubscribe from input actions
        thrustAction.performed -= OnThrust;
        thrustAction.canceled -= OnThrust;
        turnAction.performed -= OnTurn;
        fireAction.performed -= OnFire;
    }

    void Update() {

        // Apply thrust direction
        if (thrustDirection != Vector3.zero) {
            shipVelocity += thrustDirection;
        }

        // Smoothly rotate the ship.
        float currentRotationY = transform.rotation.eulerAngles.y;
        float smoothedRotationY = Mathf.LerpAngle(currentRotationY, desiredRotationY, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, smoothedRotationY, 0);

        //speed limit
        shipVelocity.x = Mathf.Clamp(shipVelocity.x, -10, 10);
        shipVelocity.z = Mathf.Clamp(shipVelocity.z, -10, 10);


        //Debug.Log($"Ship Veloctiy: {shipVelocity}");
        //Debug.Log($"Ship Rot: {transform.rotation.eulerAngles}");
        characterController.Move(shipVelocity * Time.deltaTime);
    }

    public void OnThrust(InputAction.CallbackContext context) {
        //Debug.Log($"Thrust context phase: {context.phase}");

        if (context.performed) {
            // Apply thrust in the forward direction.
            thrustDirection = (transform.rotation * Vector3.forward).normalized * thrustForce * .01f;
            //Debug.Log($"Thrust applied, Thrust direction: {thrustDirection.x},{thrustDirection.z}");
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (context.canceled) {
            // Stop thrust when input is canceled.
            thrustDirection = Vector3.zero;
            transform.GetChild(1).gameObject.SetActive(false);
            //Debug.Log($"Thrust released");
        }
    }

    public void OnTurn(InputAction.CallbackContext context) {
        if (context.performed) {
            float turnInput = context.ReadValue<float>();
            desiredRotationY += turnInput * rotationSpeed;
            //Debug.Log($"Turn Input: {turnInput}, Desired Rotation Y: {desiredRotationY}");
        }
    }

    public void OnFire(InputAction.CallbackContext context) {
        //Debug.Log($"Fire context phase: {context.phase}");
        //Debug.Log($"action performed? {context.performed}");


        if (context.phase == InputActionPhase.Performed) {
            Vector3 spawnPosition = projectileSpawn.position;
            Quaternion spawnRotation = transform.rotation;

            //Debug.Log($"Spawn Pos: {spawnPosition}");
            //Debug.Log($"Spawn Rot: {spawnRotation.eulerAngles}");

            if (spawnPosition != new Vector3(0, 0, 0.75f)) {
                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
                projectile.GetComponent<PlayerProjectileController>().SetInitialSpeed(characterController.velocity.magnitude);

                Debug.Log(characterController.velocity.magnitude);
                Destroy(projectile, 7f);
            }
        }
    }

}

