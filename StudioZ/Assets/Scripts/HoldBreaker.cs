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

    // components you might want to disable (unused in this example, left for compatibility)
    public Behaviour[] componentsToDisable;

    // Shared timer state (single timer for both hands)
    private bool timerRunning = false;
    private float gripTimer = 0f;

    // Per-hand collision / state tracking - used to decide which hand(s) to modify when disabling
    private bool LhasCollided = false;
    private bool LalreadyLetGo = false; // true if left let go before disable
    private bool RhasCollided = false;
    private bool RalreadyLetGo = false; // true if right let go before disable

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

        // if timer is running, advance it regardless of release
        if (timerRunning)
        {
            gripTimer += Time.deltaTime;

            if (gripTimer >= gripRequiredTime)
            {
                // whichever hand(s) were holding this hold when the timer finishes
                // will be subject to having their canGrip flags cleared.
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

    // ---------- External API (kept for compatibility) ----------
    // original code used OnLCollision and StartRGrip / EndLGrip etc.
    // Provide both left and right entry points.

    public void OnLCollision()
    {
        if (isDisabled) return;

        LhasCollided = true;
        LalreadyLetGo = false;
    }

    public void OnRCollision()
    {
        if (isDisabled) return;

        RhasCollided = true;
        RalreadyLetGo = false;
    }

    // Called when left starts gripping (if you call this externally)
    public void StartLGrip()
    {
        if (isDisabled) return;

        // If left starts gripping while colliding, ensure collision state is true.
        // But main timer-start uses networkPlayerMovement.L_isGripping, so this is optional.
        LhasCollided = LhasCollided || true;
        // do not reset timers here; Update handles starting.
    }

    // Called when right starts gripping (original had StartRGrip)
    public void StartRGrip()
    {
        if (isDisabled) return;

        RhasCollided = RhasCollided || true;
    }

    // Called when left stops gripping
    public void EndLGrip()
    {
        // If left lets go, mark that left already let go so that when disabling occurs
        // we won't clear left's canGrip flags if they moved to another hold.
        LhasCollided = false;
        LalreadyLetGo = true;
        // Note: do not stop the shared timer here. Timer should continue once started.
    }

    // Called when right stops gripping
    public void EndRGrip()
    {
        RhasCollided = false;
        RalreadyLetGo = true;
    }

    // ---------- Disable routine ----------
    private IEnumerator DisableRoutine()
    {
        // guard to ensure we only disable once per cycle
        if (isDisabled)
            yield break;

        isDisabled = true;
        timerRunning = false; // stop counting further
        gripTimer = 0f;

        // turn off collider so no one can grab this hold while disabled
        selfCollider.enabled = false;

        // fade sprite
        SetOpacity(fadedOpacity);

        // Only clear the player's grip abilities for the hand(s) that were still holding this hold
        // at the moment the timer finished. This restores the original check behavior.
        // Left hand
        if (!LalreadyLetGo && networkPlayerMovement != null)
        {
            networkPlayerMovement.L_canGripJug = false;
            networkPlayerMovement.L_canGripCrimp = false;
            networkPlayerMovement.L_canGripPocket = false;
        }

        // Right hand
        if (!RalreadyLetGo && networkPlayerMovement != null)
        {
            networkPlayerMovement.R_canGripJug = false;
            networkPlayerMovement.R_canGripCrimp = false;
            networkPlayerMovement.R_canGripPocket = false;
        }

        yield return new WaitForSeconds(disableDuration);

        // re-enable
        SetOpacity(originalOpacity);
        selfCollider.enabled = true;

        // reset per-hand flags so future interactions behave normally
        LhasCollided = false;
        RhasCollided = false;
        LalreadyLetGo = false;
        RalreadyLetGo = false;

        isDisabled = false;
    }

    private void SetOpacity(float value)
    {
        Color c = spriteRenderer.color;
        c.a = value;
        spriteRenderer.color = c;
    }
}
