using Unity.Netcode;
using UnityEngine;

public class MultiplayerDeathBox : NetworkBehaviour
{
    GameObject playerGO;
    private void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) return;
        playerGO = collision.gameObject.transform.parent.gameObject;

        if (collision.gameObject.transform.parent.gameObject.TryGetComponent<NetworkObject>(out NetworkObject player))
        {
            KillPlayerClientRpc(player);
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
    }
}
