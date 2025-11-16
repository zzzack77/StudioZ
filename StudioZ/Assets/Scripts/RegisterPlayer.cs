using UnityEngine;

public class RegisterPlayer : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPlayer(this.gameObject);
        }
    }
    private void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterPlayer(this.gameObject);
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.UnregisterPlayer(this.gameObject);
        }
    }

}
