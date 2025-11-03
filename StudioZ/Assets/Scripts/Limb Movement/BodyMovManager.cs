using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UIElements;

public class DragLimbV2 : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform target;
    [SerializeField] private HingeJoint2D hinge;
    private RaycastHit2D hit;
    private Vector2 mousePos;

    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;

    // The arm's max reach
    [SerializeField] private float maxDistance;

    public float distance;

    // Update is called once per frame
    void Update()
    {
        MovingLimbs();
    }
    private void FixedUpdate()
    {
        EnableHingeJoint();
    }
    void MovingLimbs()
    {
        // Raycast from mouse to see if the player has clicked anything
        if (Input.GetMouseButtonDown(0))
        {
            // Gets the location of the mouse 
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Send out a raycast from the mouse position to below it
            hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Limb"))
            {
                isDragging = true;
                hinge.enabled = false;
            }
        }
        // If raycast is successfull set the hit transform position to the mouse location
        if (Input.GetMouseButton(0) && isDragging)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit.transform.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
        }
    }
    void EnableHingeJoint()
    {
        if (!hinge.enabled)
        {
            distance = Vector2.Distance(root.position, target.position);
            if (distance >= maxDistance && target.position.y < root.position.y && !isDragging)
            {
                if (!hinge.enabled) hinge.enabled = true;
            }
        }


        
        //if (target.position.y < root.position.y && !isDragging && !hinge.enabled) hinge.enabled = true;

        //if (!isDragging && Vector3.Distance(root.position, target.position) <= maxDistance)
        //{
            
        //    //hinge.enabled = true;
        //    // Calculates the direction and distance using the start of the arm and the end
        //    //Vector3 direction = transform.position - limbStartPoint.position;
        //    //float distance = direction.magnitude;

        //    //Vector3 direction = target.position - root.position;
        //    //Debug.Log(direction);
        //    //float distance = direction.magnitude;
        //    //if (distance < maxDistance)
        //    //{
        //    //    //transform.position = limbStartPoint.position + direction.normalized * maxDistance;

        //    //    target.position = root.position + direction.normalized * maxDistance;
        //    //}
        //}
    }
}
