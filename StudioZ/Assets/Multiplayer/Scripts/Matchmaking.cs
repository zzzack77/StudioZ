using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;


public class Matchmaking : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private GameObject inputUI;
    
    private UnityTransport transport;
    private const int maxPlayers = 8;

    async void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();
        
        inputUI.SetActive(false);

        await Authenticate();
        
        inputUI.SetActive(true);
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame()
    {
        inputUI.SetActive(false);
        
        Allocation a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        joinCodeText.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
       
        transport.SetHostRelayData(a.RelayServer.IpV4,(ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame()
    {
        inputUI.SetActive(false);
        
        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
        
        transport.SetClientRelayData(a.RelayServer.IpV4,(ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
        NetworkManager.Singleton.StartClient();
    }
}
