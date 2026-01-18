using System.Globalization;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class UserNameDisplay : NetworkBehaviour
{
    
    public NetworkVariable<FixedString32Bytes> networkName = new NetworkVariable<FixedString32Bytes>();
   
    [SerializeField] private TextMeshProUGUI userNameText;
    
    public Color[] playerColors;

    public override void OnNetworkSpawn()
    {
        // 1. Listen for changes
        networkName.OnValueChanged += OnNameChanged;
        
        // 2. Initial update
        UpdateNameText(networkName.Value.ToString());

        // 3. If this is MY player, request the server to set my name
        if (IsOwner)
        {
            // We cannot set networkName.Value directly here as a client.
            // We must ask the server to do it.
            SetPlayerNameServerRpc(SimpleMatchmaking.Instance.playerName);
        }
        
        // --- 2. HANDLE COLOR SETTING ---
        SetPlayerColor();
        
    }


    private void SetPlayerColor()
    {
        if (playerColors.Length == 0) return;
        
        ulong ownerId = OwnerClientId;
        if ((int)ownerId < playerColors.Length)
        {
            Color assignedColor = playerColors[ownerId];

            userNameText.color = assignedColor;
        }
       
    }

    // This function runs on the SERVER
    [ServerRpc]
    private void SetPlayerNameServerRpc(string name)
    {
        // The server has permission to edit the NetworkVariable
        networkName.Value = name;
    }

    private void OnNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        UpdateNameText(newName.ToString());
    }

    private void UpdateNameText(string name)
    {
        userNameText.text = name;
    }

    public override void OnNetworkDespawn()
    {
        networkName.OnValueChanged -= OnNameChanged;
    }
}