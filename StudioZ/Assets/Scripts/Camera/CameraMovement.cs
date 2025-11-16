using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraMovement : NetworkBehaviour
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

    void LateUpdate()
    {
        if (GameManager.instance != null && GameManager.instance.playerGameObjects.Count != 0)
        {
            addedPlayerPositions = Vector3.zero;
            playerVectors.Clear();


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

                        
                        if (dist > largestDistance)
                        {
                            largestDistance = dist;
                        }
                        
                    }
                }
                
                cameraRef.orthographicSize = Mathf.Min(defaultCameraSize + largestDistance * zoomRate, maxDistance);
            }
            
            Vector3 midPoint = addedPlayerPositions / GameManager.instance.playerGameObjects.Count;
            midPoint.z = -10f;

            transform.position = Vector3.Lerp(transform.position, midPoint, cameraSpeed * Time.deltaTime);
        }

    }
        
}

    

