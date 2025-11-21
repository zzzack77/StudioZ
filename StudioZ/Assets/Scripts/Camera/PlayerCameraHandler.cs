using Unity.Netcode;
using UnityEngine;

public class PlayerCameraHandler : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private float cameraSpeed;
    private Vector3 lerpPos;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is null!");
        }
        if (playerBody == null)
        {
            Debug.LogError("Player body is null!");
        }
    }

    
    void LateUpdate()
    {
        lerpPos = Vector3.Lerp(playerCamera.transform.position, playerBody.transform.position, cameraSpeed * Time.deltaTime);
        lerpPos.z = -10;
        playerCamera.transform.position = lerpPos;
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
