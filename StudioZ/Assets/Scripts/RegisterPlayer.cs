using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class RegisterPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }

    private void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPlayer(this.gameObject);
        }
    }
    private void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPlayer(this.gameObject);
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.UnregisterPlayer(this.gameObject);
        }
    }

}
