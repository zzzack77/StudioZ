using UnityEngine;
using UnityEngine.U2D.IK;

public class DragLimb : MonoBehaviour
{
    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;
    private RaycastHit2D hit;
    private Vector2 mousePos;


    private LimbComponentHolder limbHolder;
    private Rigidbody2D endLimbRB;

    private HingeJoint2D upperLimbHJ;
    private HingeJoint2D lowerLimbHJ;
    private HingeJoint2D endLimbHJ;

    LimbSolver2D limbSolver;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Gets the location of the mouse 
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Send out a raycast from the mouse position to below it
            hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Limb"))
            {
                isDragging = true;

                // Getting the limbs rigid bodies and hinge joints to be able to dynamically turn them on and off
                limbHolder = hit.collider.gameObject.GetComponent<LimbComponentHolder>();
                //if (limbHolder == null) return;
                endLimbRB = limbHolder.endLimb.GetComponent<Rigidbody2D>();

                upperLimbHJ = limbHolder.upperLimb.GetComponent<HingeJoint2D>();
                lowerLimbHJ = limbHolder.lowerLimb.GetComponent<HingeJoint2D>();
                endLimbHJ = limbHolder.endLimb.GetComponent<HingeJoint2D>();

                limbSolver = limbHolder.limbSolver.GetComponent<LimbSolver2D>();

                endLimbRB.gravityScale = 0;
                upperLimbHJ.enabled = false;
                lowerLimbHJ.enabled = false;
                endLimbHJ.enabled = false;

                limbSolver.enabled = true;

            }
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit.transform.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging) 
        {
            isDragging = false;
            endLimbRB.gravityScale = 1;
            upperLimbHJ.enabled = true;
            lowerLimbHJ.enabled = true;
            endLimbHJ.enabled = true;

            limbSolver.enabled = false;
        }
        
        
    }
}
