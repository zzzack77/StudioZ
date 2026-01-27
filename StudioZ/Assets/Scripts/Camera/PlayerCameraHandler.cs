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
            ActivatePersonalCamera();
            // Creates a list of alive players
            var players = GameManager.Instance.playerGameObjects.Values.Where(p => p.GetComponent<PlayerAliveState>().aliveState == AliveState.Alive).ToList();

            Debug.Log("There are " +  players.Count + " players alive!");
            if (players.Count == 0)
                Debug.Log("there are no alive players!");
            // Cycle through each alive player
            else
            {
                if (input.BumperLPressed())
                {
                    Debug.Log("BumperLPressed");
                    playerIndex--;
                    if (playerIndex < 0)
                    {
                        playerIndex = players.Count - 1;

                    }
                }
                else if (input.BumperRPressed())
                {
                    Debug.Log("BumperRPressed");
                    playerIndex++;
                    if (playerIndex >= players.Count)
                    {
                        playerIndex = 0;
                    }

                }

                GameObject playerToSpectate = players[playerIndex];
                
                PlayerCameraHandler playerCamera = playerToSpectate.GetComponent<PlayerCameraHandler>();
                cineCam.Follow = playerCamera.body.transform;
            }
            
        }
        else if (playerAliveState.aliveState == AliveState.Alive) cineCam.Follow = body.transform; // Follow yourself
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
