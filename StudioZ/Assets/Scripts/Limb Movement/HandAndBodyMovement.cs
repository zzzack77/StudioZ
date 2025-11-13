using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAndBodyMovement : MonoBehaviour
{
    [Header("Rigidbodys")]
    [SerializeField] private Rigidbody bodyRB;
    [SerializeField] private Rigidbody handRB;


    [Header("Arm and joint settings")]
    private ConfigurableJoint currentJoint;
    [SerializeField] private float armLength = 2f;             // Max distance before joint connects
    [SerializeField] private float jointSpring = 500f;         // How stiff the joint tries to stay at the target
    [SerializeField] private float jointDamper = 50f;

    [Header("Grip Settings")]
    public bool canGripJug { get; set; }
    public bool canGripCrimp { get; set; }
    [SerializeField] private bool LIsGripping = false;

    // Trigger values
    private float leftTrigger;
    private float rightTrigger;
    // Shoulder values
    private float leftShoulder;
    // Joystick values
    private Vector2 leftStick;
    private Vector2 rightStick;


    // Dead zones
    private float triggerDeadZone = 0.1f;
    private float joystickDeadZone = 0.2f;

    [Header("Joystick Flicking Settings")]
    [SerializeField] private bool hasFlicked = false;
    [SerializeField] private float forceMultiplier;  // How strong the flick force is
    [SerializeField] private float flickThreshold;  // How "fast" the stick must move to count as a flick
    [SerializeField] private float maxForce;
    // For calculating flick speed
    private Vector2 lastStick;
    private float lastTime;
    

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

        leftShoulder = gamepad.leftShoulder.ReadValue();

        leftTrigger = gamepad.leftTrigger.ReadValue();

    }
    private void TestingFlick()
    {
        if (!LIsGripping)
        {
            return;
        }
        float currentTime = Time.time;

        // Calculate how fast the stick moved (change in magnitude over time)
        float deltaMagnitude = (leftStick.magnitude - lastStick.magnitude);
        float deltaTime = currentTime - lastTime;

        // Calculate joystick speed (distance moved since last frame)
        Vector2 delta = leftStick - lastStick;
        float speed = delta.magnitude / Time.deltaTime;

        Debug.Log($"Joystick Speed: {speed:F3}");

        if (deltaTime > 0f)
        {
            float flickSpeed = deltaMagnitude / deltaTime;

            // Detect flick: rapid change and strong direction, but only if not already flicked
            if (!hasFlicked && flickSpeed > flickThreshold && leftStick.magnitude > 0.5f)
            {
                Vector3 flickDirection = new Vector3(leftStick.x, leftStick.y, 0).normalized;
                if ((flickDirection * flickSpeed * forceMultiplier).magnitude > maxForce)
                {
                    bodyRB.AddForce(-flickDirection * maxForce, ForceMode.Impulse);
                    Debug.Log((-flickDirection * maxForce).magnitude);

                }
                else
                {
                    bodyRB.AddForce(-flickDirection * flickSpeed * forceMultiplier, ForceMode.Impulse);
                    //Debug.Log((-flickDirection * flickSpeed * forceMultiplier).magnitude);
                }
                hasFlicked = true; // prevent multiple triggers
            }


            //Debug.Log(flickSpeed);
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
        if (!LIsGripping)
        {

            Vector3 L_WorldOffset = new Vector3(
                Mathf.Clamp(leftStick.x, -1, 1) * armLength,
                Mathf.Clamp(leftStick.y, -1, 1) * armLength,
                0f);
            Vector3 R_WorldOffset = new Vector3(
                Mathf.Clamp(rightStick.x, -1, 1) * armLength,
                Mathf.Clamp(rightStick.y, -1, 1) * armLength,
                0f);

            handRB.transform.position = L_WorldOffset + bodyRB.transform.position;
        }

        if (leftShoulder > 0.1f && canGripCrimp)
        {
            LIsGripping = true;
            handRB.constraints = RigidbodyConstraints.FreezeAll;
        }

        else if (leftTrigger >= triggerDeadZone  && canGripJug)
        {
            LIsGripping = true;
            handRB.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            LIsGripping = false;
            handRB.constraints = RigidbodyConstraints.None;
        }
        
    }
    private void JointChecking()
    {
        if (LIsGripping)
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
        else
        {
            if (currentJoint != null)
            {
                Destroy(currentJoint);
                currentJoint = null;
            }
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
