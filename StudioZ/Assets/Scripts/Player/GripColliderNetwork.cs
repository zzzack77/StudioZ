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
    private void IsBreaker(Collider collider, bool isLeft, bool isGripping)
    {
        HoldBreaker holdBreaker = collider.gameObject.GetComponent<HoldBreaker>();
        if (holdBreaker != null)
        {
            holdBreaker.GetPlayerReference(networkPlayerMovement);
            if (isGripping)
            {
                if (isLeft) holdBreaker.OnLCollision();
                else holdBreaker.OnRCollision();
            }

            else
            {
                Debug.Log("Hold Breaker is null!");
                if (isLeft) holdBreaker.EndLGrip();
                else holdBreaker.EndRGrip();
            }
        }
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
                    IsBreaker(collider, true, true);
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    networkPlayerMovement.L_canGripCrimp = true;
                    IsBreaker(collider, true, true);
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    networkPlayerMovement.L_canGripPocket = true;
                    IsBreaker(collider, true, true);
                }

                if (collider.gameObject.CompareTag("Weapon"))
                {
                    networkPlayerMovement.L_canGripWeapon = true;
                    networkPlayerMovement.L_GripWeaponGameObject = collider.gameObject;
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
                    IsBreaker(collider, false, true);
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    networkPlayerMovement.R_canGripCrimp = true;
                    IsBreaker(collider, false, true);
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    networkPlayerMovement.R_canGripPocket = true;
                    IsBreaker(collider, false, true);
                }
                if (collider.gameObject.CompareTag("Weapon"))
                {
                    networkPlayerMovement.R_canGripWeapon = true;
                    networkPlayerMovement.R_GripWeaponGameObject = collider.gameObject;
                    Debug.unityLogger.Log("Right hand: " +collider.gameObject.name);
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
                    IsBreaker(collider, true, false);
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    IsBreaker(collider, true, false);
                    networkPlayerMovement.L_canGripCrimp = false;
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    IsBreaker(collider, true, false);
                    networkPlayerMovement.L_canGripPocket = false;
                }
                if (collider.gameObject.CompareTag("Weapon"))
                {
                    networkPlayerMovement.L_canGripWeapon = false;
                    networkPlayerMovement.L_GripWeaponGameObject = null;
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
                    IsBreaker(collider, false, false);
                }
                if (collider.gameObject.CompareTag("Crimp"))
                {
                    networkPlayerMovement.R_canGripCrimp = false;
                    IsBreaker(collider, false, false);
                }
                if (collider.gameObject.CompareTag("Pocket"))
                {
                    networkPlayerMovement.R_canGripPocket = false;
                    IsBreaker(collider, false, false);
                }
                if (collider.gameObject.CompareTag("Weapon"))
                {
                    networkPlayerMovement.R_canGripWeapon = false;
                    networkPlayerMovement.R_GripWeaponGameObject = null;
                }
            }
        }
    }
}
