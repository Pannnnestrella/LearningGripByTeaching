using UnityEngine;

public class AnimatorHierarchyDebug : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // 获取角色根节点上的 Animator 组件
        animator = GetComponent<Animator>();

        // 检查 Animator 是否存在
        if (animator != null)
        {
            Debug.Log("Animator found on: " + gameObject.name);
            Debug.Log("Animator Root: " + animator.transform.name);

            // 遍历 Animator 根节点的所有子节点
            TraverseHierarchy(animator.transform);
        }
        else
        {
            Debug.LogError("Animator component not found on this object!");
        }
    }

    // 递归遍历层次结构并输出每个子节点的名称
    void TraverseHierarchy(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Debug.Log("Child found: " + child.name);

            // 递归遍历每个子对象的子节点
            TraverseHierarchy(child);
        }
    }
}