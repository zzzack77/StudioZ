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
            PlayerAliveState.OnPlayerDead?.Invoke(player);
        }
    }
}
