using Unity.VisualScripting;
using UnityEngine;

public class GripHolds : MonoBehaviour
{
    [SerializeField] private JoystickTesting joystickTesting;
    private void OnTriggerEnter(Collider collider)
    {
        if (this.name == "L_Hand")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canLeftGrip = true;
            }
        }
        if (this.name == "R_Hand")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canRightGrip = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (this.name == "L_Hand")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canLeftGrip = false;
            }
        }
        if (this.name == "R_Hand")
        {
            if (collider.gameObject.CompareTag("Jug"))
            {
                joystickTesting.canRightGrip = false;
            }
        }
    }
}
