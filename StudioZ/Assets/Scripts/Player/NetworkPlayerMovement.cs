using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private TimerHandeler timerHandeler;

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
    [SerializeField] private float handMoveSpeed = 100;
    [SerializeField] private float jointBreakingSensitivity = 0.99f;
    [SerializeField] private float jointSpring = 500f;
    [SerializeField] private float jointDamper = 80f;
    [SerializeField] private float projectionDistance = 0.1f;
    [SerializeField] private float projectionAngle = 5f;

    [Header("Player Settings")]
    private bool shouldRestartTimer = false;
    [SerializeField] private bool invertGrippingInput = true;
    [SerializeField] private bool hasFinished;

    // Vibration
    private Coroutine GripVibrationCoroutine;
    [SerializeField] private bool vibrationEnabled = true;
    [SerializeField] private float vibrationDuration = 0.05f;
    [SerializeField] private float vibrationStrengthLowFrequency = 0.05f;
    [SerializeField] private float vibrationStrengthHighFrequency = 0.1f;



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
            }
        }
    }
    public Vector2 currentCheckpoint { get; set; }

    // Player on player griping
    public GameObject L_playerGrippedGameObject {  get; set; }
    public GameObject R_playerGrippedGameObject { get; set; }
    private Vector3 L_distanceFromHandToGrippedObject;
    private Vector3 R_distanceFromHandToGrippedObject;
    private bool L_isGrippingPlayer;
    private bool R_isGrippingPlayer;
    // Breaker holds 


    // Left Grips
    public bool L_canGripFinish { get; set; }
    public bool L_canGripCheckpoint { get; set; }
    public bool L_canGripPlayer { get; set; }
    public bool L_canGripJug { get; set; }
    public bool L_canGripCrimp { get; set; }
    public bool L_canGripPocket { get; set; }
    public bool L_canGripBreaker { get; set; }

    // Right Grips
    public bool R_canGripFinish     { get; set; }
    public bool R_canGripCheckpoint { get; set; }
    public bool R_canGripPlayer     { get; set; }
    public bool R_canGripJug        { get; set; }
    public bool R_canGripCrimp  { get; set; }
    public bool R_canGripPocket     { get; set; }

    [Header("Grip Settings")]
    public bool L_isGripping = false;
    public bool R_isGripping = false;

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
    // Vibration handelers
    private bool L_hasVibrated;
    private bool R_hasVibrated;

    [Header("Joystick Gripping Settings")]
    [SerializeField] float forceMultiplier = 15f;
    [SerializeField] float downThreshold = -0.85f;        // Stick must be this downward to apply following settings
    [SerializeField] float singleHandUpwardBoost = 2f; // Force multiplier on y axis when going straight up
    [SerializeField] float doubleHandedUpwardBoost = 1.4f;
    [SerializeField] float horizontalDamping = 1;      // Force dampener on x axis when going straight up
    [SerializeField] float swingDampening = 0.98f;        // The rate which the x axis linear velocity multiplies by on fixed update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
        SpawnPlayer();
    }
    private void Start()
    {
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        isRespawning = true;
        ResetGrips();
        bodyRB.linearVelocity = Vector3.zero;
        bodyRB.constraints = RigidbodyConstraints.FreezeAll;
        if (currentCheckpoint == Vector2.zero)
        {
            bodyRB.transform.position = new Vector2(spawnPoint.x, spawnPoint.y - armLength);
            L_handRB.transform.position = L_shoulderPoint.transform.position;
            R_handRB.transform.position = R_shoulderPoint.transform.position;
            timerHandeler.isTimerRunning = false;
            shouldRestartTimer = true;
            hasFinished = false;
        }
        else
        {
            bodyRB.transform.position = new Vector2(currentCheckpoint.x, currentCheckpoint.y - armLength);
            L_handRB.transform.position = L_shoulderPoint.transform.position;
            R_handRB.transform.position = R_shoulderPoint.transform.position;
        }
    }
    private void ResetGrips()
    {
        L_isGripping = false;
        L_isGrippingPlayer = false;
        L_playerGrippedGameObject = null;
        L_canGripFinish = false;
        L_canGripCheckpoint = false;
        L_canGripPlayer = false;
        L_canGripJug = false;
        L_canGripCrimp = false;
        L_canGripPocket = false;

        R_isGripping = false;
        R_isGrippingPlayer = false;
        R_playerGrippedGameObject = null;
        R_canGripFinish = false;
        R_canGripCheckpoint = false;
        R_canGripPlayer = false;
        R_canGripJug = false;
        R_canGripCrimp = false;
        R_canGripPocket = false;
    }

    // Update is called once per frame
    void Update()
    {
        JointChecking(); // Check if joints need to be created or destroyed
        InitializeGamepad(); // Read gamepad inputs
        ControllerMovement(); // Move hands based on joystick input
        GrippingLogic(); // Handle gripping logic

        if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.SetUI(true);
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
             currentCheckpoint = Vector2.zero;
             SpawnPlayer();
        }
        if (gamepad.buttonSouth.wasPressedThisFrame && hasFinished) GameManager.Instance.SetUI(true);
        if (gamepad.startButton.wasPressedThisFrame) GameManager.Instance.SetUI(true);
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
        //Debug.Log(bodyRB.GetAccumulatedForce());
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

            Vector3 targetPos = L_WorldOffset + L_shoulderPoint.transform.position;
            L_handRB.transform.position = Vector3.MoveTowards(L_handRB.transform.position, targetPos, handMoveSpeed * Time.deltaTime);
        }

        if (!R_isGripping)
        {
            Vector3 R_WorldOffset = new Vector3(
                Mathf.Clamp(rightStick.x, -1, 1) * armLength,
                Mathf.Clamp(rightStick.y, -1, 1) * armLength,
                0f);

            Vector3 targetPos = R_WorldOffset + R_shoulderPoint.transform.position;
            R_handRB.transform.position = Vector3.MoveTowards(R_handRB.transform.position, targetPos, handMoveSpeed * Time.deltaTime);
        }
    }
    private void GrippingLogic()
    {
        bool leftTriggerPressed = leftTrigger >= triggerDeadZone;
        bool leftShoulderPressed = leftShoulder >= triggerDeadZone;
        bool rightTriggerPressed = rightTrigger >= triggerDeadZone;
        bool rightShoulderPressed = rightShoulder >= triggerDeadZone;

        // Left Hand Grip Logic
        if (leftTriggerPressed)
        {
            if (L_canGripFinish) { OnLGrip(); Finish(); }
            else if (L_canGripCheckpoint) { OnLGrip(); SetCheckPoint(); }
            else if (L_canGripPlayer) { OnLPlayerGrip(); }
            else if (L_canGripJug && !leftShoulderPressed) OnLGrip();
            else if (L_canGripPocket && leftShoulderPressed) OnLGrip();
            else if (!isRespawning) { OnLGripRelease(); }
        }
        else if (leftShoulderPressed)
        {
            if (L_canGripFinish) { OnLGrip(); Finish(); }
            else if (L_canGripCheckpoint) { OnLGrip(); SetCheckPoint(); }
            else if (L_canGripCrimp && !leftTriggerPressed) OnLGrip();
            else if (!isRespawning) { OnLGripRelease(); }
        }
        else if (!isRespawning)
        {
            OnLPlayerLetGo();
            OnLGripRelease();
        }
        // Right Hand Grip Logic
        if (rightTriggerPressed)
        {
            if (R_canGripFinish) { OnRGrip(); Finish(); }
            else if (R_canGripCheckpoint) { OnRGrip(); SetCheckPoint(); }
            else if (R_canGripPlayer) { OnRPlayerGrip(); }
            else if (R_canGripJug && !rightShoulderPressed) OnRGrip();
            else if (R_canGripPocket && rightShoulderPressed) OnRGrip();
            else if (!isRespawning) { OnRGripRelease(); }
        }
        else if (rightShoulderPressed)
        {
            if (R_canGripFinish) { OnRGrip(); Finish(); }
            else if (R_canGripCheckpoint) { OnRGrip(); SetCheckPoint(); }
            else if (R_canGripCrimp && !rightTriggerPressed) OnRGrip();
            else if (!isRespawning) { OnRGripRelease(); }
        }
        else if(!isRespawning)
        {
            OnRPlayerLetGo();
            OnRGripRelease();
        }
    }

    private void OnLGrip()
    {
        if (isRespawning)
        {
            isRespawning = false;
            if (shouldRestartTimer)
            {
                timerHandeler.timeElapsed = 0f;
                timerHandeler.isTimerRunning = true;
                shouldRestartTimer = false;
            }
            bodyRB.constraints = RigidbodyConstraints.None;
            bodyRB.constraints = RigidbodyConstraints.FreezePositionZ;
            bodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (!L_hasVibrated && vibrationEnabled)
        {
            L_hasVibrated = true;
            if (GripVibrationCoroutine != null) StopCoroutine(GripVibrationCoroutine);
            GripVibrationCoroutine = StartCoroutine(DoGripVibration());
        }
        L_isGripping = true;
        L_handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void OnLGripRelease()
    {
        L_hasVibrated = false;
        L_isGripping = false; 
        L_handRB.constraints = RigidbodyConstraints.None;
    }
    private void OnRGrip()
    {
        if (isRespawning) 
        {
            isRespawning = false;
            if (shouldRestartTimer)
            {
                timerHandeler.timeElapsed = 0f;
                timerHandeler.isTimerRunning = true;
                shouldRestartTimer = false;
            }
            bodyRB.constraints = RigidbodyConstraints.None;
            bodyRB.constraints = RigidbodyConstraints.FreezePositionZ;
            bodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (!R_hasVibrated && vibrationEnabled)
        {
            R_hasVibrated = true;
            if (GripVibrationCoroutine != null) StopCoroutine(GripVibrationCoroutine);
            GripVibrationCoroutine = StartCoroutine(DoGripVibration());
        }
        
        R_isGripping = true;
        R_handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private IEnumerator DoGripVibration()
    {
        Gamepad.current.SetMotorSpeeds(vibrationStrengthLowFrequency, vibrationStrengthHighFrequency);
        yield return new WaitForSeconds(vibrationDuration);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
    private void OnRGripRelease()
    {
        R_hasVibrated = false;
        R_isGripping = false;
        R_handRB.constraints = RigidbodyConstraints.None;
    }
    private void OnLPlayerGrip()
    {
        if (!isRespawning && (L_playerGrippedGameObject != bodyRB.gameObject))
        {
            if (!L_isGrippingPlayer)
            {
                L_distanceFromHandToGrippedObject = L_handRB.transform.position - L_playerGrippedGameObject.transform.position;
                L_isGrippingPlayer = true;
            }
            L_isGripping = true;
            L_handRB.constraints = RigidbodyConstraints.FreezeAll;
            L_handRB.transform.position = L_playerGrippedGameObject.transform.position + L_distanceFromHandToGrippedObject;
        }
    }
    private void OnRPlayerGrip()
    {
        if (!isRespawning && (R_playerGrippedGameObject != bodyRB.gameObject))
        {
            if (!R_isGrippingPlayer)
            {
                R_distanceFromHandToGrippedObject = R_handRB.transform.position - R_playerGrippedGameObject.transform.position;
                R_isGrippingPlayer = true;
            }
            R_isGripping = true;
            R_handRB.constraints = RigidbodyConstraints.FreezeAll;
            R_handRB.transform.position = R_playerGrippedGameObject.transform.position + R_distanceFromHandToGrippedObject;
        }
    }
    private void OnLPlayerLetGo()
    {
        L_isGrippingPlayer = false;
        L_isGripping = false;
        L_handRB.constraints = RigidbodyConstraints.None;
    }
    private void OnRPlayerLetGo()
    {
        R_isGrippingPlayer = false;
        R_isGripping = false;
        R_handRB.constraints = RigidbodyConstraints.None;
    }
    private void Finish()
    {
        if (!hasFinished)
        {
            hasFinished = true;
            timerHandeler.isTimerRunning = false;
            Debug.Log("Best Time: " + GameManager.Instance.GetCurrentLevelBestTime());
            float timeDif = timerHandeler.timeElapsed - GameManager.Instance.GetCurrentLevelBestTime();
            GameManager.Instance.setCurrentLevelTime(timerHandeler.timeElapsed);
            Debug.Log("Time: " + timerHandeler.timeElapsed + ((timeDif > 0) ? " Time difference from best: +": " Time difference from best: ") + timeDif); 

        }
    }
    private void SetCheckPoint()
    {
        if (currentCheckpoint != potentialCheckPoint)
        {
            currentCheckpoint = potentialCheckPoint;
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
    private void CreateLeftJoint()
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

        // Spring to arm to reduce jitering when swinging

        SoftJointLimitSpring linearSpring = new SoftJointLimitSpring();
        linearSpring.spring = jointSpring;
        linearSpring.damper = jointDamper;
        L_currentJoint.linearLimitSpring = linearSpring;

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

        // Spring to arm to reduce jitering when swinging
        SoftJointLimitSpring linearSpring = new SoftJointLimitSpring();
        linearSpring.spring = jointSpring;
        linearSpring.damper = jointDamper;
        L_currentJoint.linearLimitSpring = linearSpring;

        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = armLength; // arm can stretch this far
        R_currentJoint.linearLimit = linearLimit;
    }
}
