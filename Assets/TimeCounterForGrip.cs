using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LetterTaskManager : MonoBehaviour
{
    public GripDataCollector gripDataCollector; // Hand gesture detector
    public RectTransform fillBar; // Progress bar fill
    public TMP_Text taskText; // Task goal text
    public TMP_Text statusText; // Real-time status text
    public TMP_Text timerText; // Countdown timer text
    public Button completeButton; // Button appears after all tasks are complete
    public Button nextButton; // Button to goto next task
    public Animator animator; // Animator, control the mask layer weight

    public float taskTimeLimit = 30f; // Time limit for each task
    private float timer = 0f; // Countdown timer for current task
    private bool isTaskActive = false; // Is the current task active
    private bool isHandGestureMatched = false; // Is the hand gesture matched

    private string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    private int totalTasks;
    private int currentTaskIndex = 0; // Index of the current task
    private int completedTasks = 0; // Number of successfully completed tasks
    private float originalWidth; // Original width of the progress bar

    private int lastTaskGroup = -1; // 记录上一次动画更新的任务组
    private string userID = "unknown_user"; // 用于保存文件的用户ID

    public void SetUserID(string id)
    {
        userID = gripDataCollector.GetUserID();
        Debug.Log($"UserID 已设置为: {userID}");
    }

    void Start()
    {
        totalTasks = letters.Length;
        
        // originalWidth = fillBar.sizeDelta.x;
        // Debug.Log(originalWidth);

        if (gripDataCollector != null)
        {
            gripDataCollector.OnAllJointsMatched += OnHandGestureMatched;
        }   

        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(false); // 初始隐藏完成按钮
            completeButton.onClick.AddListener(OnCompleteButtonClicked);
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false); // 初始隐藏 Next 按钮
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        timerText?.gameObject.SetActive(false);
        
        taskText.text = "Welcome to the task! Form the letter with the correct grip gesture. Good luck!";
        Invoke(nameof(StartNextTask), 8f);
    }

    void Update()
    {
        if (isTaskActive)
        {
            // 倒计时逻辑
            timer -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = $"{Mathf.CeilToInt(timer)}s";
            }
            // 如果时间用完，任务失败
            if (timer <= 0)
            {
                FailCurrentTask();
            }
            else if(isHandGestureMatched){
                statusText.text = "Correct gesture detected!";
                Debug.Log("姿势匹配上了");
                // 用户在这个任务中匹配上了，完成当前任务
                CompleteCurrentTask();
            }
        }
    }

    void EvaluateCompletionRate()
    {
        Debug.Log("任务全部完成");
        isTaskActive = false;
        float completionRate = (float)completedTasks / totalTasks;
        string resultMessage;
        if (completionRate >= 0.8f)
        {
            resultMessage = "Congratulations! You completed 80% or more of the tasks!";
        }
        else
        {
            resultMessage = "You didn't complete 80% of the tasks. We couldn't teach the avatar the complete gestures.";
        }

        taskText.text = resultMessage;

        SaveCompletionReport(completionRate);

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false); // 隐藏倒计时文本
        }
    }

    void SaveCompletionReport(float completionRate)
    {
        // 确保 userID 已被设置
        if (string.IsNullOrEmpty(userID))
        {
            Debug.LogWarning("UserID 未设置，将使用默认值 unknown_user.");
            userID = "unknown_user";
        }

        // 确保文件夹路径存在
        string saveFolderPath = Path.Combine(Application.persistentDataPath, userID);
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string filePath = Path.Combine(saveFolderPath, "completion_report.json");

        // 保存数据为 JSON
        CompletionReport report = new CompletionReport
        {
            userID = userID,
            completionRate = completionRate,
            completedTasks = completedTasks,
            totalTasks = totalTasks
        };

        string json = JsonUtility.ToJson(report, true);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Completion report saved to {filePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save completion report: {e.Message}");
        }
    }

    [System.Serializable]
    public class CompletionReport
    {
        public string userID;
        public float completionRate;
        public int completedTasks;
        public int totalTasks;
    }

    void StartNextTask()
    {
        timer = taskTimeLimit; // 重置倒计时
        isTaskActive = true;
        isHandGestureMatched = false;

        // 更新进度条状态
        Debug.Log("更新进度条状态");
        Invoke(nameof(UpdateProgressBar),1f);

        // 如果全部任务都完成
        if (currentTaskIndex >= totalTasks)
        {
            Debug.Log("全部任务完成，准备评估");
            nextButton.gameObject.SetActive(false); // 隐藏 Next 按钮
            statusText.text = "All tasks finished, click the button to evaluate";
            // 引导用户点击评估按钮
            completeButton.gameObject.SetActive(true);
            // 然后结束
            return;
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true); // 显示倒计时文本
            timerText.text = $"{Mathf.CeilToInt(timer)}s"; // 初始化倒计时文本
        }
        
        taskText.text = $"Task {currentTaskIndex + 1}/{totalTasks}: Write letter {letters[currentTaskIndex]}.";
        statusText.text = "Waiting for the correct gesture...";
        // 交给update监控
    }

    void OnHandGestureMatched()
    {
        isHandGestureMatched = true;
    }

    void CompleteCurrentTask()
    {
        Debug.Log("完成了一个任务");
        isTaskActive = false;
        completedTasks++;
        currentTaskIndex++;

        // 隐藏倒计时
        timerText.gameObject.SetActive(false);

        statusText.text = "Task completed!";
        // 任务成功，显示 Next 按钮，用户点击到下一个
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }

    }
    void OnNextButtonClicked()
    {
        Debug.Log("用户点了next");
        nextButton.gameObject.SetActive(false); // 隐藏 Next 按钮
        // 开始下一个任务
        Invoke(nameof(StartNextTask), 1f);
    }

    void OnCompleteButtonClicked()
    {
        Debug.Log("用户点了完成按钮");
        // 用户点击了完成按钮,评估任务完成情况
        EvaluateCompletionRate();
    }

    void FailCurrentTask()
    {
        Debug.Log("任务失败");
        isTaskActive = false; // 任务失效
        currentTaskIndex++; // 当前任务序号加1
        taskText.text = "Task failed! Moving to the next task...";
        statusText.text = "";
        // 如果任务失败自动跳转下一个
        
        Invoke(nameof(StartNextTask), 2f);
    }
    void UpdateProgressBar()
    {
        originalWidth = fillBar.sizeDelta.y;
        if (fillBar == null) return;

        float progress = (float)currentTaskIndex / totalTasks;
        fillBar.sizeDelta = new Vector2(originalWidth * progress, fillBar.sizeDelta.y);

        // 每5个任务更新动画权重
        int currentTaskGroup = currentTaskIndex / 5;
        if (currentTaskGroup != lastTaskGroup)
        {
            float wrongWeight = Mathf.Lerp(1f, 0f, progress);
            float rightWeight = Mathf.Lerp(0f, 1f, progress);

            animator.SetLayerWeight(animator.GetLayerIndex("wrong"), wrongWeight);
            animator.SetLayerWeight(animator.GetLayerIndex("right"), rightWeight);

            lastTaskGroup = currentTaskGroup;
        }
    }
}
