using UnityEngine;

public class RotatingHoldChild : MonoBehaviour
{
    [SerializeField] private float rotatingAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(new Vector3 (0, 0, rotatingAmount), Space.Self);
    }
}
