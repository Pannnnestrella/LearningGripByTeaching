using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputManager : MonoBehaviour
{
    public InputField userIDInputField;  // 用户ID输入框
    public TMP_Dropdown genderDropdown;  // 性别选择Dropdown（male/female）
    public TMP_Text warningText;
    public Button submitButton; // 提交按钮
    public GameObject currentPanel; // 当前Panel
    public GameObject nextPanel; // 下一个Panel
    public GripDataCollector handDataCollector; // 负责保存手势数据的脚本

    public LetterTaskManager taskRecorder;

    private void Start()
    {
        // 初始化：隐藏提示文本
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        // 注册按钮点击事件
        submitButton.onClick.AddListener(OnSubmitButtonClicked);

        // 初始化性别Dropdown
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

        // 验证输入
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

        // 输入合法，隐藏当前Panel，显示下一个Panel
        Debug.Log($"用户ID合法: {userID}, 性别: {gender}");
        SwitchPanel();

        // 将合法的 userID 和 gender 传递给保存手势数据的脚本
        if (handDataCollector != null)
        {
            handDataCollector.SetUserID(userID); // 设置用户ID
            handDataCollector.SetGender(gender.ToLower()); // 设置性别（转为小写，"male"/"female"）
        }
        if (taskRecorder != null)
        {
            taskRecorder.SetUserID(userID); // 设置性别（转为小写，"male"/"female"）
        }
    }

    // 获取性别选择
    private string GetSelectedGender()
    {
        if (genderDropdown != null)
        {
            return genderDropdown.options[genderDropdown.value].text; // 获取Dropdown当前选项
        }
        return null;
    }

    // 显示提示信息
    private void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
        }
    }

    // 检查输入是否为数字
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

    // 切换Panel
    private void SwitchPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false); // 隐藏当前Panel
        }

        if (nextPanel != null)
        {
            nextPanel.SetActive(true); // 显示下一个Panel
        }
    }
}
