using UnityEngine;
using UnityEngine.InputSystem;

public class PCInputTesting : MonoBehaviour
{
    // Create a reference to the player input interface
    private IPlayerInput input;
    

    [SerializeField] private Rigidbody R_Anchor;
    [SerializeField] private Rigidbody L_Anchor;
    // LS Left Shoulder, RS Right Shoulder
    [SerializeField] private Transform LS;
    [SerializeField] private Transform RS;
    private Transform currentAnchor;

    [SerializeField] private HingeJoint2D hinge;


    

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

    
    private void Awake()
    {
        // Get the player input interface
        // This interface's vales are updated through the PlayerInputProvider script
        // Ensure the PlayerInputProvider script is on a game object (preferably the thing thats moving)
        input = GetComponent<IPlayerInput>();

        if (input == null)
        {
            Debug.LogError(
                $"{nameof(PCInputTesting)} requires a component that implements IPlayerInput.\n" +
                $"Add PlayerInputProvider to the same GameObject.",
                this
            );
        }

    }
    void Update()
    {
        ReadControls();
    }
    
    void ReadControls()
    {
        if (input.MoveR != Vector2.zero)
        {
            R_Anchor.AddForce(input.MoveR * variableForce);
        }
        if (input.MoveL != Vector2.zero)
        {
            L_Anchor.AddForce(input.MoveL * variableForce);
        }
        if (input.GripRPressed()) { R_Anchor.isKinematic = true; }
        if (!input.GripRPressed()) { R_Anchor.isKinematic = false; }
        if (input.GripLPressed()) { L_Anchor.isKinematic = true; }
        if (!input.GripLPressed()) { L_Anchor.isKinematic = false; }

        if (input.ButtonSouthPressed()) Debug.Log("A was pressed");
        if (input.ButtonEastPressed()) Debug.Log("B was pressed");
        if (input.ButtonNorthPressed()) Debug.Log("Y was pressed");
        if (input.ButtonWestPressed()) Debug.Log("X was pressed");

        if (input.CrimpLPressed()) Debug.Log("Left Shoulder was pressed");
        if (input.CrimpRPressed()) Debug.Log("Right Shoulder was pressed");
    }
}

