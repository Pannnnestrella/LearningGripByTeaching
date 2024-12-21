using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isTiming = false;

    void Update()
    {
        // 检查文本框是否被激活
        if (timerText.gameObject.activeSelf)
        {
            isTiming = true;
        }
        else
        {
            isTiming = false;
        }

        // 如果正在计时且未达到5分钟，继续计时
        if (isTiming && elapsedTime < 300)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        // 如果到达5分钟，停止计时
        if (elapsedTime >= 300)
        {
            timerText.text = "05:00";
            isTiming = false;
        }
    }
}
