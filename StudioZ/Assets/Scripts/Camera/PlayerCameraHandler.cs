using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerCameraHandler : NetworkBehaviour
{
    private IPlayerInput input;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private CinemachineCamera cineCam;
    private PlayerAliveState playerAliveState;
    int playerIndex = 0;

    [SerializeField] private CinemachineTargetGroup targetGroup;

    public GameObject body;

    private GameObject currentSpectatedPlayer;

    private bool isSpectating = false;
    private void Awake()
    {
        input = GetComponent<IPlayerInput>();
        playerAliveState = GetComponent<PlayerAliveState>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;

        targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
    }

    private void OnEnable()
    {
        PlayerAliveState.OnPlayerDead += RemovePlayerFromMultiCamera;
        PlayerAliveState.OnPlayerAlive += AddPlayerToMultiCamera;
    }

    private void OnDisable()
    {
        PlayerAliveState.OnPlayerDead -= RemovePlayerFromMultiCamera;
        PlayerAliveState.OnPlayerAlive -= AddPlayerToMultiCamera;
    }
    void Start()
    {
        
    }

    public void ActivatePersonalCamera()
    {
        cineCam.Priority = 2;
    }

    public void DeactivatePersonalCamera()
    {
        cineCam.Priority = 0;
    }

    private void Update()
    {
        Spectate();
    }

    private void Spectate()
    {
        if (playerAliveState != null && playerAliveState.aliveState == AliveState.Dead)
        {
            if (!isSpectating)
            {
                ActivatePersonalCamera();
                isSpectating = true;
            } 
                
            // Creates a list of alive players
            List<GameObject> players = GameManager.Instance.playerGameObjects.Values
            .Where(p => p.GetComponent<PlayerAliveState>().aliveState == AliveState.Alive)
            .OrderBy(p => p.GetComponent<NetworkObject>().OwnerClientId) // stable order
            .ToList();

           

            if (players.Count == 0)
            {
                Debug.Log("there are no alive players!");
                return;
            }
                
            // Cycle through each alive player
            
            if (currentSpectatedPlayer == null || !players.Contains(currentSpectatedPlayer))
            {
                currentSpectatedPlayer = players[0];
                playerIndex = 0;
            }
            else
            {
                playerIndex = players.IndexOf(currentSpectatedPlayer);
            }

            if (input.BumperLPressed())
            {
                playerIndex = (playerIndex - 1 + players.Count) % players.Count;
                currentSpectatedPlayer = players[playerIndex];
            }
            else if (input.BumperRPressed())
            {
                playerIndex = (playerIndex + 1) % players.Count;
                currentSpectatedPlayer = players[playerIndex];

            }

            PlayerCameraHandler cam = currentSpectatedPlayer.GetComponent<PlayerCameraHandler>();
            if (cineCam.Follow != cam.body.transform)
            {
                cineCam.Follow = cam.body.transform;
            }


        }
        else if (playerAliveState.aliveState == AliveState.Alive)
        {
            isSpectating = false;
            currentSpectatedPlayer = null;
            cineCam.Follow = body.transform;
        }
    }

    private void RemovePlayerFromMultiCamera(GameObject player)
    {
        if (targetGroup != null && player == gameObject && GameManager.Instance.trackedTargets.Contains(body))
        {
            targetGroup.RemoveMember(body.transform);
            GameManager.Instance.trackedTargets.Remove(body);
        }
    }

    private void AddPlayerToMultiCamera(GameObject player)
    {
        if (targetGroup != null && player == gameObject && !GameManager.Instance.trackedTargets.Contains(body))
        {
            targetGroup.AddMember(body.transform, 3f, 0.2f);
            GameManager.Instance.trackedTargets.Add(body);
        }
    }
}
