using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickTesting : MonoBehaviour
{
    [Header("Rigidbodies")]
    // The rigidbodies attached to the right and left hand
    [SerializeField] private Rigidbody L_AnchorRB;
    [SerializeField] private Rigidbody R_AnchorRB;
    [SerializeField] private Rigidbody bodyRB;

    [Header("Joints")]
    

    // LS Left Shoulder, RS Right Shoulder
    [SerializeField] private Transform LS;
    [SerializeField] private Transform RS;
    private Transform currentAnchor;


    // -------- Muscle Joint Variables --------
    [Header("Muscle Joint Settings")]
    [SerializeField] private ConfigurableJoint L_ElbowCJ;
    [SerializeField] private ConfigurableJoint R_ElbowCJ;
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

    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;

    // -------- Controller Movement Variables --------

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
    // The amount of force applied to rigidbody when moving limbs
    [SerializeField] private float handAcceleration;
    [SerializeField] private float handMaxVelocity;
    [SerializeField] private float pullUpStrength;
    [SerializeField] private bool grippingDisabled;

    // For furture use change trigger deadzone so slight pressure doesn't activate grip
    // Higher deadzone means more pressure is needed
    private float triggerDeadZone = 0.01f;
    [Header("Debugging Variables")]
    [SerializeField] private bool leftIsGripping = false;
    [SerializeField] private bool rightIsGripping = false;

    // Bools will return true if the hand is in a grippable area
    // Used by GripHolds.cs
    public bool canLeftGrip { get; set; }
    public bool canRightGrip { get; set; }
    void Update()
    {
        InitializeGamePad();
        MovingLimbsWMouse();
    }
    private void FixedUpdate()
    {
        ControllerMovement();
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
    }
    
    private void ControllerMovement()
    {
        if (leftStick != Vector2.zero)
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
                L_AnchorRB.AddForce(leftStick * handAcceleration);

                Debug.Log(L_AnchorRB.linearVelocity.magnitude);
                if (L_AnchorRB.linearVelocity.magnitude > handMaxVelocity)
                {
                    L_AnchorRB.linearVelocity = L_AnchorRB.linearVelocity.normalized * handMaxVelocity;
                }
            } 
        }
        if (rightStick != Vector2.zero)
        {
            R_AnchorRB.AddForce(rightStick * handAcceleration);
            if (R_AnchorRB.linearVelocity.magnitude > handMaxVelocity)
            {
                R_AnchorRB.linearVelocity = R_AnchorRB.linearVelocity.normalized * handMaxVelocity;
            }
        }
    }
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

