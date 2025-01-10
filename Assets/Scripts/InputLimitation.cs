using TMPro;
using UnityEngine;

public class NumericInputField : MonoBehaviour
{
    public TMP_InputField inputField; 
    public TextMeshProUGUI warningText; // Display text objects for reminders

    void Start()
    {
        
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        // Add a listener to detect input changes
        inputField.onValueChanged.AddListener(ValidateInput);
    }

    // numeric
    private void ValidateInput(string input)
    {
        bool hasNonNumeric = false; 
        string numericInput = "";

        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                numericInput += c;
            }
            else
            {
                hasNonNumeric = true;
            }
        }

        // If the input contains non-numeric characters, a reminder is displayed
        if (hasNonNumeric)
        {
            if (warningText != null)
            {
                warningText.text = "Please ensure your ID is numeric！";
                warningText.gameObject.SetActive(true);
            }
        }
        else
        {
            
            if (warningText != null)
            {
                warningText.gameObject.SetActive(false);
            }
        }

        
        if (input != numericInput)
        {
            inputField.text = numericInput;
        }
    }

    void OnDestroy()
    {
       
        inputField.onValueChanged.RemoveListener(ValidateInput);
    }
}
