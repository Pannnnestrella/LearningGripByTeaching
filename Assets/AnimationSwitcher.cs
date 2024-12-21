using UnityEngine;

public class AnimationLayerSwitcher : MonoBehaviour
{
    public Animator animator; 

    public void SwitchToSecondAnimation()
    {
        if (animator != null)
        {
            // switch
            animator.SetBool("IsWritting", true); 
            Debug.Log("switch to the second animation");

            // initialize
            SetMaskLayerWeights("wrong", 1); // wrong layer is 1 at the beginning
            SetMaskLayerWeights("right", 0); // right layer is 0 at the beginning
        }
        else
        {
            Debug.LogError("Animator 未绑定！");
        }
    }

    private void SetMaskLayerWeights(string layerName, float weight)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        if (layerIndex < 0)
        {
            Debug.LogError($"未找到动画层：{layerName}");
            return;
        }
        animator.SetLayerWeight(layerIndex, weight);
    }
}
