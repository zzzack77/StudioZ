using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UI)
        {
            UI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // for testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (UI)
            {
                UI.SetActive(true);
            }
        }
    }

    public void LeaveMultiplayerGame()
    {
        if (NetworkManager.Singleton == null) return;
       
        NetworkManager.Singleton.Shutdown();
        //change scene to main menu 
        SceneManager.LoadScene("EasyLobbyScene");
    }
}
