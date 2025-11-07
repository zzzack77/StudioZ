using Unity.VisualScripting;
using UnityEngine;

public class GripHolds : MonoBehaviour
{
    [SerializeField] private JoystickTesting joystickTesting;

    private void OnTriggerEnter(Collider collider)
    {
        if (this.name == "LA_Anchor")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canLeftGrip = true;
            }
        }
        if (this.name == "RA_Anchor")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canRightGrip = true;
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (this.name == "LA_Anchor")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canLeftGrip = false;
            }
        }
        if (this.name == "RA_Anchor")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canRightGrip = false;
            }
        }
    }
}
