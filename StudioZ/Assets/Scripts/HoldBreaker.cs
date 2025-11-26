using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class HoldBreaker : NetworkBehaviour
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

    // Is either hand currently in the collision box (local flags)
    private bool LhasCollided = false;
    private bool RhasCollided = false;

    // local disabled flag (mirrors networked state)
    private bool isDisabled = false;

    // Network-synced disabled state
    private NetworkVariable<bool> isDisabledNet = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        selfCollider = GetComponent<Collider>();
        originalOpacity = spriteRenderer.color.a;
    }

    private void OnEnable()
    {
        isDisabledNet.OnValueChanged += OnDisabledChanged;
    }

    private void OnDisable()
    {
        isDisabledNet.OnValueChanged -= OnDisabledChanged;
    }

    // Called on all clients when server changes isDisabledNet
    private void OnDisabledChanged(bool oldVal, bool newVal)
    {
        // Mirror network state locally
        isDisabled = newVal;

        if (newVal)
        {
            // Server says: disabled. Clients should update visuals and run local disable actions.
            SetOpacity(fadedOpacity);
            selfCollider.enabled = false;

            // Run the local disable actions that used to be in DisableRoutine (no re-enable here).
            RunLocalDisableActions();
        }
        else
        {
            // Server says: enabled again. Clients re-enable visuals and reset local collision flags.
            SetOpacity(originalOpacity);
            selfCollider.enabled = true;

            // Reset local flags so the hold can be gripped again
            LhasCollided = false;
            RhasCollided = false;
            timerRunning = false;
            gripTimer = 0f;
        }
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
                // Request server to disable. Only call once per timer cycle.
                RequestDisableServerRpc();
                // stop local timer so we don't spam RPCs while waiting for network update
                timerRunning = false;
                gripTimer = 0f;
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

    // Client -> Server: ask server to start disable cycle
    [ServerRpc(RequireOwnership = false)]
    private void RequestDisableServerRpc(ServerRpcParams rpcParams = default)
    {
        // Server starts the authoritative disable routine if not already disabled
        if (!isDisabledNet.Value)
        {
            StartCoroutine(ServerDisableRoutine());
        }
    }

    // Server-side authoritative disable (controls duration and network variable)
    private IEnumerator ServerDisableRoutine()
    {
        isDisabledNet.Value = true;

        yield return new WaitForSeconds(disableDuration);

        isDisabledNet.Value = false;
    }

    // Local actions that used to happen in DisableRoutine; executed on each client when server tells them the hold is disabled.
    // Note: this routine does NOT re-enable visuals — the server controls the re-enable via isDisabledNet.
    private void RunLocalDisableActions()
    {
        // Only run if not already run locally
        if (isDisabled == false)
            return;

        // Stop/clear local timer(s)
        timerRunning = false;
        gripTimer = 0f;

        // If hand is still in collision, clear local grip allowances so player ungrips similar to previous behavior
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

        // Clear local collision flags so the hold won't be considered colliding locally while disabled
        LhasCollided = false;
        RhasCollided = false;

        // Local 'isDisabled' already set by OnDisabledChanged.
    }

    private void SetOpacity(float value)
    {
        Color c = spriteRenderer.color;
        c.a = value;
        spriteRenderer.color = c;
    }
}
