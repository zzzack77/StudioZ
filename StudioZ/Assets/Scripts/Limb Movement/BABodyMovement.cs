using UnityEngine;

public class BABodyMovement : MonoBehaviour
{
    // LS Left Shoulder, RS Right Shoulder
    // LH Left Hip, RH Right Hip
    [SerializeField] private Transform LS;
    [SerializeField] private Transform RS;
    //[SerializeField] private Transform LH;
    //[SerializeField] private Transform RH;
    private Transform currentAnchor;

    private RaycastHit2D hit;
    private Vector3 mousePos;

    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;

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
            // Gets the location of the mouse on screen and raycasts from it
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Limb"))
            {
                isDragging = true;

                // Determine which anchor to use based on limb name
                switch (hit.collider.name)
                {
                    case "LA_Anchor":
                        currentAnchor = LS;
                        break;
                    case "RA_Anchor":
                        currentAnchor = RS;
                        break;
                    //case "LL_Anchor":
                    //    currentAnchor = LH;
                    //    break;
                    //case "RL_Anchor":
                    //    currentAnchor = RH;
                    //    break;
                    default:
                        currentAnchor = null;
                        break;
                }
                Debug.Log(currentAnchor.ToString());
            }
        }

        // If raycast is successfull set the hit transform position to the mouse location
        if (Input.GetMouseButton(0) && isDragging)
        {
            // Get the mouse position in world space
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // Calculate the direction and distance from anchor to mouse
            Vector3 direction = mousePos - currentAnchor.position;
            float distance = direction.magnitude;

            // Clamp distance to maxDistance
            if (distance > maxDistance)
            {
                direction = direction.normalized * maxDistance;
                hit.transform.position = currentAnchor.position + direction;
                hit.rigidbody.AddForce(direction * variableForce);
            }
            else hit.transform.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            currentAnchor = null;
        }
    }
}
