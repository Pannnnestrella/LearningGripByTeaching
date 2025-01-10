using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class LetterTaskManager : MonoBehaviour
{
    public GripDataCollector gripDataCollector; // Hand gesture detector
    public RectTransform fillBar; // Progress bar fill
    public TMP_Text taskText; // Task goal text
    public TMP_Text statusText; // Real-time status text
    public TMP_Text timerText; // Countdown timer text
    public GameObject BobCanvas;
    public TMP_Text feedbackText; // Bob's Feedback
    public Button completeButton; // Button appears after all tasks are complete
    public Button nextButton; // Button to goto next task
    public Animator animator; // Animator, control the mask layer weight
    public GameObject pen; // adjust the pen with the process
    public Transform head1; // one end of the pen top
    public Transform head2; // Another end of the pen top
    public Transform end1; // one end of the pen tail
    public Transform end2; // Another pen tail
    public AudioClip taskFinishSound; // sound when task finished
    public float taskTimeLimit = 30f; // Time limit for each task
    private float timer = 0f; // Countdown timer for current task
    private bool isTaskActive = false; // Is the current task active, decide update
    private bool isHandGestureMatched = false; // Is the hand gesture matched

    private string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    private int totalTasks;
    private int currentTaskIndex = 0; // Index of the current task
    private int completedTasks = 0; // Number of successfully completed tasks
    private float originalWidth; // Original width of the progress bar

    private int lastTaskGroup = -1; // 记The task force that recorded the last animation update
    private string userID = "unknown_user"; // User ID used to save the file
    private AudioSource audioSource;
    private List<string> encouragementMessages = new List<string>
        {
            "Claire: Great job! You’re getting better and better!",
            "Claire: Amazing! Your friend is learning so much from you!",
            "Claire: Fantastic work! Keep it up!",
            "Claire: You’re such a great teacher!",
            "Claire: Wow! That was perfect!",
            "Claire: You did it! I’m so proud of you!",
            "Claire: Awesome! Your little friend is so impressed!",
            "Claire: Excellent! You’re a pro at this!",
            "Claire: Way to go! You’re doing amazing!",
            "Claire: Nice work! Your friend is learning fast!",
            "Claire: Hooray! Another task done perfectly!",
            "Claire: You’re unstoppable! Keep going!",
            "Claire: Terrific! Your practice is paying off!",
            "Claire: So cool! You’re a writing superstar!",
            "Claire: Great effort! You’re really making progress!",
            "Claire: You’re crushing it! Your little friend is so lucky!",
            "Claire: Brilliant! You’re a natural teacher!",
            "Claire: Keep it up! You’re doing an awesome job!",
            "Claire: Amazing progress! You’re almost there!",
            "Claire: Fantastic focus! Your friend is learning well!",
            "Claire: You’re rocking this! What a great teacher!",
            "Claire: Super work! You’re helping so much!",
            "Claire: Incredible! Your hard work is showing!",
            "Claire: Outstanding! You’re making a big difference!",
            "Claire: Wow, you’re so good at this! Keep it up!",
            "Claire: You did it again! Your friend is so proud of you!"
        };
    
    private List<string> studentFeedback = new List<string>
        {
            "Bob: I think I’m starting to get it! Thank you for showing me!",
            "Bob: Oh! I see how you’re doing it now. I’ll try my best to follow you!",
            "Bob: Wow, I’m really learning a lot from you! You’re a great teacher!",
            "Bob: Look! I’m doing it just like you now! You’re helping me so much!",
            "Bob: Yay! I think I’ve mastered it thanks to you! You’re the best teacher ever!",
            "Bob: Yay! I think I’ve mastered it thanks to you! You’re the best teacher ever!"
        };

    public void SetUserID(string id)
    {
        userID = gripDataCollector.GetUserID();
        Debug.Log($"UserID is set to: {userID}");
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = taskFinishSound;

        totalTasks = letters.Length;
        originalWidth = fillBar.sizeDelta.x;
        Debug.Log(originalWidth);

        if (gripDataCollector != null)
        {
            gripDataCollector.OnAllJointsMatched += OnHandGestureMatched;
        }   

        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(false); // Initially hide the Done button
            completeButton.onClick.AddListener(OnCompleteButtonClicked);
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false); //  Initially hide the next button
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        timerText?.gameObject.SetActive(false);
        
        // Each task is timed independently, so stagger
        Invoke(nameof(CallUpdateTaskTextWelcome1), 1f);
        Invoke(nameof(CallUpdateTaskTextWelcome2), 8f);
        Invoke(nameof(CallUpdateTaskTextWelcome3), 15f);
        // enter
        Invoke(nameof(StartNextTask), 20f);
    }
    void CallUpdateTaskTextWelcome1()
    {
        string welcome1 = "Hey there! Great job learning the correct way to hold your pencil—you’re amazing! But guess what? There’s a little friend who’s still having trouble learning it. Can you be a super teacher and help them out?";
        update_tasktext(welcome1);
    }
    void CallUpdateTaskTextWelcome2()
    {
        string welcome2 = "During the writing practice, make sure to use the correct pencil grip as much as you can. Your little friend will be watching closely and copying you! The more tasks you complete, the better your friend will learn and improve!";
        update_tasktext(welcome2);
    }

    void CallUpdateTaskTextWelcome3()
    {
        string welcome3 = "Are you ready? Let’s work hard together and start practicing. You’ve got this!";
        update_tasktext(welcome3);
    }

    void update_tasktext(string text){
        taskText.text = text;
    }
    void Update()
    {
        if (isTaskActive)
        {
            // Countdown Logic
            timer -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = $"{Mathf.CeilToInt(timer)}s";
            }
            // If time runs out, the mission fails
            if (timer <= 0)
            {
                FailCurrentTask();
            }
            else if(isHandGestureMatched){
                statusText.text = encouragementMessages[currentTaskIndex];
                
                CompleteCurrentTask();
            }
        }
    }

    void EvaluateCompletionRate()
    {
        Debug.Log("all tasks have been done");
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
            timerText.gameObject.SetActive(false); // hide the countdown text
        }
    }

    void SaveCompletionReport(float completionRate)
    {
        if (string.IsNullOrEmpty(userID))
        {
            Debug.LogWarning("UserID is not set, default value will be used unknown_user.");
            userID = "unknown_user";
        }

        string saveFolderPath = Path.Combine(Application.persistentDataPath, userID);
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string filePath = Path.Combine(saveFolderPath, "completion_report.json");

        // save data to json files
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
        timer = taskTimeLimit; // Reset Countdown
        isTaskActive = true;
        isHandGestureMatched = false;

        // Update the progress bar to determine the progress of the task
        
        Invoke(nameof(UpdateProgressBar),1f);

        // If all tasks are completed
        if (currentTaskIndex >= totalTasks)
        {
            isTaskActive = false;
            Debug.Log("All tasks completed and ready for assessment");
            nextButton.gameObject.SetActive(false); //hide the next button
            statusText.text = " ";
            taskText.text = "You’ve done such a great job, and your little friend has learned so much from you! Now, we’ll do a quick test. Don’t worry—it’s just to see how much you’ve improved.\n Are you ready? Let’s go and show how amazing you are!";
            // Lead the user to click the evaluate button
            completeButton.gameObject.SetActive(true);
            // finish
            return;
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true); // Display countdown text
            timerText.text = $"{Mathf.CeilToInt(timer)}s"; // Initialise countdown text
        }
        
        taskText.text = $"Task {currentTaskIndex + 1}/{totalTasks}: Write letter {letters[currentTaskIndex]}.";
        statusText.text = "Waiting for the correct gesture...";
    }

    void OnHandGestureMatched()
    {
        isHandGestureMatched = true;
    }

    void CompleteCurrentTask()
    {
        audioSource.Play();
        // Debug.Log("完成小任务");
        isTaskActive = false;
        completedTasks++;
        currentTaskIndex++;

        if(currentTaskIndex%5 == 0){
            // Play the celebration animation
            animator.SetBool("isSittingVictory",true);
        }
        if(currentTaskIndex >= totalTasks){
            animator.SetBool("isVictory",true); // Play the all-completed celebration animation
        }
        // Hide the countdown
        timerText.gameObject.SetActive(false);
        // The task is successful, the Next button is displayed, and the user clicks to the next
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }

    }
    void OnNextButtonClicked()
    {
        // The user cancels the animation by clicking next
        animator.SetBool("isSittingVictory",false);

        nextButton.gameObject.SetActive(false); // hide Next 
        if(BobCanvas != null){
            Debug.Log("BobDialog box is hidden");
            BobCanvas.gameObject.SetActive(false); // hide Bob‘s feedback
        }
        Invoke(nameof(StartNextTask), 0.5f);
    }

    void OnCompleteButtonClicked()
    {
        animator.SetBool("isVictory",false);
        // The user clicks on the Finish button to assess task completion.
        EvaluateCompletionRate();
    }

    void FailCurrentTask()
    {
        Debug.Log("任务失败");
        isTaskActive = false; // Failure of the mandate
        currentTaskIndex++; // Current task number plus 1
        taskText.text = "Task failed! Moving to the next task...";
        statusText.text = "";
        // Automatically jump to the next one if the task fails
        
        Invoke(nameof(StartNextTask), 2f);
    }
    void UpdateProgressBar()
    {
        if (fillBar == null)
        {
            Debug.LogError("fillBar is not assigned!");
            return;
        }
        float progress = (float)currentTaskIndex / totalTasks;
        Debug.Log($"Progress: {progress}, Current Task Index: {currentTaskIndex}, Total Tasks: {totalTasks}");

        // fillBar.sizeDelta = new Vector2(fillBar.sizeDelta.x * progress, fillBar.sizeDelta.y);
        fillBar.sizeDelta = new Vector2(200f * progress, fillBar.sizeDelta.y);

        // Update animation weights every 5 tasks
        int currentTaskGroup = currentTaskIndex / 5;

        // Changing the pen position at the completion of the second set
        if ( currentTaskGroup == 2){
            UpdatePositionAndRotation();
        }

        if (currentTaskGroup != lastTaskGroup)
        {
            float wrongWeight = Mathf.Lerp(1f, 0f, progress);
            float rightWeight = Mathf.Lerp(0f, 1f, progress);

            // 让bob给一点反馈
            Debug.Log("Bob's feedback");
            feedbackText.text = studentFeedback[currentTaskGroup];
            BobCanvas.gameObject.SetActive(true);

            animator.SetLayerWeight(animator.GetLayerIndex("wrong"), wrongWeight);
            animator.SetLayerWeight(animator.GetLayerIndex("right"), rightWeight);

            lastTaskGroup = currentTaskGroup;
        }
    }

    void UpdatePositionAndRotation()
    {
        // Panning distance (panning in direction)
        float offsetDistance = -0.05f;
        // Calculate the midpoint between head1 and head2
        Vector3 headMiddle = (head1.position + head2.position) / 2;
        // Calculate the midpoint between end1 and end2
        Vector3 endMiddle = (end1.position + end2.position) / 2;
        // Compute the direction vector from headMiddle to endMiddle.
        Vector3 direction = (headMiddle - endMiddle).normalized;

        // Set the position of the target object to headMiddle
        pen.transform.position = headMiddle + direction * offsetDistance;

        // Prevents errors if the length of the direction vector is 0.
        if (direction != Vector3.zero)
        {
            // Calculating Rotation with LookRotation
            Quaternion rotation = Quaternion.LookRotation(direction);
            // Setting the rotation of the target object
            pen.transform.rotation = rotation;
        }
        else
        {
            Debug.LogWarning("Direction vector is zero, cannot set rotation.");
        }
    }
}
