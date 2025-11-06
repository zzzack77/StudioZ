using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickTesting : MonoBehaviour
{
    [SerializeField] private Rigidbody R_Anchor;
    [SerializeField] private Rigidbody L_Anchor;
    // LS Left Shoulder, RS Right Shoulder
    [SerializeField] private Transform LS;
    [SerializeField] private Transform RS;
    private Transform currentAnchor;

    [SerializeField] private HingeJoint2D hinge;


    private RaycastHit hit;
    private Vector3 mousePos;

    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;

    private float armDistance;
    // The arm's and legs max reach
    //[SerializeField] private float maxDistance;

    // The amount of force applied to rigidbody when moving limbs
    [SerializeField] private float variableForce;

    // For furture use change trigger deadzone so slight pressure doesn't activate grip
    // Higher deadzone means more pressure is needed
    private float triggerDeadZone = 0.01f;

    void Update()
    {
        InitializeGamePad();
        MovingLimbs();
    }
    void MovingLimbs()
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
                hit.rigidbody.AddForce(direction.normalized * variableForce);
            }
            else
            {
                hit.rigidbody.AddForce(direction.normalized * variableForce * direction.magnitude / 2);
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            currentAnchor = null;
        }
    }
    void InitializeGamePad()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.Log("No controller connected.");
            return;
        }

        // Example sticks
        Vector2 leftStick = gamepad.leftStick.ReadValue();
        Vector2 rightStick = gamepad.rightStick.ReadValue();

        // Example triggers and buttons
        float leftTrigger = gamepad.leftTrigger.ReadValue();
        float rightTrigger = gamepad.rightTrigger.ReadValue();
        float leftShoulder = gamepad.leftShoulder.ReadValue();

        if (gamepad.buttonSouth.wasPressedThisFrame) Debug.Log("A / Cross pressed");
        if (gamepad.buttonEast.wasPressedThisFrame) Debug.Log("B / Circle pressed");
        if (gamepad.buttonNorth.wasPressedThisFrame) Debug.Log("Y / Triangle pressed");
        if (gamepad.buttonWest.wasPressedThisFrame) Debug.Log("X / Square pressed");

        //Debug.Log($"LeftStick: {leftStick}, RightStick: {rightStick}, LT: {leftTrigger:F2}, RT: {rightTrigger:F2}, LS: {leftShoulder:F2}");
        
        if (rightStick != Vector2.zero)
        {
            R_Anchor.AddForce(rightStick * variableForce);
        }
        if (leftStick != Vector2.zero)
        {
            L_Anchor.AddForce(leftStick * variableForce);
        }
        if (rightTrigger > triggerDeadZone) { R_Anchor.isKinematic = true; }
        if (rightTrigger <= triggerDeadZone) { R_Anchor.isKinematic = false; }
        if (leftTrigger > triggerDeadZone) { L_Anchor.isKinematic = true; }
        if (leftTrigger <= triggerDeadZone) { L_Anchor.isKinematic = false; }
    }
}

