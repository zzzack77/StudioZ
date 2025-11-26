using UnityEngine;

public class RotatingHold : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float rotatingAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.Rotate(new Vector3 (0, 0, rotatingAmount), Space.World);

        rb.AddTorque(new Vector3(0, 0, rotatingAmount), ForceMode.Force);
    }
}
