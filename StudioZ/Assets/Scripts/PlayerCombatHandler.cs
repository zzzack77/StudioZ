using Unity.Netcode;
using UnityEngine;

public class PlayerCombatHandler : NetworkBehaviour
{
    private NetworkPlayerMovement player;
    private Rigidbody lHandRB;
    private Rigidbody RHandRB;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }

    private void Start()
    {
        player = GetComponent<NetworkPlayerMovement>();
        lHandRB = player.L_handRB;
        RHandRB = player.R_handRB;
    }

    private void Update()
    {
        

    }

    
}
