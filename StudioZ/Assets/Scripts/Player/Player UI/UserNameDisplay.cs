using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UserNameDisplay : NetworkBehaviour
{
    // --- NETWORK VARIABLES ---
    public NetworkVariable<FixedString32Bytes> networkName = new NetworkVariable<FixedString32Bytes>();
    
    public NetworkVariable<int> colorIndex = new NetworkVariable<int>(
        -1, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
   
    [SerializeField] private TextMeshProUGUI userNameText;
    public Color[] playerColors;
    
    // --- SERVER-SIDE TRACKER ---
    private static bool[] colorTakenMap;
    private static bool isPoolInitialized = false;

    public static int mySavedColorIndex = -1;

    public override void OnNetworkSpawn()
    {
        networkName.OnValueChanged += OnNameChanged;
        colorIndex.OnValueChanged += OnColorChanged;
        
        UpdateNameText(networkName.Value.ToString());

        if (IsOwner)
        {
            RequestNameAndColorServerRpc(SimpleMatchmaking.Instance.playerName, mySavedColorIndex);
        }
        
        if (colorIndex.Value != -1)
        {
            ApplyPlayerColor(colorIndex.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        networkName.OnValueChanged -= OnNameChanged;
        colorIndex.OnValueChanged -= OnColorChanged;

        // SERVER CLEANUP: When a player leaves, free up their specific color slot
        if (IsServer && colorIndex.Value >= 0 && colorIndex.Value < colorTakenMap.Length)
        {
            colorTakenMap[colorIndex.Value] = false;
        }
    }

    // --- SERVER RPC ---
    
    [ServerRpc]
    private void RequestNameAndColorServerRpc(string name, int requestedColorIndex)
    {
        networkName.Value = name;

        if (!isPoolInitialized) InitializeColorPool();

        // Host Migration: Check if they are asking for their old color back AND if it's currently free
        if (requestedColorIndex >= 0 && 
            requestedColorIndex < colorTakenMap.Length && 
            !colorTakenMap[requestedColorIndex])
        {
            colorTakenMap[requestedColorIndex] = true;
            colorIndex.Value = requestedColorIndex;
        }
        else 
        {
            AssignLowestAvailableColor();
        }
    }

    // --- COLOR LOGIC ---

    private void InitializeColorPool()
    {
        if (!isPoolInitialized)
        {
            // Create a checklist exactly the size of your color array
            colorTakenMap = new bool[playerColors.Length];
            isPoolInitialized = true;
        }
    }
    // Finds the next available colour so colours remain inline with player numbers
    private void AssignLowestAvailableColor()
    {
        // Start from 0 and find the first empty slot
        for (int i = 0; i < colorTakenMap.Length; i++)
        {
            if (!colorTakenMap[i]) // If the slot is false (empty)
            {
                colorTakenMap[i] = true; // Mark it as taken
                colorIndex.Value = i;    // Assign it to the player
                return;                  // Exit the loop
            }
        }
        
        Debug.LogWarning("All color slots are taken!");
    }

    private void OnColorChanged(int oldIndex, int newIndex)
    {
        if (IsOwner) 
        {
            mySavedColorIndex = newIndex;
        }

        ApplyPlayerColor(newIndex);
    }

    private void ApplyPlayerColor(int index)
    {
        if (playerColors.Length == 0 || index < 0 || index >= playerColors.Length) return;
        
        userNameText.color = playerColors[index];
    }

    private void OnNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        UpdateNameText(newName.ToString());
    }

    private void UpdateNameText(string name)
    {
        userNameText.text = name;
    }

    // --- CLEANUP ---

    public static void ResetColorPool()
    {
        // Null out the array completely so it regenerates cleanly next game
        colorTakenMap = null; 
        isPoolInitialized = false;
        mySavedColorIndex = -1;
    }
}