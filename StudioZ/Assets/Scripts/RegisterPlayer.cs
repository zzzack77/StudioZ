using System.Globalization;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class RegisterPlayer : NetworkBehaviour
{

    [SerializeField] private CinemachineTargetGroup targetGroup;

    [SerializeField] private float maxDistance = 10f;
    
    [SerializeField] PlayerCameraHandler playerCameraHandler;

    [SerializeField] private GameObject body;

    
    

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(NetworkObject.OwnerClientId, this.gameObject);
            if (targetGroup != null && !GameManager.Instance.trackedTargets.Contains(body))
            {
                
                targetGroup.AddMember(body.transform, 3f, 0.2f);
                GameManager.Instance.trackedTargets.Add(body);
            }
        }
        targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
    }

    public override void OnNetworkDespawn()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterPlayer(NetworkObject.OwnerClientId);
            if (targetGroup != null && GameManager.Instance.trackedTargets.Contains(body))
            {
                targetGroup.RemoveMember(body.transform);
                GameManager.Instance.trackedTargets.Remove(body);
            }
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(NetworkObject.OwnerClientId, this.gameObject);
        }
        if (targetGroup == null && GameMode.IsMultiplayer) Debug.LogError("Target group ref is null!");
        if (targetGroup != null && !GameManager.Instance.trackedTargets.Contains(body))
        {
            targetGroup.AddMember(body.transform, 3f, 0.2f);
            GameManager.Instance.trackedTargets.Add(body);
        }
    }

    private void Update()
    {
        if (targetGroup != null && playerCameraHandler != null)
        {
            float dist = Vector3.Distance(body.transform.position, targetGroup.Sphere.position);

            if (dist > maxDistance)
            {
                if (GameManager.Instance.trackedTargets.Contains(body))
                {
                    targetGroup.RemoveMember(body.transform);
                    GameManager.Instance.trackedTargets.Remove(body);
                    playerCameraHandler.ActivateCamera();

                }

            }
            else if (dist < maxDistance && !GameManager.Instance.trackedTargets.Contains(body))
            {
                targetGroup.AddMember(body.transform, 3f, 0.2f);
                GameManager.Instance.trackedTargets.Add(body);
                playerCameraHandler.DeactivateCamera();
            }

            float largestDistance = 0;
            foreach (var p in GameManager.Instance.trackedTargets)
            {
                dist = Vector3.Distance(p.transform.position, targetGroup.Sphere.position);

                if (dist > largestDistance)
                {
                    largestDistance = dist;
                }
            }
        }
    }
}
