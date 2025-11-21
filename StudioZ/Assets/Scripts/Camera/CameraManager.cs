using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cineCam;

    [SerializeField] private CinemachineTargetGroup targetGroup;

    [SerializeField] private float defaultCameraSize = 20;

    private float dist = 0;

    private float zoomScalar = 0.4f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float largestDistance = 0;
        foreach (var p in GameManager.instance.trackedTargets)
        {
            dist = Vector3.Distance(p.transform.position, targetGroup.Sphere.position);

            if (dist > largestDistance)
            {
                largestDistance = dist;
            }
        }

        cineCam.Lens.OrthographicSize = defaultCameraSize + largestDistance * zoomScalar;
    }
}
