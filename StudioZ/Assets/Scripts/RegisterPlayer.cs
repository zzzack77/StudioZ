using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class RegisterPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPlayer(this.gameObject);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (GameManager.instance != null)
            GameManager.instance.UnregisterPlayer(gameObject);
    }
}
