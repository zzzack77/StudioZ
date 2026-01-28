using Unity.Netcode;
using UnityEngine;

public class MultiplayerDeathBox : NetworkBehaviour
{
    GameObject playerGO;
    private void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) return;
        playerGO = collision.gameObject.transform.parent.gameObject;

        if (playerGO != null)
        {
            KillPlayerServerRpc();
        }
    }

    [ClientRpc]
    private void KillPlayerClientRpc()
    {
        PlayerAliveState.OnPlayerDead?.Invoke(playerGO);
    }

    [ServerRpc(RequireOwnership = false)]
    public void KillPlayerServerRpc()
    {
        KillPlayerClientRpc();
    }
}
