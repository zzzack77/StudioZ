using UnityEngine;

public class playerMovment : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0.1f, 0);
        }
        if (Input.GetKey(KeyCode.S)) 
        {
            transform.position += new Vector3(0, -0.1f, 0);
        }
    }
}
