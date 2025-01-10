using UnityEngine;

public class CustomProgressBar : MonoBehaviour
{
    public RectTransform fillBar; 
    private float originalWidth;
    void Start()
    {
        if (fillBar != null)
        {
            originalWidth = fillBar.sizeDelta.x; 
            Debug.Log($"Original Width: {originalWidth}"); 
            fillBar.sizeDelta = new Vector2(0, fillBar.sizeDelta.y); 
        }
    }

    /// <summary>
    /// update
    /// </summary>
    /// <param name="progress">Current progress value, in the range 0 to 1</param>
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