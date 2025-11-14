using Unity.Netcode;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GripColliderNetwork : NetworkBehaviour
{
    [SerializeField] private NetworkPlayerMovement networkPlayerMovement;
    private void OnTriggerEnter(Collider collider)
    {
        // New system
        if (networkPlayerMovement != null)
        {
            if (this.name == "L Joystick Pos")
            {
                //if (collider.gameObject.CompareTag("Any"))
                //{
                //    handAndBodyMovement.canGripJug = true;
                //}
                if (collider.gameObject.CompareTag("Jug"))
                {
                    networkPlayerMovement.L_canGripJug = true;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    networkPlayerMovement.L_canGripCrimp = true;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    networkPlayerMovement.L_canGripPocket = true;
                }
            }
            if (this.name == "R Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Jug"))
                {
                    networkPlayerMovement.R_canGripJug = true;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    networkPlayerMovement.R_canGripCrimp = true;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    networkPlayerMovement.R_canGripPocket = true;
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider collider)
    {
        // New system
        if (networkPlayerMovement != null)
        {
            if (this.name == "L Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Jug"))
                {
                    networkPlayerMovement.L_canGripJug = false;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    networkPlayerMovement.L_canGripCrimp = false;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    networkPlayerMovement.L_canGripPocket = false;
                }
            }
            if (this.name == "R Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Jug"))
                {
                    networkPlayerMovement.R_canGripJug = false;
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    networkPlayerMovement.R_canGripCrimp = false;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    networkPlayerMovement.R_canGripPocket = false;
                }
            }
        }
    }
}
