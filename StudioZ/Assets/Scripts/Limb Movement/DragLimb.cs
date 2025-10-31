using UnityEngine;

public class DragLimb : MonoBehaviour
{
    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;
    private RaycastHit2D hit;
    private Vector2 mousePos;
   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Gets the location of the mouse 
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider.CompareTag("Appendage") && hit.collider != null)
            {
                isDragging = true;
            }
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit.transform.position = mousePos;
        }
        else isDragging = false;
        
    }
}
