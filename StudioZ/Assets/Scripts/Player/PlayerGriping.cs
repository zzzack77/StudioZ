using UnityEngine;

public class PlayerGriping : MonoBehaviour
{
    [Header("Left Hand")]
    public SpriteRenderer LHandRenderer;
    public Sprite LHandOpen;
    public Sprite LHandClose;

    [Header ("Right Hand")]
    public SpriteRenderer RHandRenderer;
    public Sprite RHandOpen;
    public Sprite RHandClose;

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

        if (Left)
        {
            LHandRenderer.sprite = LHandClose;
        } 
        else
        {
            LHandRenderer.sprite = LHandOpen;
        }

        lastLeft = Left;
    }
    //Controls which Right hand model is active based on if the player is gripping or not 
    public void GripRightHand()
    {
        Right = playerMovement.R_isGripping;

        if (Right)
        {
            RHandRenderer.sprite = RHandClose;
        }
        else
        {
            RHandRenderer.sprite = RHandOpen;
        }

        lastRight = Right;
    }

   

}
