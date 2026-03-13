using System.Collections;
using TMPro;
using UnityEngine;

// This script is to be put on a TextMeshPro UI GameObject

public class MatchCountdown : MonoBehaviour
{
    private bool isStartOfCountdown = true; // Tracks if it is the start of the countdown
    private TextMeshProUGUI countdownText;
    private void OnEnable()
    {
        GameModeBase.OnCountdownUpdated += UpdateCountdownText; // Recieves an event that triggers values: 3,2,1,0
    }

    private void OnDisable()
    {
        GameModeBase.OnCountdownUpdated -= UpdateCountdownText;
    }
    private void Awake()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        countdownText.enabled = false;
    }


    private void UpdateCountdownText(float countdownTime)
    {
        // Sets the text to be active only once
        if (isStartOfCountdown)
        {
            countdownText.text = countdownTime.ToString();
            countdownText.enabled = true;
            isStartOfCountdown = !isStartOfCountdown;
        }
        countdownText.text = countdownTime.ToString();
        
        // If the time is 0 then show the "Go!" text via a Coroutine
        if (countdownTime <= 0)
        {
            StartCoroutine(ShowGoText());
        }
        
    }

    private IEnumerator ShowGoText()
    {
        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        countdownText.enabled = false;
    }
}
