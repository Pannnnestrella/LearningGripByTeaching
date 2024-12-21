using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class InstructionManagerTwo : MonoBehaviour
{
    public TextMeshProUGUI instructionText;  // 引用 TextMeshPro 文本框
    public Animator avatarAnimator;  // 引用 Avatar 的 Animator 组件
    public GameObject button;  // 引用绑定的按钮对象
    public Button ControllButton;
    private List<string> pages;              // 存储每一页的文本
    private int currentPageIndex = 0;        // 当前页的索引
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

        // 显示第一页
        if (pages.Count > 0)
        {
            instructionText.text = pages[currentPageIndex];
        }
    }

    // 更新到下一页的文本和任务talk的动画
    public void ShowNextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            instructionText.text = pages[currentPageIndex];
            // 触发 Avatar 动画
            if (avatarAnimator != null)
            {
                avatarAnimator.SetTrigger("IsTeaching");  // 使用 Trigger 参数来触发动画
            }
        }
        else
        {
            Debug.Log("No more pages.");
            instructionText.gameObject.SetActive(false);  // 设置文本框 inactive
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