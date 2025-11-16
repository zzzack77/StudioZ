using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> playerTransforms = new List<Transform>();

    [SerializeField] private float cameraSpeed = 5f;
    private Camera cameraRef;

    [SerializeField] private float defaultCameraSize = 13f;
    [SerializeField] private float zoomRate = 0.1f;
    void Start()
    {
        cameraRef = GetComponent<Camera>();
    }

    void Update()
    {
        switch (playerTransforms.Count)
        {
            // If there is just 1 player then get the camera to follow the player
            case 1:
                Vector3 playerPosition = new Vector3(playerTransforms[0].position.x, playerTransforms[0].position.y, -10);

                // Move camera toward the player
                transform.position = Vector3.Lerp(transform.position, playerPosition, cameraSpeed * Time.deltaTime);
                cameraRef.orthographicSize = defaultCameraSize;
                break;

            // If there are 2 players then get the mid point between both of these and zoom out the camera based on distance from each other
            case 2:
                
                // Gets the midpoint between the player and focal point
                Vector3 midpoint1 = (playerTransforms[0].position + playerTransforms[1].position) * 0.5f;
                midpoint1.z = -10f; // keep camera in 2D view

                // Move camera toward the midpoint
                transform.position = Vector3.Lerp(transform.position, midpoint1, cameraSpeed * Time.deltaTime);

                // Compares all the players distances from eachother
                float dist = Vector3.Distance(playerTransforms[0].position, playerTransforms[1].position);
                // Sets the cameras zoom
                cameraRef.orthographicSize = defaultCameraSize + dist * zoomRate;
                break;

            // If there are 3 players then get the mid point between them and zoom out the camera based on distance from the furthest away player
            case 3:

                // Gets the midpoint between the player and focal point
                Vector3 midpoint2 = (playerTransforms[0].position + playerTransforms[1].position + playerTransforms[2].position) * 0.33f;
                midpoint2.z = -10f; // keep camera in 2D view

                // Move camera toward the midpoint
                transform.position = Vector3.Lerp(transform.position, midpoint2, cameraSpeed * Time.deltaTime);

                // Compares all the players distances from eachother
                float dist1 = Vector3.Distance(playerTransforms[0].position, playerTransforms[1].position);
                float dist2 = Vector3.Distance(playerTransforms[0].position, playerTransforms[2].position);
                float dist3 = Vector3.Distance(playerTransforms[1].position, playerTransforms[2].position);

                float largestDist = Mathf.Max(dist1, dist2, dist3);
                cameraRef.orthographicSize = defaultCameraSize + largestDist * zoomRate;
                break;

            // If there are 4 players then get the mid point between them and zoom out the camera based on distance from the furthest away player
            case 4:
                // Gets the midpoint between the player and focal point
                Vector3 midpoint3 = (playerTransforms[0].position + playerTransforms[1].position
                    + playerTransforms[2].position + playerTransforms[3].position) * 0.25f;
                midpoint3.z = -10f; // keep camera in 2D view) 
                

                // Move camera toward the midpoint
                transform.position = Vector3.Lerp(transform.position, midpoint3, cameraSpeed * Time.deltaTime);

                // Compares all the players distances from eachother
                float dista1 = Vector3.Distance(playerTransforms[0].position, playerTransforms[1].position);
                float dista2 = Vector3.Distance(playerTransforms[0].position, playerTransforms[2].position);
                float dista3 = Vector3.Distance(playerTransforms[0].position, playerTransforms[3].position);
                float dista4 = Vector3.Distance(playerTransforms[1].position, playerTransforms[2].position);
                float dista5 = Vector3.Distance(playerTransforms[1].position, playerTransforms[3].position);
                float dista6 = Vector3.Distance(playerTransforms[2].position, playerTransforms[3].position);

                float largestDistance = Mathf.Max(dista1, dista2, dista3, dista4, dista5, dista6);
                cameraRef.orthographicSize = defaultCameraSize + largestDistance * zoomRate;
                break;

        }
        
    }

    void AddToTransformList(Transform transform)
    {
        playerTransforms.Add(transform);
    }
}
