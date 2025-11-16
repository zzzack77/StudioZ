using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [Header("Rigidbodys")]
    [SerializeField] private Rigidbody bodyRB;
    [SerializeField] private Rigidbody L_handRB;
    [SerializeField] private Rigidbody R_handRB;

    [Header("Shoulder Points")]
    [SerializeField] private Transform L_shoulderPoint;
    [SerializeField] private Transform R_shoulderPoint;


    [Header("Arm and joint settings")]
    private ConfigurableJoint L_currentJoint;
    private ConfigurableJoint R_currentJoint;
    [SerializeField] private float armLength = 4.2f;
    [SerializeField] private float jointBreakingSensitivity = 0.99f;

    // Left Grips
    public bool L_canGripFinish { get; set; }
    public bool L_canGripCheckPoint { get; set; }
    public bool L_canGripJug { get; set; }
    public bool L_canGripCrimp { get; set; }
    public bool L_canGripPocket { get; set; }
    // Right Grips
    public bool R_canGripFinish { get; set; }
    public bool R_canGripCheckPoint { get; set; }
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

    [Header("Joystick Flicking Settings")]
    [SerializeField] private float forceMultiplier = 5; // How strong the swigning force is

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
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
            bodyRB.transform.position = Vector3.zero;
            bodyRB.linearVelocity = Vector3.zero;
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


    }
    private void LGrippedHandMovement()
    {
        if (!L_isGripping) return;

        Vector3 controllerDirection = new Vector3(leftStick.x, leftStick.y, 0);
        bodyRB.AddForce(-controllerDirection * forceMultiplier);
    }
    private void RGrippedHandMovement()
    {
        if (!R_isGripping) return;
        Vector3 controllerDirection = new Vector3(rightStick.x, rightStick.y, 0);
        bodyRB.AddForce(-controllerDirection * forceMultiplier);
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
        // Left Hand Grip Checks
        if (leftTrigger >= triggerDeadZone && leftShoulder < triggerDeadZone && L_canGripJug)
        {
            OnLGrip();
        }
        else if (leftShoulder >= triggerDeadZone && leftTrigger < triggerDeadZone && L_canGripCrimp)
        {
            OnLGrip();
        }
        else if (leftTrigger >= triggerDeadZone && leftShoulder >= triggerDeadZone && L_canGripPocket)
        {
            OnLGrip();
        }
        else
        {
            L_isGripping = false;
            L_handRB.constraints = RigidbodyConstraints.None;
        }
        // Right Hand Grip Checks
        if (rightTrigger >= triggerDeadZone && rightShoulder < triggerDeadZone && R_canGripJug)
        {
            OnRGrip();
        }
        else if (rightShoulder >= triggerDeadZone && rightTrigger < triggerDeadZone && R_canGripCrimp)
        {
            OnRGrip();
        }
        else if (rightTrigger >= triggerDeadZone && rightShoulder >= triggerDeadZone && R_canGripPocket)
        {
            OnRGrip();
        }
        else
        {
            R_isGripping = false;
            R_handRB.constraints = RigidbodyConstraints.None;
        }
    }
    private void GrippingLogic()
    {
        bool leftTriggerPressed = leftTrigger >= triggerDeadZone;
        bool leftShoulderPressed = leftShoulder >= triggerDeadZone;
        if (leftTriggerPressed)
        {
            if (L_canGripFinish)
            {
                OnLGrip();
                Finish();
            }
            else if (L_canGripCheckPoint)
            {
                OnLGrip();
                CheckPoint();
            }
            else if (L_canGripJug && !leftShoulderPressed) OnLGrip();
            else if (L_canGripPocket && leftShoulderPressed) OnLGrip();
            else
            {
                L_isGripping = false;
                L_handRB.constraints = RigidbodyConstraints.None;
            }
        }
        if (leftShoulderPressed)
        {
            if (L_canGripFinish)
            {
                OnLGrip();
                Finish();
            }
            else if (L_canGripCheckPoint)
            {
                OnLGrip();
                CheckPoint();
            }
            else if (L_canGripCrimp && !leftTriggerPressed) OnLGrip();
            else
            {
                L_isGripping = false;
                L_handRB.constraints = RigidbodyConstraints.None;
            }
        }

        bool rightTriggerPressed = rightTrigger >= triggerDeadZone;
        bool rightShoulderPressed = rightShoulder >= triggerDeadZone;
        if (rightTriggerPressed)
        {
            if (R_canGripFinish)
            {
                OnRGrip();
                Finish();
            }
            else if (R_canGripCheckPoint)
            {
                OnRGrip();
                CheckPoint();
            }
            else if (R_canGripJug && !rightShoulderPressed) OnRGrip();
            else if (R_canGripPocket && rightShoulderPressed) OnRGrip();
            else
            {
                R_isGripping = false;
                R_handRB.constraints = RigidbodyConstraints.None;
            }
        }
        if (rightShoulderPressed)
        {
            if (R_canGripFinish)
            {
                OnRGrip();
                Finish();
            }
            else if (R_canGripCheckPoint)
            {
                OnRGrip();
                CheckPoint();
            }
            else if (R_canGripCrimp && !rightTriggerPressed) OnRGrip();
            else
            {
                R_isGripping = false;
                R_handRB.constraints = RigidbodyConstraints.None;
            }
        }
    }

    private void OnLGrip()
    {
        L_isGripping = true;
        L_handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void OnRGrip()
    {
        R_isGripping = true;
        R_handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void Finish()
    {
        Debug.Log("Finish Reached!");
    }
    private void CheckPoint()
    {
        Debug.Log("Checkpoint Reached!");
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
