using Unity.Netcode;
using UnityEngine;

public class MultiplayerDeathBox : NetworkBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) return;
        GameObject player = collision.gameObject.transform.parent.gameObject;

        if (player != null)
        {
            ServerKillPlayer(player);
        }
    }

    [ClientRpc]
    private void ClientKillPlayer(GameObject player)
    {
        PlayerAliveState.OnPlayerDead?.Invoke(player);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerKillPlayer(GameObject player)
    {
        ClientKillPlayer(player);
    }
}
