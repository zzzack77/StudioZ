using UnityEngine;
using System.Collections;

public class HoldBreaker : MonoBehaviour
{
    private int holdIndex = -1;
    private NetworkPlayerMovement networkPlayerMovement;

    [Header("Settings")]
    [SerializeField] private float gripTime = 2f;
    [SerializeField] private float disableDuration = 5f;
    [SerializeField] private float fadedOpacity = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Collider selfCollider;
    private float originalOpacity;
    private Animator Animator;

    private bool isDisabled = false;
    private bool LhasCollided = false;
    private bool RhasCollided = false;

     private bool timerRunning = false;
     private float gripTimer = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        selfCollider = GetComponent<Collider>();
        originalOpacity = spriteRenderer.color.a;
        Animator = GetComponent<Animator>();

    }

    private void Start()
    {
        // register this hold with the manager
        if (GripBreakerManager.Instance != null) holdIndex = GripBreakerManager.Instance.RegisterHold(this);
        else Debug.LogWarning("GripBreakerManager is not currently in this scene, if you want multiplayer gripbreaker add GripBreakerManager Script to the level root.");
    }

    private void Update()
    {
        if (networkPlayerMovement == null || isDisabled)
            return;

        if (!timerRunning)
        {
            if ((LhasCollided && networkPlayerMovement.L_isGripping) ||
                (RhasCollided && networkPlayerMovement.R_isGripping))
            {
                if (gripTime < 1f)
                {
                    //Set Ani Bool active
                    Animator.SetBool("IsBShake", true);
                }
                else
                {
                    //Set Ani Bool active
                    Animator.SetBool("IsShake", true);
                }


                timerRunning = true;
                gripTimer = 0f;
            }
        }

        if (timerRunning)
        {
            gripTimer += Time.deltaTime;

            if (gripTimer >= gripTime)
            {
                if (GripBreakerManager.Instance != null) GripBreakerManager.Instance.RequestDisableHoldServerRpc(holdIndex);
                else StartCoroutine(DisableRoutine());
                timerRunning = false;
            }
        }
    }

    // Called by manager on all clients
    public void TriggerDisable()
    {
        if (!gameObject.activeInHierarchy)
            return;

        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        isDisabled = true;
        timerRunning = false;

        Animator.SetBool("IsBShake", false);
        Animator.SetBool("IsShake", false);

        selfCollider.enabled = false;
        SetOpacity(fadedOpacity);

        // Ungrip if hand was inside
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

        // re-enable
        SetOpacity(originalOpacity);
        selfCollider.enabled = true;

        isDisabled = false;
        LhasCollided = false;
        RhasCollided = false;
    }

    // Player reference
    public void GetPlayerReference(NetworkPlayerMovement player)
    {
        networkPlayerMovement = player;
    }

    public void OnLCollision() { if (!isDisabled) LhasCollided = true; }
    public void OnRCollision() { if (!isDisabled) RhasCollided = true; }
    public void EndLGrip() { LhasCollided = false; }
    public void EndRGrip() { RhasCollided = false; }

    private void SetOpacity(float v)
    {
        Color c = spriteRenderer.color;
        c.a = v;
        spriteRenderer.color = c;
    }
}
