using Unity.Netcode;
using UnityEngine;

public class MultiplayerDeathBox : NetworkBehaviour
{
    GameObject playerGO;
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Initial Collision");
        if (!IsServer) return;
        playerGO = collision.gameObject.transform.parent.gameObject;

        if (playerGO.TryGetComponent<NetworkObject>(out NetworkObject player))
        {
            KillPlayerClientRpc(player);
            Debug.Log("Has collided with: " +  player.gameObject);
        }
    }

    [ClientRpc]
    private void KillPlayerClientRpc(NetworkObjectReference playerRef)
    {
        if (playerRef.TryGet(out NetworkObject playerNetObject))
        {
            KillPlayer(playerNetObject.gameObject);
        }
        
    }

   
    private void KillPlayer(GameObject playerGORef)
    {
        PlayerAliveState.OnPlayerDead?.Invoke(playerGORef);
        Debug.Log("Kill Player");
    }
}
