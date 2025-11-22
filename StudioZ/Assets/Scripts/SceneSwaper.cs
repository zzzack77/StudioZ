using Unity.Netcode;
using UnityEngine;

public class SceneSwaper : MonoBehaviour
{
    public IPlayerInput input;
    public NetworkSceneLoader netScene;
    private void Start()
    {
        input = GetComponent<IPlayerInput>();
    }

    private void Update()
    {
        if (input != null)
        {
            if (input.ButtonSouthPressed())
            {
                netScene.LoadSceneForAll("SceneTest2");
            }
        }
    }
}
        
        
