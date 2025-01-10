using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonBController : MonoBehaviour
{
    public Button buttonB; 
    public TMP_Text warningText; 
    public InstructionManager InstructionManager;
    public GameObject preTeachPanel; // PreTeach 
    public GameObject learningPanel; // Learning 
    public GameObject child; // character 
    // public GameObject nextTexts; // Next-text
    public Animator animator; 

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
            
            if (preTeachPanel != null)
                preTeachPanel.SetActive(false); // close

            if (learningPanel != null)
                learningPanel.SetActive(true); 

            if (child != null)
                child.SetActive(true); 
            // if (nextTexts != null)
            //     nextTexts.SetActive(true);

            // Add animation logic: switch to second animation
            if (animator != null)
            {
                animator.SetBool("IsWritting", true); 
                Debug.Log("Animation switches to ‘Writing’ state");

                // intialize Mask layer weight
                SetMaskLayerWeights("wrong", 1); // wrong weight is 1
                SetMaskLayerWeights("right", 0); // right weight is 0
            }
            else
            {
                Debug.LogError("Animator is not attached！");
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
            int layerIndex = animator.GetLayerIndex(layerName); 
            if (layerIndex < 0)
            {
                Debug.LogError($"Animation layer not found：{layerName}");
                return;
            }
            animator.SetLayerWeight(layerIndex, weight); 
        }
    }
}
