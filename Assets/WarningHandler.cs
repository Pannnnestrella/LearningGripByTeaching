using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonBController : MonoBehaviour
{
    public Button buttonB; // 按钮 B
    public TMP_Text warningText; // 提示文本
    public InstructionManager InstructionManager;
    public GameObject preTeachPanel; // PreTeach 
    public GameObject learningPanel; // Learning 
    public GameObject nextButton; // Next-Button 
    public GameObject nextTexts;
    public Animator animator; // 动画控制器

    void Start()
    {
        // initialize: hide the texts
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    public void OnButtonBClicked()
    {
        if (!InstructionManager.isButtonBEnabled)
        {
            if (warningText != null)
            {
                warningText.gameObject.SetActive(true);
                Invoke("HideWarningText", 2f);
            }
        }
        else
        {
            Debug.Log("按钮 B 正常点击！");
            // 执行图中逻辑：控制面板和按钮显示状态
            if (preTeachPanel != null)
                preTeachPanel.SetActive(false); // close

            if (learningPanel != null)
                learningPanel.SetActive(true); 

            if (nextButton != null)
                nextButton.SetActive(true); 
            if (nextTexts != null)
                nextTexts.SetActive(true);

            // 添加动画逻辑：切换到第二个动画
            if (animator != null)
            {
                animator.SetBool("IsWritting", true); // 设置动画参数
                Debug.Log("动画切换到 'Writing' 状态");

                // 初始化 Mask 层权重
                SetMaskLayerWeights("wrong", 1); // wrong 层权重为 1
                SetMaskLayerWeights("right", 0); // right 层权重为 0
            }
            else
            {
                Debug.LogError("Animator 未绑定！");
            }
        }
    }

    private void HideWarningText()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    private void SetMaskLayerWeights(string layerName, float weight)
    {
        if (animator != null)
        {
            int layerIndex = animator.GetLayerIndex(layerName); // 获取层索引
            if (layerIndex < 0)
            {
                Debug.LogError($"未找到动画层：{layerName}");
                return;
            }
            animator.SetLayerWeight(layerIndex, weight); // 设置层权重
        }
    }
}
