using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GripHolds : MonoBehaviour
{
    [SerializeField] private JoystickTesting joystickTesting;
    [SerializeField] private HandAndBodyMovement handAndBodyMovement; 
    private void OnTriggerEnter(Collider collider)
    {
        if (joystickTesting != null)
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
        

        // New system
        if (handAndBodyMovement != null)
        {
            if (this.name == "L Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Jug"))
                {
                    handAndBodyMovement.canGripJug = true;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    handAndBodyMovement.canGripCrimp = true;
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider collider)
    {
        if (joystickTesting != null)
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

        // New system
        if (handAndBodyMovement != null)
        {
            if (this.name == "L Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Jug"))
                {
                    handAndBodyMovement.canGripJug = false;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    handAndBodyMovement.canGripCrimp = false;
                }
            }
        }
    }
}
