// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using UnityEngine.XR;
//
// public class ConserveGrips : MonoBehaviour
// {
//     public Transform rightHand; // 右手的Transform（手的骨骼或手部对象）
//     public string poseFileName = "hand_pose.json"; // 保存的文件名
//     private XRNode leftHandNode = XRNode.LeftHand; // 检测左手控制器的输入
//
//     void Update()
//     {
//         // 检查左手控制器的X按钮是否被按下
//         if (InputDevices.GetDeviceAtXRNode(leftHandNode).TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressed) && isPressed)
//         {
//             // 保存右手的错误握笔姿势
//             SaveHandPose("poor_grip");
//         }
//
//         if (InputDevices.GetDeviceAtXRNode(leftHandNode).TryGetFeatureValue(CommonUsages.secondaryButton, out bool isSecondaryPressed) && isSecondaryPressed)
//         {
//             // 保存右手的正确握笔姿势
//             SaveHandPose("correct_grip");
//         }
//     }
//
//     // 保存右手的手部姿势
//     public void SaveHandPose(string poseName)
//     {
//         HandPoseData poseData = new HandPoseData
//         {
//             position = rightHand.localPosition,
//             rotation = rightHand.localRotation,
//             poseName = poseName
//         };
//
//         string json = JsonUtility.ToJson(poseData, true);
//         string customPath = Path.Combine("/Users/star/Desktop", poseName + ".json"); // Windows上的桌面路径
//         File.WriteAllText(customPath, json);
//         Debug.Log("Right hand pose saved as: " + poseName + " at " + customPath);
//     }
// }
//
// // 定义一个结构体来保存手部姿势的数据
// [System.Serializable]
// public class HandPoseData
// {
//     public Vector3 position;
//     public Quaternion rotation;
//     public string poseName;
// }
//
