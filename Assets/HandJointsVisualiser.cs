using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandJointVisualizer : MonoBehaviour
{
    public GameObject handTrackingObject;  // 这里设置为 [BuildingBlock] Hand Tracking 对象
    public GameObject jointPrefab; // 一个小球体的预制体，用来表示关节

    private List<GameObject> jointVisuals = new List<GameObject>();
    private OVRHand ovrHand;
    private OVRSkeleton ovrSkeleton;

    void Start()
    {
        // 确保 OVRSkeleton 组件存在于场景中
        
        // 自动获取 OVRHand 和 OVRSkeleton 组件
        if (handTrackingObject != null)
        {
            ovrHand = handTrackingObject.GetComponentInChildren<OVRHand>();
            ovrSkeleton = handTrackingObject.GetComponentInChildren<OVRSkeleton>();
        }
        
        if (ovrHand == null || ovrSkeleton == null)
        {
            Debug.LogError("OVRHand or OVRSkeleton component is not found on the specified object.");
            return;
        }


        // 等待骨骼初始化
        StartCoroutine(InitializeJointVisuals());
    }

    System.Collections.IEnumerator InitializeJointVisuals()
    {
        // 等待骨骼数据准备好
        while (ovrSkeleton.Bones.Count == 0)
        {
            yield return null;
        }

        // 为每个关节创建一个可视化对象
        foreach (var bone in ovrSkeleton.Bones)
        {
            GameObject jointVisual = Instantiate(jointPrefab);
            jointVisual.transform.parent = this.transform; // 将它们作为 HandVisualizer 的子对象
            jointVisuals.Add(jointVisual);
        }
    }

    void Update()
    {
        if (ovrHand != null && ovrHand.IsTracked)
        {
            for (int i = 0; i < ovrSkeleton.Bones.Count; i++)
            {
                var bone = ovrSkeleton.Bones[i];
                jointVisuals[i].transform.position = bone.Transform.position;
                jointVisuals[i].SetActive(true);
            }
        }
        else
        {
            // 如果手未被追踪，将关节可视化对象隐藏或设置为默认位置
            foreach (var joint in jointVisuals)
            {
                joint.SetActive(false);
            }
        }
    }
}