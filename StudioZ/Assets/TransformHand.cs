using UnityEngine;

public class TransformHand : MonoBehaviour
{
    public Transform hand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = hand.position;
    }
}
