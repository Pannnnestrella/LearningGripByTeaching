using UnityEngine;
using TMPro;

public class NameInputHandler : MonoBehaviour
{
    private TMP_InputField inputField;

    void Awake()
    {
        // 自动获取 TMP_InputField 组件
        inputField = GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            Debug.LogError("TMP_InputField component not found on this GameObject.");
        }
    }

    // 当用户选择输入字段时，自动激活输入
    public void OnInputFieldSelect()
    {
        inputField.ActivateInputField();
    }

    // 实时处理用户输入
    public void OnInputValueChanged(string text)
    {
        Debug.Log("User input changed: " + text);
        // 过滤逻辑，先没完善
        string filteredText = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z\s]", "");
        if (filteredText != text)
        {
            inputField.text = filteredText;
            Debug.Log("Filtered non-alphabetic characters.");
        }
    }

    // 当用户完成输入后验证内容
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