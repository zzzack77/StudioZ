using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAndBodyMovement : MonoBehaviour
{
    [Header("Rigid Bodys")]
    public Rigidbody bodyRB;
    public Rigidbody handRB;

    private ConfigurableJoint currentJoint;

    [Header("Arm Settings")]
    public float armLength = 2f;             // Max distance before joint connects
    public float jointSpring = 500f;         // How stiff the joint tries to stay at the target
    public float jointDamper = 50f;

    [Header("Controller Stick Positions")]
    [SerializeField] private Transform leftControllerPoint;
    [SerializeField] private Transform rightControllerPoint;
    [SerializeField] private Transform bodyPoint;
    [SerializeField] private float maxReach;

    private float leftTrigger;
    // Joystick values
    private Vector2 leftStick;
    private Vector2 rightStick;

    // Dead zones
    private float triggerDeadZone = 0.1f;
    public float joystickDeadZone = 0.2f;

    [SerializeField] private bool hasFlicked = false;

    public Rigidbody rb;
    public float forceMultiplier = 10f;  // How strong the flick force is
    public float flickThreshold = 0.7f;  // How "fast" the stick must move to count as a flick

    private Vector2 lastStick;
    private float lastTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        JointChecking();
        InitializeGamepad();
        TestingFlick();
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
    }
    private void TestingFlick()
    {
        float currentTime = Time.time;

        // Calculate how fast the stick moved (change in magnitude over time)
        float deltaMagnitude = (leftStick.magnitude - lastStick.magnitude);
        float deltaTime = currentTime - lastTime;

        if (deltaTime > 0f)
        {
            float flickSpeed = deltaMagnitude / deltaTime;

            // Detect flick: rapid change and strong direction, but only if not already flicked
            if (!hasFlicked && flickSpeed > flickThreshold && leftStick.magnitude > 0.5f)
            {
                Vector3 flickDirection = new Vector3(leftStick.x, leftStick.y, 0).normalized;
                rb.AddForce(flickDirection * flickSpeed * forceMultiplier, ForceMode.Impulse);
                Debug.Log((flickDirection * flickSpeed * forceMultiplier).magnitude);

                hasFlicked = true; // prevent multiple triggers
            }

            // Reset flick once the joystick returns near center
            if (hasFlicked && leftStick.magnitude < joystickDeadZone)
            {
                hasFlicked = false;
            }
        }

        // Update for next frame
        lastStick = leftStick;
        lastTime = currentTime;
    }
    private void ControllerMovement()
    {
        if (leftTrigger < triggerDeadZone)
        {
            Vector3 L_WorldOffset = new Vector3(
                Mathf.Clamp(leftStick.x, -1, 1) * maxReach,
                Mathf.Clamp(leftStick.y, -1, 1) * maxReach,
                0f);
            Vector3 R_WorldOffset = new Vector3(
                Mathf.Clamp(rightStick.x, -1, 1) * maxReach,
                Mathf.Clamp(rightStick.y, -1, 1) * maxReach,
                0f);

            leftControllerPoint.transform.position = L_WorldOffset + bodyPoint.transform.position;
        }

        if (leftTrigger > triggerDeadZone)
        {
            // Calculate hand target positions based on joystick input
            Vector3 L_WorldOffset = new Vector3(
                -Mathf.Clamp(leftStick.x, -1, 1) * maxReach,
                -Mathf.Clamp(leftStick.y, -1, 1) * maxReach,
                0f);
            Vector3 R_WorldOffset = new Vector3(
                Mathf.Clamp(rightStick.x, -1, 1) * maxReach,
                Mathf.Clamp(rightStick.y, -1, 1) * maxReach,
                0f);

            //bodyPoint.transform.position = leftControllerPoint.InverseTransformDirection(L_WorldOffset);
            bodyPoint.transform.position = L_WorldOffset + leftControllerPoint.transform.position;
        }
        else
        {
            // Calculate hand target positions based on joystick input
            Vector3 L_WorldOffset = new Vector3(
                Mathf.Clamp(leftStick.x, -1, 1) * maxReach,
                Mathf.Clamp(leftStick.y, -1, 1) * maxReach,
                0f);
            Vector3 R_WorldOffset = new Vector3(
                Mathf.Clamp(rightStick.x, -1, 1) * maxReach,
                Mathf.Clamp(rightStick.y, -1, 1) * maxReach,
                0f);

            leftControllerPoint.transform.position = L_WorldOffset + bodyPoint.transform.position;
            //rightControllerPoint.localPosition = rightControllerPoint.parent.InverseTransformDirection(R_WorldOffset);
        }


    }
    private void JointChecking()
    {
        float distance = Vector3.Distance(bodyRB.position, handRB.position);

        // When hand is beyond arm length and no joint exists create joint
        if (currentJoint == null && distance >= armLength)
        {
            CreateJoint();
        }

        // When hand comes back within range remove joint
        if (currentJoint != null && distance < armLength * 0.9f)
        {
            Destroy(currentJoint);
            currentJoint = null;
        }
    }
    void CreateJoint()
    {
        currentJoint = bodyRB.gameObject.AddComponent<ConfigurableJoint>();
        currentJoint.connectedBody = handRB;

        // Prevent Unity from auto adjusting anchor positions
        currentJoint.autoConfigureConnectedAnchor = false;
        currentJoint.anchor = Vector3.zero;
        currentJoint.connectedAnchor = Vector3.zero;

        // Limit motion to simulate a rope/arm constraint
        currentJoint.xMotion = ConfigurableJointMotion.Limited;
        currentJoint.yMotion = ConfigurableJointMotion.Limited;
        currentJoint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = armLength; // arm can stretch this far
        currentJoint.linearLimit = linearLimit;
    }
}
