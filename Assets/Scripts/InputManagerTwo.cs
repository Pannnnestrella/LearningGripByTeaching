using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InputManagerTwo : MonoBehaviour
{
    public TextMeshProUGUI instructionText;  
    public Animator avatarAnimator;  
    private List<string> pages;             
    private int currentPageIndex = 0;        
    
    

    void Start()
    {
        // Intial text
        pages = new List<string>
        {
            "Could you teach him by showing the correct grip?",
            "If your grip is correct, Bob's grip will gradually change to the correct posture!",
            "Follow the tasks on the board and teach Bob how to hold the pen correctly as you complete each task!"
        };

        // the first page
        if (pages.Count > 0)
        {
            instructionText.text = pages[currentPageIndex];
        }
    }

    // Update to next page of text and tasktalk animation
    public void ShowNextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            instructionText.text = pages[currentPageIndex];
            // animation
            if (avatarAnimator != null)
            {
                avatarAnimator.SetTrigger("IsTeaching");  // Use the Trigger parameter to trigger an animation
            }
        }
        else
        {
            Debug.Log("No more pages.");
            instructionText.gameObject.SetActive(false);  // Set the textbox inactive
        }
    }

    // previous text
    public void ShowPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            instructionText.text = pages[currentPageIndex];
        }
        else
        {
            Debug.Log("This is the first page.");
        }
    }
}