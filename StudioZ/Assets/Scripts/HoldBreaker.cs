using UnityEngine;
using System.Collections;

public class HoldBreaker : MonoBehaviour
{
    private NetworkPlayerMovement networkPlayerMovement;

    [Header("Settings")]
    [SerializeField] private float gripRequiredTime = 2f;   // time from initial press to disable
    [SerializeField] private float disableDuration = 5f;    // how long object stays disabled
    [SerializeField] private float fadedOpacity = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Collider selfCollider;
    private float originalOpacity;

    // Timer
    private bool timerRunning = false;
    private float gripTimer = 0f;

    // Is either hand currently in the collision box
    private bool LhasCollided = false;
    private bool RhasCollided = false;
    
    // is the hold disabled currently
    private bool isDisabled = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        selfCollider = GetComponent<Collider>();
        originalOpacity = spriteRenderer.color.a;
    }

    private void Update()
    {
        // if no player linked or already disabled, nothing to do
        if (networkPlayerMovement == null || isDisabled)
            return;

        // start the shared timer when either hand grips this hold for the first time
        // timer keeps running even if they release
        if (!timerRunning)
        {
            // start timer if any hand is currently gripping AND that hand has registered collision with this hold
            if ((LhasCollided && networkPlayerMovement.L_isGripping) ||
                (RhasCollided && networkPlayerMovement.R_isGripping))
            {
                
                timerRunning = true;
                gripTimer = 0f;
            }
        }

        if (timerRunning)
        {
            gripTimer += Time.deltaTime;

            if (gripTimer >= gripRequiredTime)
            {
                StartCoroutine(DisableRoutine());
            }
        }
    }

    // keep method to receive player reference
    public void GetPlayerReference(NetworkPlayerMovement player)
    {
        if (player != null)
            networkPlayerMovement = player;
    }
   
    public void OnLCollision()
    {
        if (isDisabled) return;
        LhasCollided = true;
    }

    public void OnRCollision()
    {
        if (isDisabled) return;
        RhasCollided = true;
    }

    // Called when left stops gripping
    public void EndLGrip()
    {
        LhasCollided = false;
    }

    // Called when right stops gripping
    public void EndRGrip()
    {
        RhasCollided = false;
    }

    // ---------- Disable routine ----------
    private IEnumerator DisableRoutine()
    {
        // Only disable once per cycle
        if (isDisabled)
            yield break;

        isDisabled = true;
        timerRunning = false;
        gripTimer = 0f;

        // turn off collider
        selfCollider.enabled = false;

        // fade sprite
        SetOpacity(fadedOpacity);

        // If hand is still in collision, ungrip hands
        if (LhasCollided && networkPlayerMovement != null)
        {
            networkPlayerMovement.L_canGripJug = false;
            networkPlayerMovement.L_canGripCrimp = false;
            networkPlayerMovement.L_canGripPocket = false;
        }
        if (RhasCollided && networkPlayerMovement != null)
        {
            networkPlayerMovement.R_canGripJug = false;
            networkPlayerMovement.R_canGripCrimp = false;
            networkPlayerMovement.R_canGripPocket = false;
        }

        yield return new WaitForSeconds(disableDuration);

        // Reset values
        SetOpacity(originalOpacity);
        selfCollider.enabled = true;
        LhasCollided = false;
        RhasCollided = false;
        isDisabled = false;
    }

    private void SetOpacity(float value)
    {
        Color c = spriteRenderer.color;
        c.a = value;
        spriteRenderer.color = c;
    }
}
