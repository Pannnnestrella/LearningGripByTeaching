using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour
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
            "Could you teach him by showing the correct grip?",
            "If your grip is correct, Bob's grip will gradually change to the correct posture!",
            "Follow the tasks on the board and teach Bob how to hold the pen correctly as you complete each task!"
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