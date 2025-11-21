using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraMovement : NetworkBehaviour
{
    private Camera cameraRef;

    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private float defaultCameraSize = 13f;
    [SerializeField] private float zoomRate = 0.1f;
    [SerializeField] private float maxTrackingDistance = 25f;

    private void Start()
    {
        cameraRef = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        var players = GameManager.instance?.playerGameObjects;
        if (players == null || players.Count == 0) return;

        // TEMP LIST (don't use a persistent list!)
        List<GameObject> trackedPlayers = new List<GameObject>();

        // 1. compute midpoint of ALL players
        Vector3 globalMid = Vector3.zero;
        foreach (var p in players)
            globalMid += p.transform.position;
        globalMid /= players.Count;

        // 2. find players close enough to track
        foreach (var p in players)
        {
            float dist = Vector3.Distance(p.transform.position, globalMid);
            if (dist <= maxTrackingDistance)
                trackedPlayers.Add(p);
        }

        // 3. If nobody is close enough, track everyone
        if (trackedPlayers.Count == 0)
            trackedPlayers.AddRange(players);

        // 4. Compute midpoint of tracked players
        Vector3 mid = Vector3.zero;
        foreach (var p in trackedPlayers)
            mid += p.transform.position;
        mid /= trackedPlayers.Count;
        mid.z = -10f;

        // 5. Compute camera zoom based on tracked players
        float largestDist = 0f;
        for (int i = 0; i < trackedPlayers.Count; i++)
        {
            for (int j = i + 1; j < trackedPlayers.Count; j++)
            {
                float dist = Vector3.Distance(
                    trackedPlayers[i].transform.position,
                    trackedPlayers[j].transform.position
                );
                if (dist > largestDist)
                    largestDist = dist;
            }
        }

        cameraRef.orthographicSize = defaultCameraSize + largestDist * zoomRate;

        // 6. Move camera
        transform.position = Vector3.Lerp(
            transform.position,
            mid,
            cameraSpeed * Time.deltaTime
        );
    }
}
