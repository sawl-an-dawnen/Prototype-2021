using UnityEngine;
using TMPro;
using System.Collections;

public class Clock : MonoBehaviour
{
    public Transform minute, hour;
    public float rotationSpeed = 200f;
    public float targetHourAngle = -30f; // Target angle for 10:30
    public float targetMinuteAngle = 180f; // Target angle for 10:30

    private bool isWinningConditionMet = false;
    private bool exiting = false;

    public TextMeshProUGUI resultText; // Reference to the UI Text component
    public GameObject panel;
    
    public void Resume()
	{
		panel.SetActive(false);
		Time.timeScale = 1f;
	}

	public void Pause()
	{
		panel.SetActive(true);
		Time.timeScale = 0f;
	}

    public IEnumerator returnToGame(float f)
    {
        yield return new WaitForSecondsRealtime(f);
        Resume();
    }

    private void Update()
    {
        if (panel.activeSelf && !isWinningConditionMet && !exiting)
		{
			Pause();
		}
        else if (!exiting)
        {
            exiting = true;
            StartCoroutine(returnToGame(2f));
        }

        if (!isWinningConditionMet)
        {
            if (Input.GetKey(KeyCode.E))
            {
                RotateClockwise();
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                RotateCounterclockwise();
            }

            // Check winning condition when pressing the "check" button (for example, Space key)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckWinningCondition();
            }
        }
    }

    private void RotateClockwise()
    {
        minute.Rotate(Vector3.back, rotationSpeed * Time.unscaledDeltaTime);
        hour.Rotate(Vector3.back, rotationSpeed * 0.083f * Time.unscaledDeltaTime);
    }

    private void RotateCounterclockwise()
    {
        minute.Rotate(Vector3.forward, rotationSpeed * Time.unscaledDeltaTime);
        hour.Rotate(Vector3.forward, rotationSpeed * 0.083f * Time.unscaledDeltaTime);
    }

    private void CheckWinningCondition()
    {
        float currentHourAngle = GetNormalizedAngle(hour.localEulerAngles.z);
        float currentMinuteAngle = GetNormalizedAngle(minute.localEulerAngles.z);

        float targetHourAngle = GetNormalizedAngle(43f); // Target angle for 10:30
        float targetMinuteAngle = GetNormalizedAngle(180f); // Target angle for 10:30

        if (Mathf.Abs(currentHourAngle - targetHourAngle) < 5f && Mathf.Abs(currentMinuteAngle - targetMinuteAngle) < 5f)
        {
            resultText.text = "Congratulations! This is the right time!";
            isWinningConditionMet = true;
        }
        else
        {
            resultText.text = "Not yet at the right time. Keep adjusting.";
            StartCoroutine(ClearResultTextAfterDelay(2f));
        }
    }

    private float GetNormalizedAngle(float angle)
    {
        // Adjust the angle to be in the range [0, 360)
        return (angle + 360f) % 360f;
    }

    private IEnumerator ClearResultTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resultText.text = "";
    }
}
