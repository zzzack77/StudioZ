using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //[SerializeField] private List<GameObject> playerGameObjects = new List<GameObject>();

    private List<Vector3> playerVectors = new List<Vector3>();

    private Vector3 addedPlayerPositions;

    [SerializeField] private float cameraSpeed = 5f;
    private Camera cameraRef;

    [SerializeField] private float defaultCameraSize = 13f;
    [SerializeField] private float zoomRate = 0.1f;
    [SerializeField] private float maxDistance;
    private float largestDistance;


    private Vector3 prevVector;
    void Start()
    {
        cameraRef = GetComponent<Camera>();
    }

    void Update()
    {
        if (GameManager.instance != null)
        {
            addedPlayerPositions = Vector3.zero;
            playerVectors = new List<Vector3>();


            foreach (GameObject playerGameObject in GameManager.instance.playerGameObjects)
            {
                addedPlayerPositions += playerGameObject.transform.position;
                playerVectors.Add(playerGameObject.transform.position);

            }

            if (playerVectors.Count > 1 && playerVectors.Count < 5)
            {
                float largestDistance = 0f;

                for (int i = 0; i < playerVectors.Count; i++)
                {
                    
                    for (int j = i + 1; j < playerVectors.Count; j++)
                    {
                        float dist = Vector3.Distance(playerVectors[i], playerVectors[j]);

                        Debug.Log(dist);

                        
                        if (dist > largestDistance)
                        {
                            largestDistance = dist;
                        }
                        
                    }
                }
                
                cameraRef.orthographicSize = Mathf.Min(defaultCameraSize + largestDistance * zoomRate, maxDistance);
            }
            

            Vector3 midPoint = new Vector3(addedPlayerPositions.x / GameManager.instance.playerGameObjects.Count, 
                                            addedPlayerPositions.y / GameManager.instance.playerGameObjects.Count, -10);

            transform.position = Vector3.Lerp(transform.position, midPoint, cameraSpeed * Time.deltaTime);
        }

        

        //switch (playerGameObjects.Count)
        //{
        //    // If there is just 1 player then get the camera to follow the player
        //    case 1:
        //        Vector3 playerPosition = new Vector3(playerGameObjects[0].transform.position.x, playerGameObjects[0].transform.position.y, -10);

        //        // Move camera toward the player
        //        transform.position = Vector3.Lerp(transform.position, playerPosition, cameraSpeed * Time.deltaTime);
        //        cameraRef.orthographicSize = defaultCameraSize;
        //        break;

        //    // If there are 2 players then get the mid point between both of these and zoom out the camera based on distance from each other
        //    case 2:
                
        //        // Gets the midpoint between the player and focal point
        //        Vector3 midpoint1 = (playerGameObjects[0].transform.position + playerGameObjects[1].transform.position) * 0.5f;
        //        midpoint1.z = -10f; // keep camera in 2D view

        //        // Move camera toward the midpoint
        //        transform.position = Vector3.Lerp(transform.position, midpoint1, cameraSpeed * Time.deltaTime);

        //        // Compares all the players distances from eachother
        //        float dist = Vector3.Distance(playerGameObjects[0].transform.position, playerGameObjects[1].transform.position);
        //        // Sets the cameras zoom
        //        cameraRef.orthographicSize = defaultCameraSize + dist * zoomRate;
        //        break;

        //    // If there are 3 players then get the mid point between them and zoom out the camera based on distance from the furthest away player
        //    case 3:

        //        // Gets the midpoint between the player and focal point
        //        Vector3 midpoint2 = (playerGameObjects[0].transform.position + playerGameObjects[1].transform.position + playerGameObjects[2].transform.position) * 0.33f;
        //        midpoint2.z = -10f; // keep camera in 2D view

        //        // Move camera toward the midpoint
        //        transform.position = Vector3.Lerp(transform.position, midpoint2, cameraSpeed * Time.deltaTime);

        //        // Compares all the players distances from eachother
        //        float dist1 = Vector3.Distance(playerGameObjects[0].transform.position, playerGameObjects[1].transform.position);
        //        float dist2 = Vector3.Distance(playerGameObjects[0].transform.position, playerGameObjects[2].transform.position);
        //        float dist3 = Vector3.Distance(playerGameObjects[1].transform.position, playerGameObjects[2].transform.position);

        //        float largestDist = Mathf.Max(dist1, dist2, dist3);
        //        cameraRef.orthographicSize = defaultCameraSize + largestDist * zoomRate;
        //        break;

        //    // If there are 4 players then get the mid point between them and zoom out the camera based on distance from the furthest away player
        //    case 4:
        //        // Gets the midpoint between the player and focal point
        //        Vector3 midpoint3 = (playerGameObjects[0].transform.position + playerGameObjects[1].transform.position
        //            + playerGameObjects[2].transform.position + playerGameObjects[3].transform.position) * 0.25f;
        //        midpoint3.z = -10f; // keep camera in 2D view) 
                

        //        // Move camera toward the midpoint
        //        transform.position = Vector3.Lerp(transform.position, midpoint3, cameraSpeed * Time.deltaTime);

        //        // Compares all the players distances from eachother
        //        float dista1 = Vector3.Distance(playerGameObjects[0].transform.position, playerGameObjects[1].transform.position);
        //        float dista2 = Vector3.Distance(playerGameObjects[0].transform.position, playerGameObjects[2].transform.position);
        //        float dista3 = Vector3.Distance(playerGameObjects[0].transform.position, playerGameObjects[3].transform.position);
        //        float dista4 = Vector3.Distance(playerGameObjects[1].transform.position, playerGameObjects[2].transform.position);
        //        float dista5 = Vector3.Distance(playerGameObjects[1].transform.position, playerGameObjects[3].transform.position);
        //        float dista6 = Vector3.Distance(playerGameObjects[2].transform.position, playerGameObjects[3].transform.position);

        //        float largestDistance = Mathf.Max(dista1, dista2, dista3, dista4, dista5, dista6);
        //        cameraRef.orthographicSize = defaultCameraSize + largestDistance * zoomRate;
        //        break;

        }
        
}

    

