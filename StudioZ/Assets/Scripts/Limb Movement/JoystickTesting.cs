using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class JoystickTesting : MonoBehaviour
{
    [Header("Controller Stick Positions")]
    [SerializeField] private Transform leftControllerPoint;
    [SerializeField] private Transform rightControllerPoint;
    [SerializeField] private float maxReach;

    [Header("Rigidbodies")]
    // The rigidbodies attached to the right and left hand
    [SerializeField] private Rigidbody L_AnchorRB;
    [SerializeField] private Rigidbody L_ForearmRB;
    [SerializeField] private Rigidbody L_UpperarmRB;
    [SerializeField] private Rigidbody R_AnchorRB;
    [SerializeField] private Rigidbody R_ForearmRB;
    [SerializeField] private Rigidbody R_UpperarmRB;
    [SerializeField] private Rigidbody bodyRB;

    [Header("Joints")]
    [SerializeField] private ConfigurableJoint L_ElbowCJ;
    [SerializeField] private ConfigurableJoint R_ElbowCJ;
     

    // LS Left Shoulder, RS Right Shoulder
    [SerializeField] private Transform LS;
    [SerializeField] private Transform RS;
    private Transform currentAnchor;

    // -------- Muscle Joint Variables --------
    [Header("Muscle Joint Settings")]
    [SerializeField] private float maxContratableAngle;
    [SerializeField] private float contractedAngle = 45f;
    [SerializeField] private float relaxedAngle = 0f;

    [SerializeField] private float contractionSpringForce = 30f;
    [SerializeField] private float relaxationSpringForce = 100f;
    [SerializeField] private float springDamper = 10f;
    [SerializeField] private float muscleStrength = 100f;

    public JointDrive ContractionDrive
    {
        get
        {
            return new JointDrive
            {
                positionSpring = contractionSpringForce,
                positionDamper = springDamper,
                maximumForce = muscleStrength
            };
        }
    }

    public JointDrive RelaxationDrive
    {
        get
        {
            return new JointDrive
            {
                positionSpring = relaxationSpringForce,
                positionDamper = springDamper,
                maximumForce = muscleStrength
            };
        }
    }

    // -------- Mouse Movement Variables --------

    // Stores the raycast hit data and mouse vector3 position
    private RaycastHit hit;
    private Vector3 mousePos;
    private bool isDragging = false;

    // -------- Controller Variables --------

    // Joystick values
    private Vector2 leftStick;
    private Vector2 rightStick;

    // Triggers and shoulders values
    private float leftTrigger;
    private float rightTrigger;
    private float leftShoulder;
    private float rightShoulder;

    // -------- Movement Settings --------
    [Header("Movement Settings")]
    [SerializeField] private bool leftStickIsMoving;
    [SerializeField] private bool rightStickIsMoving;
    // The amount of force applied to rigidbody when moving limbs
    [SerializeField] private float handAcceleration;
    [SerializeField] private float minimumHandAcceleration;
    [SerializeField] private float handMaxVelocity;
    [SerializeField] private float pullUpStrength;

    [Header("Ragdoll Settings")]
    // the amount of distance on the x axis from the shoulder to the hand before angular damping is applied
    [SerializeField] private float xDistanceAmountForAngularDamping;
    [SerializeField] private float controllingHandLinearDamping; // Normal linear damping for the hand
    [SerializeField] private float ragdollHandLinearDamping; // Ragdoll linear damping applied when not moving
    [SerializeField] private float controllingHandAngularDamping; // Normal angular damping for the hand
    [SerializeField] private float ragdollHandAngularDamping; // Damping applied when not gripping and not moving

    // For furture use change trigger deadzone so slight pressure doesn't activate grip
    // Higher deadzone means more pressure is needed
    private float triggerDeadZone = 0.01f;
    [Header("Debugging Variables")]
    [SerializeField] private bool grippingDisabled;
    [SerializeField] private bool leftIsGripping = false;
    [SerializeField] private bool rightIsGripping = false;

    // Bools will return true if the hand is in a grippable area
    // Used by GripHolds.cs
    public bool canLeftGrip { get; set; }
    public bool canRightGrip { get; set; }
    private void Start()
    {
        controllingHandAngularDamping = L_AnchorRB.angularDamping;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale -= 0.1f;
        }
        InitializeGamePad(); // Gets controller and its buttons
        MovingLimbsWMouse(); // Mouse logic
        
    }
    private void FixedUpdate()
    {
        ControllerMovement();
        MovementArmLogicRB(); // RB settings for arm movement while in use
        RagdollArmLogicRB(); // RB settings for arm movement while not in use
    }
    private void MovingLimbsWMouse()
    {
        // Raycast from mouse to see if the player has clicked anything
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                isDragging = true;
                // Determine which anchor to use based on name
                currentAnchor = (hit.collider.name == "LA_Anchor") ?
                    LS : (hit.collider.name == "RA_Anchor") ?
                    RS : null;
            }
        }

        // If raycast is successfull set the hit transform position to the mouse location
        if (Input.GetMouseButton(0) && isDragging)
        {
            // Get the mouse position in world space
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector3 direction = mousePos - hit.transform.position;
            if (direction.magnitude > 0.3f)
            {
                //hit.rigidbody.forc
                hit.rigidbody.AddForce(direction.normalized * handAcceleration);
            }
            else
            {
                hit.rigidbody.AddForce(direction.normalized * handAcceleration * direction.magnitude / 2);
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            currentAnchor = null;
        }
    }
    private void InitializeGamePad()
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

        // Read trigger and shoulder values
        leftTrigger = gamepad.leftTrigger.ReadValue();
        rightTrigger = gamepad.rightTrigger.ReadValue();
        leftShoulder = gamepad.leftShoulder.ReadValue();

        // Debug button presses
        if (gamepad.buttonSouth.wasPressedThisFrame) Debug.Log("A / Cross pressed");
        if (gamepad.buttonEast.wasPressedThisFrame) Debug.Log("B / Circle pressed");
        if (gamepad.buttonNorth.wasPressedThisFrame) Debug.Log("Y / Triangle pressed");
        if (gamepad.buttonWest.wasPressedThisFrame) Debug.Log("X / Square pressed");

        //Debug.Log($"LeftStick: {leftStick}, RightStick: {rightStick}, LT: {leftTrigger:F2}, RT: {rightTrigger:F2}, LS: {leftShoulder:F2}");

        if (!grippingDisabled)
        {
            // Grip logic for left and right hands
            if (leftTrigger > triggerDeadZone && canLeftGrip) { L_AnchorRB.isKinematic = true; leftIsGripping = true; }
            if (leftTrigger <= triggerDeadZone) { L_AnchorRB.isKinematic = false; leftIsGripping = false; }
            if (rightTrigger > triggerDeadZone && canRightGrip) { R_AnchorRB.isKinematic = true; rightIsGripping = true; }
            if (rightTrigger <= triggerDeadZone) { R_AnchorRB.isKinematic = false; rightIsGripping = false; }
        }
        if (leftStick != Vector2.zero) leftStickIsMoving = true;
        else leftStickIsMoving = false;
        if (rightStick != Vector2.zero) rightStickIsMoving = true;
        else rightStickIsMoving = false;
    }
    
    private void ControllerMovement()
    {
        leftControllerPoint.transform.localPosition = new Vector3(
            Mathf.Clamp(leftStick.x, -1, 1) * maxReach,
            Mathf.Clamp(leftStick.y, -1, 1) * maxReach,
            leftControllerPoint.transform.localPosition.z);

        rightControllerPoint.transform.localPosition = new Vector3(
            Mathf.Clamp(rightStick.x, -1, 1) * maxReach,
            Mathf.Clamp(rightStick.y, -1, 1) * maxReach,
            rightControllerPoint.transform.localPosition.z);

        if (leftStickIsMoving)
        {
            if (leftIsGripping)
            {
                Vector2 distanceFromBodyToHand = new Vector2(
                    L_AnchorRB.position.x - bodyRB.position.x,
                    L_AnchorRB.position.y - bodyRB.position.y);

                Debug.Log(leftStick.y);
                
                //bodyRB.AddForce(distanceFromBodyToHand.normalized * pullUpStrength);
                contractedAngle = Mathf.Lerp(0, maxContratableAngle, leftStick.y);
                ContractMuscle(L_ElbowCJ);
            }
            else
            {
                RelaxMuscle(L_ElbowCJ);


                Vector3 force = ((leftControllerPoint.position - L_AnchorRB.transform.position) * handAcceleration);
                if (force.magnitude > minimumHandAcceleration) L_AnchorRB.AddForce(force);
                else L_AnchorRB.AddForce((leftControllerPoint.position - L_AnchorRB.transform.position).normalized * minimumHandAcceleration);

                if (L_AnchorRB.linearVelocity.magnitude > handMaxVelocity)
                {
                    L_AnchorRB.linearVelocity = L_AnchorRB.linearVelocity.normalized * handMaxVelocity;
                }
            } 
        }
        if (rightStickIsMoving)
        {
            Vector3 force = ((rightControllerPoint.position - R_AnchorRB.transform.position) * handAcceleration);
            if (force.magnitude > minimumHandAcceleration) R_AnchorRB.AddForce(force);
            else R_AnchorRB.AddForce((rightControllerPoint.position - R_AnchorRB.transform.position).normalized * minimumHandAcceleration);

            if (R_AnchorRB.linearVelocity.magnitude > handMaxVelocity)
            {
                R_AnchorRB.linearVelocity = R_AnchorRB.linearVelocity.normalized * handMaxVelocity;
            }
        }
    }
    // Applies rigidbody logic to arms when in use
    private void MovementArmLogicRB()
    {
        if (leftStickIsMoving)
        {
            L_AnchorRB.linearDamping = controllingHandLinearDamping;
            L_AnchorRB.angularDamping = controllingHandAngularDamping;
        }
        if (rightStickIsMoving)
        {
            R_AnchorRB.linearDamping = controllingHandLinearDamping;
            R_AnchorRB.angularDamping = controllingHandAngularDamping;
        }
    }
    // Applies ragdoll logic to arms when not in use
    private void RagdollArmLogicRB()
    {
        if (!leftIsGripping && !leftStickIsMoving)
        {
            L_AnchorRB.linearDamping = ragdollHandLinearDamping;

            if (L_AnchorRB.transform.position.x < L_UpperarmRB.transform.position.x + xDistanceAmountForAngularDamping &&
                L_AnchorRB.transform.position.x > L_UpperarmRB.transform.position.x - xDistanceAmountForAngularDamping &&
                L_AnchorRB.transform.position.y < L_ForearmRB.transform.position.y)
            {
                L_AnchorRB.angularDamping = ragdollHandAngularDamping;
            }
        }
        if (!rightIsGripping && !rightStickIsMoving)
        {
            R_AnchorRB.linearDamping = ragdollHandLinearDamping;

            if (R_AnchorRB.transform.position.x < R_UpperarmRB.transform.position.x + xDistanceAmountForAngularDamping &&
                R_AnchorRB.transform.position.x > R_UpperarmRB.transform.position.x - xDistanceAmountForAngularDamping &&
                R_AnchorRB.transform.position.y < R_ForearmRB.transform.position.y)
            {
                R_AnchorRB.angularDamping = ragdollHandAngularDamping;
            }
        }
    }
    // Unused at the moment but the idea is to have muscle contraction and relaxation functions
    private void ContractMuscle(ConfigurableJoint joint)
    {
        joint.angularXDrive = ContractionDrive;
        joint.targetRotation = Quaternion.Euler(contractedAngle, 0, 0);
    }

    private void RelaxMuscle(ConfigurableJoint joint)
    {
        joint.angularYZDrive = RelaxationDrive;
        joint.targetRotation = Quaternion.Euler(0, 0, 0);
    }
}

