using TMPro;
using UnityEngine;

public class NumericInputField : MonoBehaviour
{
    public TMP_InputField inputField; // 在 Unity 编辑器中拖拽 TMP_InputField 到此字段
    public TextMeshProUGUI warningText; // 显示提醒的文本对象

    void Start()
    {
        
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        // 添加监听器，检测输入变化
        inputField.onValueChanged.AddListener(ValidateInput);
    }

    // 只保留数字
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

        // 如果输入包含非数字字符，显示提醒
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
