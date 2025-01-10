using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputManager : MonoBehaviour
{
    public InputField userIDInputField;  
    public TMP_Dropdown genderDropdown;  
    public TMP_Text warningText;
    public Button submitButton; 
    public GameObject currentPanel; 
    public GameObject nextPanel;
    public GripDataCollector handDataCollector;

    public LetterTaskManager taskRecorder;

    private void Start()
    {
        
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

       
        submitButton.onClick.AddListener(OnSubmitButtonClicked);

        
        if (genderDropdown != null)
        {
            genderDropdown.ClearOptions();
            genderDropdown.AddOptions(new System.Collections.Generic.List<string> { "Male", "Female" });
        }
    }

    private void OnSubmitButtonClicked()
    {
        string userID = userIDInputField.text;
        string gender = GetSelectedGender();

       
        if (string.IsNullOrEmpty(userID))
        {
            ShowWarning("UserID shouldn't be empty!");
            return;
        }

        if (!IsInputNumeric(userID))
        {
            ShowWarning("UserID should be numbers!");
            return;
        }

        if (string.IsNullOrEmpty(gender))
        {
            ShowWarning("Please select a gender!");
            return;
        }

        // Input is legal, hide the current panel and show the next one.
        Debug.Log($"use id is legal: {userID}, gender: {gender}");
        SwitchPanel();

        // Pass the legal userID and gender to the script that holds the gesture data.
        if (handDataCollector != null)
        {
            handDataCollector.SetUserID(userID); // set user ID
            handDataCollector.SetGender(gender.ToLower()); // set gender（，"male"/"female"）
        }
        if (taskRecorder != null)
        {
            taskRecorder.SetUserID(userID); 
        }
    }

    // get gender
    private string GetSelectedGender()
    {
        if (genderDropdown != null)
        {
            return genderDropdown.options[genderDropdown.value].text; 
        }
        return null;
    }

    // show info 
    private void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
        }
    }

    // check if the input are numbers
    private bool IsInputNumeric(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    // change Panel
    private void SwitchPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false); // hide present Panel
        }

        if (nextPanel != null)
        {
            nextPanel.SetActive(true); // show next panel
        }
    }
}
