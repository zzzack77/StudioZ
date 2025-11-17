using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraMovement2 : NetworkBehaviour
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
            


            foreach (GameObject playerGameObject in GameManager.instance.playerGameObjects)
            {
                addedPlayerPositions += playerGameObject.transform.position;
            }

            
            
            Vector3 midPoint = addedPlayerPositions / GameManager.instance.playerGameObjects.Count;
            midPoint.z = -10f;

            transform.position = Vector3.Lerp(transform.position, midPoint, cameraSpeed * Time.deltaTime);

            foreach (GameObject playerGameObject in GameManager.instance.playerGameObjects)
            {
                
                if (playerVectors.Count > 1 && playerVectors.Count < 5)
                {
                    float largestDistance = 0f;

                    float dist = Vector3.Distance(playerGameObject.transform.position, midPoint);


                    if (dist > largestDistance && dist < maxDistance)
                    {
                        largestDistance = dist;
                    }
                    else if (dist > maxDistance)
                    {
                        // Call a function to give player their own camera
                        Debug.Log(playerGameObject.name + "Has gone out of range");
                    }

                        cameraRef.orthographicSize = Mathf.Min(defaultCameraSize + largestDistance * zoomRate, maxDistance);
                }
            }
        }

    }
        
}

    

