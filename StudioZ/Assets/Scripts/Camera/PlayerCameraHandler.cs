using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCameraHandler : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private CinemachineCamera cineCam;
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
        cineCam.Priority = 2;
    }

    public void DeactivateCamera()
    {
        cineCam.Priority = 0;
    }
}
