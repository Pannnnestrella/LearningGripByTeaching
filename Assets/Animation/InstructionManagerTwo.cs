using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class InstructionManagerTwo : MonoBehaviour
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
            "Today, your friend Bob will learn the correct grip gesture with you together.",
            "Let’start! Look at the tutorial on the right board.",
            "If you fully understand it, press on the button which shows 'I can do it'.",
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
            // trig the animation
            if (avatarAnimator != null)
            {
                avatarAnimator.SetTrigger("IsTeaching"); 
            }
        }
        else
        {
            Debug.Log("No more pages.");
            instructionText.gameObject.SetActive(false);  // text inactive
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