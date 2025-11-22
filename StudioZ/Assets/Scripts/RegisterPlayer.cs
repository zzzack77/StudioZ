using System.Globalization;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class RegisterPlayer : NetworkBehaviour
{

    [SerializeField] private CinemachineTargetGroup targetGroup;

    [SerializeField] private float maxDistance = 10f;
    
    [SerializeField] PlayerCameraHandler playerCameraHandler;

    
    

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPlayer(this.gameObject);
            if (targetGroup != null && !GameManager.instance.trackedTargets.Contains(this.gameObject))
            {
                
                targetGroup.AddMember(transform, 3f, 0.2f);
                GameManager.instance.trackedTargets.Add(this.gameObject);
            }
        }
        targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
    }

    public override void OnNetworkDespawn()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.UnregisterPlayer(gameObject);
            if (targetGroup != null && GameManager.instance.trackedTargets.Contains(this.gameObject))
            {
                targetGroup.RemoveMember(transform);
                GameManager.instance.trackedTargets.Remove(this.gameObject);
            }
        }
    }

    private void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPlayer(this.gameObject);
        }
        if (targetGroup == null) Debug.LogError("Target group ref is null!");
        if (targetGroup != null && !GameManager.instance.trackedTargets.Contains(this.gameObject))
        {
            targetGroup.AddMember(transform, 3f, 0.2f);
            GameManager.instance.trackedTargets.Add(this.gameObject);
        }
    }

    private void Update()
    {
        if (targetGroup != null && playerCameraHandler != null)
        {
            float dist = Vector3.Distance(transform.position, targetGroup.Sphere.position);

            if (dist > maxDistance)
            {
                if (GameManager.instance.trackedTargets.Contains(this.gameObject))
                {
                    targetGroup.RemoveMember(transform);
                    GameManager.instance.trackedTargets.Remove(this.gameObject);
                    playerCameraHandler.ActivateCamera();

                }

            }
            else if (dist < maxDistance && !GameManager.instance.trackedTargets.Contains(this.gameObject))
            {
                targetGroup.AddMember(transform, 3f, 0.2f);
                GameManager.instance.trackedTargets.Add(this.gameObject);
                playerCameraHandler.DeactivateCamera();
            }

            float largestDistance = 0;
            foreach (var p in GameManager.instance.trackedTargets)
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
