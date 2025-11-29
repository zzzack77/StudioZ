using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UserNameDisplay : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            enabled = false;
        }
    }

    [SerializeField] private PlayerGameData playerData;
    [SerializeField] private TextMeshProUGUI userNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userNameText.text = playerData.Username;
    }

    // Update is called once per frame
    void Update()
    {
        userNameText.text = playerData.Username;
    }
}
