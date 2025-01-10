using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour
{
    public TextMeshProUGUI instructionText;  
    public Animator avatarAnimator;  
    public GameObject button;  
    public Button ControllButton;
    private List<string> pages;             
    private int currentPageIndex = 0;      
    // private bool isButtonBEnabled = false;
    
    public bool isButtonBEnabled { get; private set; } = false;
    void Start()
    {
        // Intial text
        pages = new List<string>
        {
            "Hi,I'm Claire, welcome to our grip gesture class. Look at the board, let me show you the correct grip gesture today. "
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
         
            if (avatarAnimator != null)
            {
                avatarAnimator.SetTrigger("IsTeaching");  // Use the Trigger parameter to trigger an animation
            }
        }
        else
        {
            Debug.Log("No more pages.");
            instructionText.gameObject.SetActive(false);  // Set the textbox inactive
            button.SetActive(false);
            isButtonBEnabled = true;
            // ControllButton.interactable = true;
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