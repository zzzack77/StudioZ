using UnityEngine;

public class PlayerGriping : MonoBehaviour
{
    public GameObject LHandOpen;
    public GameObject LHandClose;
    public GameObject LHandPoint;

    public GameObject RHandOpen;
    public GameObject RHandClose;
    public GameObject RHandPoint;

    public Transform LTogglePos;
    public Transform RTogglePos;

    private bool lastLeft;
    private bool lastRight;

    private bool Left;
    private bool Right;

    NetworkPlayerMovement playerMovement;
    void Awake()
    {
        playerMovement = GetComponent<NetworkPlayerMovement>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastLeft = !playerMovement.L_isGripping;
        lastRight = !playerMovement.R_isGripping;
    }

    // Update is called once per frame
    void Update()
    {
        //GripLeftHand();
        //GripRightHand();
        GripCheck();

        LeftHandRoataion();

        RightHandRoataion();
    }


    public void GripCheck()
    {
       if (playerMovement.L_isGripping != lastLeft)
       {
           GripLeftHand();
       }
       if (playerMovement.R_isGripping != lastRight)
       {
           GripRightHand();
       }
    }

    //Controls which Left hand model is active based on if the player is gripping or not 
    public void GripLeftHand()
    {
        Left = playerMovement.L_isGripping;

        LHandOpen.SetActive(!Left);
        LHandClose.SetActive(Left);

        lastLeft = Left;
    }
    //Controls which Right hand model is active based on if the player is gripping or not 
    public void GripRightHand()
    {
        Right = playerMovement.R_isGripping;

        RHandOpen.SetActive(!Right);
        RHandClose.SetActive(Right);

        lastRight = Right;
    }

    public void LeftHandRoataion()
    {
        if (Left) return;

        Vector3 direction = LTogglePos.position - LHandPoint.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        LHandOpen.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void RightHandRoataion()
    {
        if (Right) return;

        Vector3 direction = RTogglePos.position - RHandPoint.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        RHandOpen.transform.rotation = Quaternion.Euler(0, 0, angle);
        //Debug.Log(angle);
        //Debug.Log(RHandPoint.transform.rotation);
    }


}
