using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerGameData : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            enabled = false;
        }
    }

    [SerializeField] private string username;

    // Clamps the length of the username to be max 12 characters
    public string Username
    {
        get => username;
        set
        {
            int maxLength = 12;
            username = string.IsNullOrEmpty(value)
                ? value
                : value.Substring(0, Mathf.Min(value.Length, maxLength));
        }
    }
}
