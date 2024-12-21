using UnityEngine;

public class CustomProgressBar : MonoBehaviour
{
    public RectTransform fillBar; // 进度条填充部分的 RectTransform
    private float originalWidth; // 填充条的最大宽度

    void Start()
    {
        // 获取进度条的初始宽度
        if (fillBar != null)
        {
            originalWidth = fillBar.sizeDelta.x; // 背景条的宽度
            Debug.Log($"Original Width: {originalWidth}"); 
            fillBar.sizeDelta = new Vector2(0, fillBar.sizeDelta.y); // 初始化填充条为 0 宽度
        }
    }

    /// <summary>
    /// update
    /// </summary>
    /// <param name="progress">当前进度值，范围为 0 到 1</param>
    public void UpdateProgress(float progress)
    {
        // limit progress between 0 and 1 
        progress = Mathf.Clamp01(progress);

        // update the width
        if (fillBar != null)
        {
            fillBar.sizeDelta = new Vector2(originalWidth * progress, fillBar.sizeDelta.y);
            Debug.Log($"Progress: {progress}, New Width: {originalWidth * progress}");
        }
    }
}