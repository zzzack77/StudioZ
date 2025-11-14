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
    [SerializeField] private float armLength = 2f;             // Max distance before joint connects
    [SerializeField] private float jointSpring = 500f;         // How stiff the joint tries to stay at the target
    [SerializeField] private float jointDamper = 50f;

    [Header("Grip Settings")]
    public bool canGripAny { get; set; }
    public bool canGripCheckPoint { get; set; }
    public bool canGripFinish { get; set; }

    // Left Grips
    public bool L_canGripJug { get; set; }
    public bool L_canGripCrimp { get; set; }
    public bool L_canGripPocket { get; set; }
    // Right Grips
    public bool R_canGripJug;
    public bool R_canGripCrimp { get; set; }
    public bool R_canGripPocket { get; set; }

    [SerializeField] private bool L_isGripping = false;
    [SerializeField] private bool R_isGripping = false;

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
    [SerializeField] private bool L_hasFlicked = false;
    [SerializeField] private bool R_hasFlicked = false;
    [SerializeField] private float flickCooldown = 0.2f;
    [SerializeField] private float forceMultiplier; // How strong the swigning force is
    [SerializeField] private float flickMultiplier; // How strong the flick force is
    [SerializeField] private float flickThreshold;  // How "fast" the stick must move to count as a flick
    [SerializeField] private float maxForce;
    // For calculating flick speed
    private Vector2 L_lastStick;
    private float L_lastTime;
    private float L_lastFlickTime = -1f;

    private Vector2 R_lastStick;
    private float R_lastTime;
    private float R_lastFlickTime = -1f;


    // Update is called once per frame
    void Update()
    {
        JointChecking();
        InitializeGamepad();
        LGrippedHandMovement();
        RGrippedHandMovement();
    }
    private void FixedUpdate()
    {
        ControllerMovement();
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

        Vector3 controllerDirection = new Vector3(leftStick.x, leftStick.y, 0).normalized;

        //float forceApplied = Mathf.Min((- controllerDirection.normalized * forceMultiplier).magnitude, maxForce);
        bodyRB.AddForce(-controllerDirection * forceMultiplier);


        float currentTime = Time.time;

        // Calculate flick speed
        float deltaMagnitude = (leftStick.magnitude - L_lastStick.magnitude);
        float deltaTime = currentTime - L_lastTime;
        Vector2 delta = leftStick - L_lastStick;
        float speed = delta.magnitude / Time.deltaTime;

        // Only proceed if time has passed
        if (deltaTime > 0f)
        {
            float flickSpeed = deltaMagnitude / deltaTime;
            bool canFlick = !L_hasFlicked && (currentTime - L_lastFlickTime) > flickCooldown;
            // Check if flick conditions are met
            if (canFlick && flickSpeed > flickThreshold && leftStick.magnitude > 0.5f)
            {
                Vector3 flickDirection = new Vector3(leftStick.x, leftStick.y, 0).normalized;
                float flickForce = Mathf.Min(flickSpeed * flickMultiplier, maxForce);

                bodyRB.AddForce(-flickDirection * flickForce, ForceMode.Impulse);

                L_hasFlicked = true;
                L_lastFlickTime = currentTime; // start cooldown
            }
            if (L_hasFlicked && (currentTime - L_lastFlickTime) > flickCooldown)
            {
                L_hasFlicked = false;
            }
        }

        // Update for next frame
        L_lastStick = leftStick;
        L_lastTime = currentTime;
    }
    private void RGrippedHandMovement()
    {
        if (!R_isGripping) return;

        Vector3 controllerDirection = new Vector3(rightStick.x, rightStick.y, 0).normalized;
        bodyRB.AddForce(-controllerDirection * forceMultiplier);


        float currentTime = Time.time;

        // Calculate flick speed
        float deltaMagnitude = (rightStick.magnitude - R_lastStick.magnitude);
        float deltaTime = currentTime - R_lastTime;
        Vector2 delta = rightStick - R_lastStick;
        float speed = delta.magnitude / Time.deltaTime;

        // Only proceed if time has passed
        if (deltaTime > 0f)
        {
            float flickSpeed = deltaMagnitude / deltaTime;
            bool canFlick = !R_hasFlicked && (currentTime - R_lastFlickTime) > flickCooldown;
            // Check if flick conditions are met
            if (canFlick && flickSpeed > flickThreshold && rightStick.magnitude > 0.5f)
            {
                Vector3 flickDirection = new Vector3(rightStick.x, rightStick.y, 0).normalized;
                float flickForce = Mathf.Min(flickSpeed * flickMultiplier, maxForce);

                bodyRB.AddForce(-flickDirection * flickForce, ForceMode.Impulse);

                R_hasFlicked = true;
                R_lastFlickTime = currentTime; // start cooldown
            }
            if (R_hasFlicked && (currentTime - R_lastFlickTime) > flickCooldown)
            {
                R_hasFlicked = false;
            }
        }

        // Update for next frame
        R_lastStick = leftStick;
        R_lastTime = currentTime;
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
            L_hasFlicked = false;
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
            R_hasFlicked = false;
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
            if (L_currentJoint != null && distance < armLength * 0.9f)
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
            if (R_currentJoint != null && distance < armLength * 0.9f)
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
        L_currentJoint.anchor = Vector3.zero;
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
        R_currentJoint.anchor = Vector3.zero;
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
