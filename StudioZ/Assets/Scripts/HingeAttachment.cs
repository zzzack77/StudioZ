using UnityEngine;

public class HingeAttachment : MonoBehaviour
{
    public HingeJoint2D joint;

    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("pressed");
            joint.connectedBody = null;
        }
    }
}
