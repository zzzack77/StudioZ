using Unity.Netcode;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GripColliderNetwork : NetworkBehaviour
{
    [SerializeField] private NetworkPlayerMovement networkPlayerMovement;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }
    private void OnTriggerEnter(Collider collider)
    {
        // New system
        if (networkPlayerMovement != null)
        {
            if (this.name == "L Joystick Pos")
            {
                if (collider.gameObject.CompareTag("Finish"))
                {
                    networkPlayerMovement.L_canGripFinish = true;
                }
                if (collider.gameObject.CompareTag("Checkpoint"))
                {
                    networkPlayerMovement.L_canGripCheckpoint = true;
                    networkPlayerMovement.PotentialCheckpoint = collider.transform.position;
                }
                if (collider.gameObject.CompareTag("Player"))
                {
                    networkPlayerMovement.L_canGripPlayer = true;
                    networkPlayerMovement.L_playerGrippedGameObject = collider.gameObject;
                }
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
                if (collider.gameObject.CompareTag("Finish"))
                {
                    networkPlayerMovement.R_canGripFinish = true;
                }
                if (collider.gameObject.CompareTag("Checkpoint"))
                {
                    networkPlayerMovement.R_canGripCheckpoint = true;
                    networkPlayerMovement.PotentialCheckpoint = collider.transform.position;
                }
                if (collider.gameObject.CompareTag("Player"))
                {
                    networkPlayerMovement.R_canGripPlayer = true;
                    networkPlayerMovement.R_playerGrippedGameObject = collider.gameObject;
                }
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
                if (collider.gameObject.CompareTag("Finish"))
                {
                    networkPlayerMovement.L_canGripFinish = false;
                }
                if (collider.gameObject.CompareTag("Checkpoint"))
                {
                    networkPlayerMovement.L_canGripCheckpoint = false;
                }
                if (collider.gameObject.CompareTag("Player"))
                {
                    networkPlayerMovement.L_canGripPlayer = false;
                    networkPlayerMovement.L_playerGrippedGameObject = null;
                }
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
                if (collider.gameObject.CompareTag("Finish"))
                {
                    networkPlayerMovement.R_canGripFinish = false;
                }
                if (collider.gameObject.CompareTag("Checkpoint"))
                {
                    networkPlayerMovement.R_canGripCheckpoint = false;
                }
                if (collider.gameObject.CompareTag("Player"))
                {
                    networkPlayerMovement.R_canGripPlayer = false;
                    networkPlayerMovement.R_playerGrippedGameObject = null;
                }
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
