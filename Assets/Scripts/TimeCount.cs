using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isTiming = false;

    void Update()
    {
        if (timerText.gameObject.activeSelf)
        {
            isTiming = true;
        }
        else
        {
            isTiming = false;
        }

        // If the timer is running and has not reached 5 minutes, continue the timer.
        if (isTiming && elapsedTime < 300)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        // If 5 minutes are reached, stop the clock
        if (elapsedTime >= 300)
        {
            timerText.text = "05:00";
            isTiming = false;
        }
    }
}
