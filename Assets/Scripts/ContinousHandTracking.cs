using UnityEngine;
using System.IO;

public class ContinuousHandMatching : MonoBehaviour
{
    public GripDataCollector gripDataCollector; 
    private bool isMatching = false; 

    void Start()
    {
        if (gripDataCollector == null)
        {
            Debug.LogError("GripDataCollector is not attached！");
            return;
        }

        
        gripDataCollector.OnAllJointsMatched += HandleAllJointsMatched;
        gripDataCollector.OnJointMismatch += HandleJointMismatch;
    }

    void Update()
    {
        if (isMatching)
        {
            PerformMatching(); 
        }
    }

    public void ToggleMatching()
    {
        isMatching = !isMatching;
        
    }

    void PerformMatching()
    {
        if (gripDataCollector == null) return;

        // Collect real-time gesture data
        GripDataCollector.HandData currentHandData = gripDataCollector.CollectGripData(saveToFile: false);

        // Get standard gesture file path
        string standardFileName = gripDataCollector.GetGender() == "male" ? "standard_gesture_male" : "standard_gesture_female";
    
        // Enforcement comparison
        bool isMatched = gripDataCollector.CompareHandPose(currentHandData, standardFileName);

        if (isMatched)
        {
            Debug.Log("Real-time gesture matching is successful!");
        }
        else
        {
            Debug.Log("Real-time gesture matching fails!");
        }
    }


    // Event handler: called when all joints match
    private void HandleAllJointsMatched()
    {
        // Debug.Log(‘Event: all joints matched successfully!’) ;)
        // This is where you can extend the functionality further, such as updating the UI or triggering other logic.
    }

    // Event handler: called when a single joint doesn't match
    private void HandleJointMismatch(string jointName)
    {
        // Debug.Log($‘Event: joint {jointName} does not match!’) ;)
        // This is where you can extend the functionality further, such as logging or prompting the user with
    }

    void OnDestroy()
    {
        //write-off event to prevent memory leaks
        if (gripDataCollector != null)
        {
            gripDataCollector.OnAllJointsMatched -= HandleAllJointsMatched;
            gripDataCollector.OnJointMismatch -= HandleJointMismatch;
        }
    }
}
