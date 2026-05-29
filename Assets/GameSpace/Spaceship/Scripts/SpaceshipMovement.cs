using UnityEngine;
using UnityEngine.InputSystem;

using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipMovement : MonoBehaviour
{
    // Defines the rigidbody for the object to use
    private Rigidbody rb;

    // Sets up the keyboard controls for the spaceship
    private PlayerInputActions spaceshipControls;
    private InputAction thrustInput;
    private InputAction pitchInput;
    private InputAction yawInput;
    private InputAction rollInput;

    // Input type
    public static bool angMomentum;       //false for no angular momentum, true for angular momentum

    // Used to change the movement of the spaceship
    private float thrust;
    private float pitch;
    private float yaw;
    private float roll;

    // Spaceship feel values
    public float MaxSpeed = 4.25f;
    public float LinearDamping = 0.5f;
    public float AngularDamping = 0.5f;

    public static float ThrustScaler = 50.0f;
    public static float PitchScaler = 1.0f;
    public static float YawScaler = 1.5f;
    public static float RollScaler = 1.1f;

    // Direct rotation feel (deg/sec) instead of per tick
    public float DirectRotationSpeed = 120.0f;

    // Cache TrackIR rotation so it doesn't "drop to zero" between pose updates
    private float trackirPitch;
    private float trackirYaw;
    private float trackirRoll;
    private float trackirThrust;


    private void OnEnable()
    {
        thrustInput = InputSystem.actions.FindAction("Thrust");
        thrustInput.Enable();

        pitchInput = InputSystem.actions.FindAction("Pitch");
        pitchInput.Enable();

        yawInput = InputSystem.actions.FindAction("Yaw");
        yawInput.Enable();

        rollInput = InputSystem.actions.FindAction("Roll");
        rollInput.Enable();
    }

    public void OnDisable()
    {
        thrustInput.Disable();
        pitchInput.Disable();
        yawInput.Disable();
        rollInput.Disable();
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;

            // Adjust these values for feel
            rb.linearDamping = LinearDamping;
            rb.angularDamping = AngularDamping;

            // Helps smooth rendering between FixedUpdate steps
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        //get settings values
        angMomentum = PlayerPrefs.GetInt("angMoment") == 1;
        
        if (PlayerPrefs.HasKey("ptchScl"))
            PitchScaler = PlayerPrefs.GetFloat("ptchScl");

        if (PlayerPrefs.HasKey("rollScl"))
            RollScaler = PlayerPrefs.GetFloat("rollScl");

        if (PlayerPrefs.HasKey("yawScl"))
            YawScaler = PlayerPrefs.GetFloat("yawScl");

        // trade show mode speed settings
        if (GameModeMenu.gameModeSetting == 0)  // Trade show mode
        {
            float speedMultiplier = 1.75f;
            float accelMultiplier = 1.75f;
            float dampingMultiplier = 0.3f;

            MaxSpeed *= speedMultiplier;
            ThrustScaler *= accelMultiplier;

            LinearDamping *= dampingMultiplier;
            AngularDamping *= dampingMultiplier;

            rb.linearDamping = LinearDamping;
            rb.angularDamping = AngularDamping;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Read keyboard every frame
        float kbThrust = thrustInput.ReadValue<float>() * ThrustScaler;

        // For direct mode we treat pitch/yaw/roll as "input" (-1..1) and convert to deg/sec later
        float kbPitch = pitchInput.ReadValue<float>();
        float kbYaw = yawInput.ReadValue<float>();
        float kbRoll = -rollInput.ReadValue<float>();

        // Read from TrackIR
        if (TrackIRManager.Instance != null && TrackIRManager.Instance.IsTracking)
        {
            trackirPitch = TrackIRManager.Instance.HeadPitch;
            trackirYaw = TrackIRManager.Instance.HeadYaw;
            trackirRoll = TrackIRManager.Instance.HeadRoll;

            trackirThrust = (TrackIRManager.Instance.HeadZPos * -1f * ThrustScaler) + 0.2f;
        }

        // Combine inputs (TrackIR stays stable between updates; keyboard is continuous)
        thrust = kbThrust + trackirThrust;
        pitch = kbPitch + trackirPitch;
        yaw = kbYaw + trackirYaw;
        roll = kbRoll + trackirRoll;
    }

    // Use for physics operations (is called at fixed time intervals not every frame)
    void FixedUpdate()
    {
        float currentSpeed = rb.linearVelocity.magnitude;

        // Thrust, if less than max speed
        if (currentSpeed < MaxSpeed)
            rb.AddRelativeForce(Vector3.forward * thrust, ForceMode.Acceleration);

        // Rotation
        // head match rotation
        if (angMomentum == false)
        {
            // Convert input to degrees per second, then apply per fixed step
            Vector3 eulerDelta =
                new Vector3(pitch * PitchScaler, yaw * YawScaler, roll * RollScaler) * DirectRotationSpeed * Time.fixedDeltaTime;

            // Use Rigidbody rotation
            rb.MoveRotation(rb.rotation * Quaternion.Euler(eulerDelta));
        }
        //angular momentum
        else
        {
            Vector3 rotation = new(pitch * PitchScaler, yaw * YawScaler, roll * RollScaler);
            rb.AddRelativeTorque(rotation, ForceMode.Acceleration);
        }
    }
}
