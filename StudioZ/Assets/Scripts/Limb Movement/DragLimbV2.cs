using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UIElements;

public class BodyMovManager : MonoBehaviour
{
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
    [SerializeField] private float maxDistance;

    [SerializeField] private float variableForce;

    void Update()
    {
        MovingLimbs();
    }
    private void FixedUpdate()
    {
        //EnableHingeJoint();
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
                    RS :null;
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
                hit.rigidbody.AddForce(direction.normalized * variableForce );
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
}
