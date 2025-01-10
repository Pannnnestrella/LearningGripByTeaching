using UnityEngine;
using TMPro;

public class NameInputHandler : MonoBehaviour
{
    private TMP_InputField inputField;

    void Awake()
    {
        
        inputField = GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            Debug.LogError("TMP_InputField component not found on this GameObject.");
        }
    }


    public void OnInputFieldSelect()
    {
        inputField.ActivateInputField();
    }

    
    public void OnInputValueChanged(string text)
    {
        Debug.Log("User input changed: " + text);
        
        string filteredText = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z\s]", "");
        if (filteredText != text)
        {
            inputField.text = filteredText;
            Debug.Log("Filtered non-alphabetic characters.");
        }
    }

  
    public void OnInputFieldEndEdit(string text)
    {
        Debug.Log("User finished editing: " + text);
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Input is empty. Please enter a name.");
        }
        else if (text.Length < 2)
        {
            Debug.LogWarning("Name is too short. Please enter at least 2 characters.");
        }
        else
        {
            Debug.Log("Name input accepted: " + text);
        }
    }
}