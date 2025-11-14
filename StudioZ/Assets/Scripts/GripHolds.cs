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
                //if (collider.gameObject.CompareTag("Any"))
                //{
                //    handAndBodyMovement.canGripJug = true;
                //}
                if (collider.gameObject.CompareTag("Jug"))
                {
                    handAndBodyMovement.L_canGripJug = true;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    handAndBodyMovement.L_canGripCrimp = true;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    handAndBodyMovement.L_canGripPocket = true;
                }
            }
            if (this.name == "R Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Jug"))
                {
                    handAndBodyMovement.R_canGripJug = true;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    handAndBodyMovement.R_canGripCrimp = true;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    handAndBodyMovement.R_canGripPocket = true;
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
                    handAndBodyMovement.L_canGripJug = false;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    handAndBodyMovement.L_canGripCrimp = false;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    handAndBodyMovement.L_canGripPocket = false;
                }
            }
            if (this.name == "R Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Jug"))
                {
                    handAndBodyMovement.R_canGripJug = false;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    handAndBodyMovement.R_canGripCrimp = false;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    handAndBodyMovement.R_canGripPocket = false;
                }
            }
        }
    }
}
