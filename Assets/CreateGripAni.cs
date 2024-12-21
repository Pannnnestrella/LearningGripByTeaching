// using UnityEngine;
// using System.IO;
// using System.Collections.Generic;
// using UnityEditor;
//
// public class StaticPoseGenerator : MonoBehaviour
// {
//     [Header("Settings for character")]
//     public Transform rootBone; // 角色根骨骼
//     public string outputClipName = "StaticPose"; // 导出的静态动画名称
//
//     private Dictionary<string, string> boneMapping; // 骨骼映射表
//
//     [System.Serializable]
//     public class PoseData
//     {
//         public JointData[] joints; // 对应 JSON 中的 "joints" 数组
//     }
//
//     [System.Serializable]
//     public class JointData
//     {
//         public string jointName;       // 对应 "jointName"
//         public string parentJointName; // 对应 "parentJointName"
//         public Position position;      // 对应 "position"
//         public Rotation rotation;      // 对应 "rotation"
//     }
//
//     [System.Serializable]
//     public class Position
//     {
//         public float x; // 对应 "position.x"
//         public float y; // 对应 "position.y"
//         public float z; // 对应 "position.z"
//     }
//
//     [System.Serializable]
//     public class Rotation
//     {
//         public float x; // 对应 "rotation.x"
//         public float y; // 对应 "rotation.y"
//         public float z; // 对应 "rotation.z"
//         public float w; // 对应 "rotation.w"
//     }
//
//     [ContextMenu("Generate Static Pose")]
//     public void GenerateStaticPose()
//     {
//         // JSON 文件路径
//         string stdFilePath = "/Users/star/unityProject/LearningPenGrip/Assets/standard_1.json";
//
//         // 检查文件是否存在
//         if (!File.Exists(stdFilePath))
//         {
//             Debug.LogError($"JSON 文件未找到：{stdFilePath}");
//             return;
//         }
//
//         string jsonContent = File.ReadAllText(stdFilePath);
//         PoseData poseData = JsonUtility.FromJson<PoseData>(jsonContent);
//
//         if (poseData == null || poseData.joints == null || poseData.joints.Length == 0)
//         {
//             Debug.LogError("JSON 数据格式错误或内容为空！");
//             return;
//         }
//
//         // 创建骨骼映射表
//         CreateBoneMapping();
//
//         // 创建静态姿势动画
//         AnimationClip clip = CreatePoseAnimation(poseData);
//
//         // 检查并创建输出目录
//         string outputDirectory = "Assets/Animation/";
//         if (!Directory.Exists(outputDirectory))
//         {
//             Directory.CreateDirectory(outputDirectory);
//         }
//
//         // 保存动画片段
// #if UNITY_EDITOR
//         string path = Path.Combine(outputDirectory, $"{outputClipName}.anim");
//         UnityEditor.AssetDatabase.CreateAsset(clip, path);
//         UnityEditor.AssetDatabase.SaveAssets();
//         Debug.Log($"静态姿势动画已保存为 {path}");
// #else
//         Debug.LogError("生成动画只能在编辑器中运行！");
// #endif
//     }
//
//      private void CreateBoneMapping()
//      {
//          boneMapping = new Dictionary<string, string>()
//          {
//              // Wrist and Forearm
//              { "Hand_WristRoot", "mixamorig:RightHand" },
//              { "Hand_ForearmStub", "mixamorig:RightForeArm" },
//      
//              // Thumb
//              { "Hand_Thumb0", "mixamorig:RightHandThumb1" },
//              { "Hand_Thumb1", "mixamorig:RightHandThumb2" },
//              { "Hand_Thumb2", "mixamorig:RightHandThumb3" },
//              { "Hand_Thumb3", "mixamorig:RightHandThumb4" },
//      
//              // Index Finger
//              { "Hand_Index1", "mixamorig:RightHandIndex1" },
//              { "Hand_Index2", "mixamorig:RightHandIndex2" },
//              { "Hand_Index3", "mixamorig:RightHandIndex3" },
//      
//              // Middle Finger
//              { "Hand_Middle1", "mixamorig:RightHandMiddle1" },
//              { "Hand_Middle2", "mixamorig:RightHandMiddle2" },
//              { "Hand_Middle3", "mixamorig:RightHandMiddle3" },
//      
//              // Ring Finger
//              { "Hand_Ring1", "mixamorig:RightHandRing1" },
//              { "Hand_Ring2", "mixamorig:RightHandRing2" },
//              { "Hand_Ring3", "mixamorig:RightHandRing3" },
//      
//              // Pinky Finger
//              { "Hand_Pinky0", "mixamorig:RightHandPinky1" },
//              { "Hand_Pinky1", "mixamorig:RightHandPinky2" },
//              { "Hand_Pinky2", "mixamorig:RightHandPinky3" },
//              { "Hand_Pinky3", "mixamorig:RightHandPinky4" },
//      
//              // // Tips (Optional, if needed for finer animations)
//              // { "Hand_ThumbTip", "mixamorig:RightHandThumbTip" },
//              // { "Hand_IndexTip", "mixamorig:RightHandIndexTip" },
//              // { "Hand_MiddleTip", "mixamorig:RightHandMiddleTip" },
//              // { "Hand_RingTip", "mixamorig:RightHandRingTip" },
//              // { "Hand_PinkyTip", "mixamorig:RightHandPinkyTip" }
//          };
//      }
//
//     private AnimationClip CreatePoseAnimation(PoseData poseData)
//     {
//         AnimationClip clip = new AnimationClip();
//         clip.legacy = false; // 使用 Mecanim 系统
//
//         foreach (var joint in poseData.joints)
//         {
//             // 检查是否在映射表中
//             if (!boneMapping.TryGetValue(joint.jointName, out string mappedBoneName))
//             {
//                 Debug.LogWarning($"未在映射表中的骨骼: {joint.jointName}，跳过该骨骼。");
//                 continue; // 跳过未映射的骨骼
//             }
//
//             // 查找骨骼
//             Transform bone = FindBoneByName(rootBone, mappedBoneName);
//             if (bone == null)
//             {
//                 Debug.LogWarning($"映射表中的骨骼未在模型中找到: {mappedBoneName}，跳过该骨骼。");
//                 continue; // 跳过模型中不存在的骨骼
//             }
//
//             // 生成动画曲线
//             string path = GetBonePath(rootBone, bone);
//
//             Debug.Log($"骨骼路径: {path}，名称: {mappedBoneName}");
//             Debug.Log($"旋转数据: (x: {joint.rotation.x}, y: {joint.rotation.y}, z: {joint.rotation.z}, w: {joint.rotation.w})");
//
//             Quaternion rotation = new Quaternion(
//                 joint.rotation.x,
//                 joint.rotation.y,
//                 joint.rotation.z,
//                 joint.rotation.w
//             );
//
//             AnimationCurve curveX = AnimationCurve.Constant(0, 1 / 60f, rotation.x);
//             AnimationCurve curveY = AnimationCurve.Constant(0, 1 / 60f, rotation.y);
//             AnimationCurve curveZ = AnimationCurve.Constant(0, 1 / 60f, rotation.z);
//             AnimationCurve curveW = AnimationCurve.Constant(0, 1 / 60f, rotation.w);
//
//             clip.SetCurve(path, typeof(Transform), "localRotation.x", curveX);
//             clip.SetCurve(path, typeof(Transform), "localRotation.y", curveY);
//             clip.SetCurve(path, typeof(Transform), "localRotation.z", curveZ);
//             clip.SetCurve(path, typeof(Transform), "localRotation.w", curveW);
//         }
//
//         return clip;
//     }
//
//
//     private string GetBonePath(Transform root, Transform bone)
//     {
//         if (bone == root) return "";
//         return GetBonePath(root, bone.parent) + "/" + bone.name;
//     }
//
//     private Transform FindBoneByName(Transform root, string name)
//     {
//         if (root.name == name) return root;
//         foreach (Transform child in root)
//         {
//             Transform found = FindBoneByName(child, name);
//             if (found != null) return found;
//         }
//         return null;
//     }
// }

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

public class StaticPoseGenerator : MonoBehaviour
{
    [Header("Settings for character")]
    public Transform rootBone; // 角色根骨骼
    public string outputClipName = "StaticPose"; // 导出的静态动画名称

    private Dictionary<string, string> boneMapping; // 骨骼映射表

    [System.Serializable]
    public class PoseData
    {
        public JointData[] joints; // 对应 JSON 中的 "joints" 数组
    }

    [System.Serializable]
    public class JointData
    {
        public string jointName;       // 对应 "jointName"
        public Position position;      // 对应 "position"
        public Rotation rotation;      // 对应 "rotation"
    }

    [System.Serializable]
    public class Position
    {
        public float x; // 对应 "position.x"
        public float y; // 对应 "position.y"
        public float z; // 对应 "position.z"
    }

    [System.Serializable]
    public class Rotation
    {
        public float x; // 对应 "rotation.x"
        public float y; // 对应 "rotation.y"
        public float z; // 对应 "rotation.z"
        public float w; // 对应 "rotation.w"
    }

    [ContextMenu("Generate Static Pose")]
    public void GenerateStaticPose()
    {
        // JSON 文件路径
        string stdFilePath = "/Users/star/unityProject/LearningPenGrip/Assets/standard_1.json";

        // 检查文件是否存在
        if (!File.Exists(stdFilePath))
        {
            Debug.LogError($"JSON 文件未找到：{stdFilePath}");
            return;
        }

        string jsonContent = File.ReadAllText(stdFilePath);
        PoseData poseData = JsonUtility.FromJson<PoseData>(jsonContent);

        if (poseData == null || poseData.joints == null || poseData.joints.Length == 0)
        {
            Debug.LogError("JSON 数据格式错误或内容为空！");
            return;
        }

        // 创建骨骼映射表
        CreateBoneMapping();

        // 创建静态姿势动画
        AnimationClip clip = CreatePoseAnimation(poseData);

        // 检查并创建输出目录
        string outputDirectory = "Assets/Animation/";
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // 保存动画片段
#if UNITY_EDITOR
        string path = Path.Combine(outputDirectory, $"{outputClipName}_Humanoid.anim");
        UnityEditor.AssetDatabase.CreateAsset(clip, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"Humanoid 静态姿势动画已保存为 {path}");
#else
        Debug.LogError("生成动画只能在编辑器中运行！");
#endif
    }

    private void CreateBoneMapping()
    {
        boneMapping = new Dictionary<string, string>()
        {
            // Wrist and Forearm
            { "Hand_WristRoot", "mixamorig:RightHand" },

            // Thumb
            { "Hand_Thumb1", "mixamorig:RightHandThumb1" },
            { "Hand_Thumb2", "mixamorig:RightHandThumb2" },
            { "Hand_Thumb3", "mixamorig:RightHandThumb3" },

            // Index Finger
            { "Hand_Index1", "mixamorig:RightHandIndex1" },
            { "Hand_Index2", "mixamorig:RightHandIndex2" },
            { "Hand_Index3", "mixamorig:RightHandIndex3" },

            // Middle Finger
            { "Hand_Middle1", "mixamorig:RightHandMiddle1" },
            { "Hand_Middle2", "mixamorig:RightHandMiddle2" },
            { "Hand_Middle3", "mixamorig:RightHandMiddle3" },

            // Ring Finger
            { "Hand_Ring1", "mixamorig:RightHandRing1" },
            { "Hand_Ring2", "mixamorig:RightHandRing2" },
            { "Hand_Ring3", "mixamorig:RightHandRing3" },

            // Pinky Finger
            { "Hand_Pinky1", "mixamorig:RightHandPinky1" },
            { "Hand_Pinky2", "mixamorig:RightHandPinky2" },
            { "Hand_Pinky3", "mixamorig:RightHandPinky3" },
            { "Hand_Pinky4", "mixamorig:RightHandPinky4" }
        };
    }

    private AnimationClip CreatePoseAnimation(PoseData poseData)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = false; // 使用 Mecanim 系统

        foreach (var joint in poseData.joints)
        {
            // 检查是否在映射表中
            if (!boneMapping.TryGetValue(joint.jointName, out string mappedBoneName))
            {
                Debug.LogWarning($"未在映射表中的骨骼: {joint.jointName}，跳过该骨骼。");
                continue;
            }

            // 转换为 Humanoid 骨骼路径
            string humanoidPath = RemapPathToHumanoid(mappedBoneName);
            if (string.IsNullOrEmpty(humanoidPath))
            {
                Debug.LogWarning($"未能映射到 Humanoid 骨骼路径: {mappedBoneName}，跳过该骨骼。");
                continue;
            }

            // 生成动画曲线
            Quaternion rotation = new Quaternion(
                joint.rotation.x,
                joint.rotation.y,
                joint.rotation.z,
                joint.rotation.w
            );

            AnimationCurve curveX = AnimationCurve.Constant(0, 1 / 60f, rotation.x);
            AnimationCurve curveY = AnimationCurve.Constant(0, 1 / 60f, rotation.y);
            AnimationCurve curveZ = AnimationCurve.Constant(0, 1 / 60f, rotation.z);
            AnimationCurve curveW = AnimationCurve.Constant(0, 1 / 60f, rotation.w);

            clip.SetCurve(humanoidPath, typeof(Transform), "localRotation.x", curveX);
            clip.SetCurve(humanoidPath, typeof(Transform), "localRotation.y", curveY);
            clip.SetCurve(humanoidPath, typeof(Transform), "localRotation.z", curveZ);
            clip.SetCurve(humanoidPath, typeof(Transform), "localRotation.w", curveW);
        }

        return clip;
    }

    private string RemapPathToHumanoid(string genericPath)
    {
        // 手部和手指的骨骼映射
        if (genericPath.Contains("mixamorig:RightHand")) return "Animator.RightHand";

        // 大拇指（Thumb）
        if (genericPath.Contains("mixamorig:RightHandThumb1")) return "Animator.RightThumbProximal";
        if (genericPath.Contains("mixamorig:RightHandThumb2")) return "Animator.RightThumbIntermediate";
        if (genericPath.Contains("mixamorig:RightHandThumb3")) return "Animator.RightThumbDistal";
        if (genericPath.Contains("mixamorig:RightHandThumb4")) return "Animator.RightThumbTip";

        // 食指（Index）
        if (genericPath.Contains("mixamorig:RightHandIndex1")) return "Animator.RightIndexProximal";
        if (genericPath.Contains("mixamorig:RightHandIndex2")) return "Animator.RightIndexIntermediate";
        if (genericPath.Contains("mixamorig:RightHandIndex3")) return "Animator.RightIndexDistal";

        // 中指（Middle）
        if (genericPath.Contains("mixamorig:RightHandMiddle1")) return "Animator.RightMiddleProximal";
        if (genericPath.Contains("mixamorig:RightHandMiddle2")) return "Animator.RightMiddleIntermediate";
        if (genericPath.Contains("mixamorig:RightHandMiddle3")) return "Animator.RightMiddleDistal";

        // 无名指（Ring）
        if (genericPath.Contains("mixamorig:RightHandRing1")) return "Animator.RightRingProximal";
        if (genericPath.Contains("mixamorig:RightHandRing2")) return "Animator.RightRingIntermediate";
        if (genericPath.Contains("mixamorig:RightHandRing3")) return "Animator.RightRingDistal";

        // 小拇指（Pinky）
        if (genericPath.Contains("mixamorig:RightHandPinky1")) return "Animator.RightLittleProximal";
        if (genericPath.Contains("mixamorig:RightHandPinky2")) return "Animator.RightLittleIntermediate";
        if (genericPath.Contains("mixamorig:RightHandPinky3")) return "Animator.RightLittleDistal";
        if (genericPath.Contains("mixamorig:RightHandPinky4")) return "Animator.RightLittleTip";

        // 默认返回 null（未匹配到路径）
        return null;
    }
}
