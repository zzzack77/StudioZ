using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [Header("Rigidbodys")]
    [SerializeField] private Rigidbody bodyRB;
    [SerializeField] public Rigidbody L_handRB;
    [SerializeField] public Rigidbody R_handRB;

    [Header("Shoulder Points")]
    [SerializeField] private Transform L_shoulderPoint;
    [SerializeField] private Transform R_shoulderPoint;


    [Header("Arm and joint settings")]
    private ConfigurableJoint L_currentJoint;
    private ConfigurableJoint R_currentJoint;
    [SerializeField] private float armLength = 4.2f;
    [SerializeField] private float jointBreakingSensitivity = 0.99f;

    [Header("Player Settings")]
    [SerializeField] private bool shouldersInLine = false;
    private Vector2 shoulderPosition;
    [SerializeField] private bool invertGrippingInput = true;


    // Spawning and checkpoints
    private bool isRespawning;
    private Vector2 spawnPoint;
    public Vector2 SpawnPoint
    {
        get => spawnPoint;
        set
        {
            if (spawnPoint != value)
            {
                spawnPoint = value;
                currentCheckpoint = Vector2.zero;
                SpawnPlayer();
            }
        }
    }
    private Vector2 potentialCheckPoint;
    public Vector2 PotentialCheckpoint
    {
        get => potentialCheckPoint;
        set
        {
            if (potentialCheckPoint != value)
            {
                potentialCheckPoint = value;
                Debug.Log(potentialCheckPoint);
            }
        }
    }
    [SerializeField] private Vector2 currentCheckpoint;
    // Left Grips
    public bool L_canGripFinish { get; set; }
    public bool L_canGripCheckpoint { get; set; }
    public bool L_canGripJug { get; set; }
    public bool L_canGripCrimp { get; set; }
    public bool L_canGripPocket { get; set; }
    // Right Grips
    public bool R_canGripFinish { get; set; }
    public bool R_canGripCheckpoint { get; set; }
    public bool R_canGripJug { get; set; }
    public bool R_canGripCrimp { get; set; }
    public bool R_canGripPocket { get; set; }

    [Header("Grip Settings")]
    [SerializeField] private bool L_isGripping = false;
    [SerializeField] private bool R_isGripping = false;

    // ---- Gamepad Input Values ----

    // Trigger values
    private float leftTrigger;
    private float rightTrigger;
    // Shoulder values
    private float leftShoulder;
    private float rightShoulder;
    // Joystick values
    private Vector2 leftStick;
    private Vector2 rightStick;
    // Dead zones
    private float triggerDeadZone = 0.1f;
    private float joystickDeadZone = 0.2f;

    [Header("Joystick Gripping Settings")]
    [SerializeField] float forceMultiplier = 15f;
    [SerializeField] float downThreshold = -0.85f;        // Stick must be this downward to apply following settings
    [SerializeField] float singleHandUpwardBoost = 1.6f; // Force multiplier on y axis when going straight up
    [SerializeField] float doubleHandedUpwardBoost = 1;
    [SerializeField] float horizontalDamping = 0.4f;      // Force dampener on x axis when going straight up
    [SerializeField] float swingDampening = 0.98f;        // The rate which the x axis linear velocity multiplies by on fixed update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
        SpawnPlayer();
    }
    private void Start()
    {
        shoulderPosition = L_shoulderPoint.position;
        SpawnPlayer();
    }
    private void SpawnPlayer()
    {
        isRespawning = true;
        bodyRB.linearVelocity = Vector3.zero;
        bodyRB.constraints = RigidbodyConstraints.FreezeAll;
        if (spawnPoint != Vector2.zero && currentCheckpoint == Vector2.zero)
        {
            bodyRB.transform.position = new Vector2(spawnPoint.x, spawnPoint.y - armLength);
        }
        else
        {
            bodyRB.transform.position = new Vector2(currentCheckpoint.x, currentCheckpoint.y - armLength);
        }
    }

    // Update is called once per frame
    void Update()
    {
        JointChecking(); // Check if joints need to be created or destroyed
        InitializeGamepad(); // Read gamepad inputs
        ControllerMovement(); // Move hands based on joystick input
        GrippingLogic(); // Handle gripping logic


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
        }
    }
    private void FixedUpdate()
    {
        // Apply swinging forces based on joystick input when gripping
        LGrippedHandMovement();
        RGrippedHandMovement();
    }
    private void InitializeGamepad()
    {
        // Get the current gamepad
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.Log("No controller connected.");
            return;
        }

        // Read joystick values
        leftStick = gamepad.leftStick.ReadValue();
        rightStick = gamepad.rightStick.ReadValue();

        leftTrigger = gamepad.leftTrigger.ReadValue();
        rightTrigger = gamepad.rightTrigger.ReadValue();

        leftShoulder = gamepad.leftShoulder.ReadValue();
        rightShoulder = gamepad.rightShoulder.ReadValue();

        if (gamepad.buttonNorth.wasPressedThisFrame)
        {
            SpawnPlayer();
        }
        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            if (shouldersInLine)
            {
                L_shoulderPoint.localPosition = new Vector2(shoulderPosition.x, shoulderPosition.y);
                R_shoulderPoint.localPosition = new Vector2(-shoulderPosition.x, shoulderPosition.y);
            }
            else
            {
                L_shoulderPoint.localPosition = new Vector2(0, shoulderPosition.y);
                R_shoulderPoint.localPosition = new Vector2(0, shoulderPosition.y);
            }
                
        }
        if (gamepad.buttonEast.wasReleasedThisFrame)
        {
            shouldersInLine = !shouldersInLine;
        }
    }
    private void LGrippedHandMovement()
    {
        if (!L_isGripping) return;

        GrippedBodyMovement(leftStick);
    }

    private void RGrippedHandMovement()
    {
        if (!R_isGripping) return;

        GrippedBodyMovement(rightStick);
    }
    private void GrippedBodyMovement(Vector2 joyStick)
    {
        // If stick is pushed downward
        if ((invertGrippingInput && joyStick.y < downThreshold) || (!invertGrippingInput && joyStick.y > -downThreshold))
        {
            Debug.Log("power");
            // apply bias for double handed or single handed grip types
            if (R_isGripping && L_isGripping) joyStick.y *= doubleHandedUpwardBoost;
            else joyStick.y *= singleHandUpwardBoost; 
            joyStick.x *= horizontalDamping;

            // Gradually dampen swinging
            Vector3 bodyVelocity = bodyRB.linearVelocity;
            bodyVelocity.x *= swingDampening;
            bodyRB.linearVelocity = bodyVelocity;
        }

        // Apply force
        if (invertGrippingInput) bodyRB.AddForce(-joyStick * forceMultiplier, ForceMode.Acceleration);
        else bodyRB.AddForce(joyStick * forceMultiplier, ForceMode.Acceleration);
    }
    // Move hand based on joystick input and handle gripping
    private void ControllerMovement()
    {
        if (!L_isGripping)
        {
            Vector3 L_WorldOffset = new Vector3(
                Mathf.Clamp(leftStick.x, -1, 1) * armLength,
                Mathf.Clamp(leftStick.y, -1, 1) * armLength,
                0f);
            
            L_handRB.transform.position = L_WorldOffset + L_shoulderPoint.transform.position;
        }
        if(!R_isGripping)
        {
            Vector3 R_WorldOffset = new Vector3(
                Mathf.Clamp(rightStick.x, -1, 1) * armLength,
                Mathf.Clamp(rightStick.y, -1, 1) * armLength,
                0f);
            R_handRB.transform.position = R_WorldOffset + R_shoulderPoint.transform.position;
        }
    }
    private void GrippingLogic()
    {
        bool leftTriggerPressed = leftTrigger >= triggerDeadZone;
        bool leftShoulderPressed = leftShoulder >= triggerDeadZone;
        bool rightTriggerPressed = rightTrigger >= triggerDeadZone;
        bool rightShoulderPressed = rightShoulder >= triggerDeadZone;

        // If any grip button is pressed, remove body constraints
        //if (leftTriggerPressed || leftShoulderPressed || rightTriggerPressed || rightShoulderPressed)
        //{
        //    bodyRB.constraints = RigidbodyConstraints.None;
        //    bodyRB.constraints = RigidbodyConstraints.FreezePositionZ;
        //    bodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        //}

        // Left Hand Grip Logic
        if (leftTriggerPressed)
        {
            if (L_canGripFinish)
            {
                OnLGrip();
                Finish();
            }
            else if (L_canGripCheckpoint)
            {
                OnLGrip();
                SetCheckPoint();
            }
            else if (L_canGripJug && !leftShoulderPressed) OnLGrip();
            else if (L_canGripPocket && leftShoulderPressed) OnLGrip();
            else
            {
                L_isGripping = false;
                L_handRB.constraints = RigidbodyConstraints.None;
            }
        }
        else if (leftShoulderPressed)
        {
            if (L_canGripFinish)
            {
                OnLGrip();
                Finish();
            }
            else if (L_canGripCheckpoint)
            {
                OnLGrip();
                SetCheckPoint();
            }
            else if (L_canGripCrimp && !leftTriggerPressed) OnLGrip();
            else
            {
                L_isGripping = false;
                L_handRB.constraints = RigidbodyConstraints.None;
            }
        }
        else
        {
            L_isGripping = false;
            L_handRB.constraints = RigidbodyConstraints.None;
        }
        // Right Hand Grip Logic
        if (rightTriggerPressed)
        {
            if (R_canGripFinish)
            {
                OnRGrip();
                Finish();
            }
            else if (R_canGripCheckpoint)
            {
                OnRGrip();
                SetCheckPoint();
            }
            else if (R_canGripJug && !rightShoulderPressed) OnRGrip();
            else if (R_canGripPocket && rightShoulderPressed) OnRGrip();
            else
            {
                R_isGripping = false;
                R_handRB.constraints = RigidbodyConstraints.None;
            }
        }
        else if (rightShoulderPressed)
        {
            if (R_canGripFinish)
            {
                OnRGrip();
                Finish();
            }
            else if (R_canGripCheckpoint)
            {
                OnRGrip();
                SetCheckPoint();
            }
            else if (R_canGripCrimp && !rightTriggerPressed) OnRGrip();
            else
            {
                R_isGripping = false;
                R_handRB.constraints = RigidbodyConstraints.None;
            }
        }
        else
        {
            R_isGripping = false;
            R_handRB.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnLGrip()
    {
        if (isRespawning)
        {
            isRespawning = false;
            bodyRB.constraints = RigidbodyConstraints.None;
            bodyRB.constraints = RigidbodyConstraints.FreezePositionZ;
            bodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
        L_isGripping = true;
        L_handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void OnRGrip()
    {
        if (isRespawning) 
        {
            isRespawning = false;
            bodyRB.constraints = RigidbodyConstraints.None;
            bodyRB.constraints = RigidbodyConstraints.FreezePositionZ;
            bodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
        R_isGripping = true;
        R_handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void Finish()
    {
        Debug.Log("Finish Reached!");
    }
    private void SetCheckPoint()
    {
        if (currentCheckpoint != potentialCheckPoint)
        {
            currentCheckpoint = potentialCheckPoint;
            Debug.Log("Checkpoint Reached!");
        }
    }
    // Check distance between hand and body to create/destroy joint
    private void JointChecking()
    {
        if (L_isGripping)
        {
            float distance = Vector3.Distance(bodyRB.position, L_handRB.position);

            // When hand is beyond arm length and no joint exists create joint
            if (L_currentJoint == null && distance >= armLength)
            {
                CreateLeftJoint();
            }

            // When hand comes back within range remove joint
            if (L_currentJoint != null && distance < armLength * jointBreakingSensitivity)
            {
                Destroy(L_currentJoint);
                L_currentJoint = null;
            }
        }
        else
        {
            if (L_currentJoint != null)
            {
                Destroy(L_currentJoint);
                L_currentJoint = null;
            }
        }
        if (R_isGripping)
        {
            float distance = Vector3.Distance(bodyRB.position, R_handRB.position);
            // When hand is beyond arm length and no joint exists create joint
            if (R_currentJoint == null && distance >= armLength)
            {
                CreateRightJoint();
            }
            // When hand comes back within range remove joint
            if (R_currentJoint != null && distance < armLength * jointBreakingSensitivity)
            {
                Destroy(R_currentJoint);
                R_currentJoint = null;
            }
        }
        else
        {
            if (R_currentJoint != null)
            {
                Destroy(R_currentJoint);
                R_currentJoint = null;
            }
        }
    }
    // Create a configurable joint between body and hand
    void CreateLeftJoint()
    {
        L_currentJoint = bodyRB.gameObject.AddComponent<ConfigurableJoint>();
        L_currentJoint.connectedBody = L_handRB;

        // Prevent Unity from auto adjusting anchor positions
        L_currentJoint.autoConfigureConnectedAnchor = false;
        // L_shoulderPoint.position
        L_currentJoint.anchor = L_shoulderPoint.localPosition;
        L_currentJoint.connectedAnchor = Vector3.zero;

        // Limit motion to simulate a rope/arm constraint
        L_currentJoint.xMotion = ConfigurableJointMotion.Limited;
        L_currentJoint.yMotion = ConfigurableJointMotion.Limited;
        L_currentJoint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = armLength; // arm can stretch this far
        L_currentJoint.linearLimit = linearLimit;
    }
    private void CreateRightJoint()
    {
        R_currentJoint = bodyRB.gameObject.AddComponent<ConfigurableJoint>();
        R_currentJoint.connectedBody = R_handRB;
        // Prevent Unity from auto adjusting anchor positions
        R_currentJoint.autoConfigureConnectedAnchor = false;
        R_currentJoint.anchor = R_shoulderPoint.localPosition;
        R_currentJoint.connectedAnchor = Vector3.zero;
        // Limit motion to simulate a rope/arm constraint
        R_currentJoint.xMotion = ConfigurableJointMotion.Limited;
        R_currentJoint.yMotion = ConfigurableJointMotion.Limited;
        R_currentJoint.zMotion = ConfigurableJointMotion.Limited;
        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = armLength; // arm can stretch this far
        R_currentJoint.linearLimit = linearLimit;
    }
}
