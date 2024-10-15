using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class PoseSaver : MonoBehaviour
{
    public Transform handRoot; // 虚拟代理手部的根Transform
    public Transform poseContainer; // 用于保存姿势的容器，例如 PoorPose 或 ReferencePose

    public void SavePose()
    {
        if (handRoot == null || poseContainer == null)
        {
            Debug.LogError("Hand root or pose container is not set!");
            return;
        }

        // 删除之前的所有子对象
        foreach (Transform child in poseContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        // 递归复制handRoot及其所有子对象
        CopyTransformsRecursively(handRoot, poseContainer);
    }

    private void CopyTransformsRecursively(Transform source, Transform destination)
    {
        foreach (Transform child in source)
        {
            // 创建一个新的空的GameObject
            GameObject newChild = new GameObject(child.name);
            newChild.transform.SetParent(destination);

            // 复制位置和旋转
            newChild.transform.localPosition = child.localPosition;
            newChild.transform.localRotation = child.localRotation;
            newChild.transform.localScale = child.localScale;

            // 递归处理子对象
            CopyTransformsRecursively(child, newChild.transform);
        }
    }
}

