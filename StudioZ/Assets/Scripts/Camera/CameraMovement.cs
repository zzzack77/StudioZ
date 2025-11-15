using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Focal point will be the centre of the map, or just the area of interest
    [SerializeField] private Transform focalPoint;
    [SerializeField] private Transform player;

    private float cameraSpeed = 5f;
    private Camera cameraRef;

    void Start()
    {
        cameraRef = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        // Gets the midpoint between the player and focal point
        Vector3 midpoint = (player.position + focalPoint.position) * 0.5f;
        midpoint.z = -10f; // keep camera in 2D view

        // Move camera toward the midpoint
        transform.position = Vector3.Lerp(transform.position, midpoint, cameraSpeed * Time.deltaTime);

        // 3. Adjust field of view based on ACTUAL distance between the two points
        float dist = Vector3.Distance(player.position, focalPoint.position);
        cameraRef.fieldOfView = 90 + dist * 0.5f;
    }
}
