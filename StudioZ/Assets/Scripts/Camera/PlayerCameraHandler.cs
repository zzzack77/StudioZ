using Unity.Netcode;
using UnityEngine;

public class PlayerCameraHandler : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }

    void Start()
    {
        
    }

    public void ActivateCamera()
    {
        playerCamera.SetActive(true);
    }

    public void DeactivateCamera()
    {
        playerCamera.SetActive(false);
    }
}
