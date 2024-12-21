using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class GripDataCollector : MonoBehaviour
{
    public GameObject handTrackingObject;
    private OVRSkeleton _ovrSkeleton;
    private string userID;

    private string gender;
    
    
    // 委托事件-当检测失败时
    public delegate void JointMismatchHandler(string jointName);
    public event JointMismatchHandler OnJointMismatch;
    
    // 委托事件，如果关节匹配上了 boneMatched
    public delegate void JointMatched(string jointName);
    public event JointMatched OnJointMatched;
    
    // 委托事件：所有关节匹配上了
    public delegate void AllJointsMatchedHandler();
    public event AllJointsMatchedHandler OnAllJointsMatched;
    
    void Start()
    {
        // 确保 OVRSkeleton 已经在 Start 时初始化
        if (_ovrSkeleton == null)
        {
            _ovrSkeleton = handTrackingObject.GetComponent<OVRSkeleton>();
            if (_ovrSkeleton == null)
            {
                Debug.LogError("OVRSkeleton component not found on the GameObject.");
                return;
            }
        }
    }
    
    // claim this structure can be serialized
    [System.Serializable]
    public class JointData
    {
        public string jointName;
        public string parentJointName;
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    public class HandData
    {
        public List<JointData> joints = new List<JointData>();
    }
    
    public void SetUserID(string id)
    {
        userID = id;
        Debug.Log($"UserID 已设置为: {userID}");
    }

    public void SetGender(string userGender)
    {
        gender = userGender;
        Debug.Log($"Gender 已设置为: {gender}");
    }

    public string GetGender()
    { 
        return gender ;
    }

    public string GetUserID(){
        return userID;
    }
    
    // For some ID reason (see OVRPlugin.Boneid), the joints' name is wrong; Use this dictionary to re-map the joins' name
    public static Dictionary<string, string> JointMapping = new Dictionary<string, string>
    {
        {"FullBody_Start", "Hand_Start"},
        {"FullBody_Hips", "Hand_WristRoot"},
        {"FullBody_SpineLower", "Hand_Thumb0"},
        {"FullBody_SpineMiddle", "Hand_Thumb1"},
        {"FullBody_SpineUpper", "Hand_Thumb2"},
        {"Hand_Thumb3", "Hand_Thumb3"},
        {"Hand_Index1", "Hand_Index1"},
        {"FullBody_Head", "Hand_Index2"},
        {"Hand_Index3", "Hand_Index3"},
        {"Hand_Middle1", "Hand_Middle1"},
        {"FullBody_LeftArmUpper", "Hand_Middle2"},
        {"Body_LeftArmLower", "Hand_Middle3"},
        {"Body_LeftHandWristTwist", "Hand_Ring1"},
        {"Body_RightShoulder", "Hand_Ring2"},
        {"Body_RightScapula", "Hand_Ring3"},
        {"Hand_Pinky0", "Hand_Pinky0"},
        {"Body_RightArmLower", "Hand_Pinky1"},
        {"FullBody_RightHandWristTwist", "Hand_Pinky2"},
        {"Hand_Pinky3", "Hand_Pinky3"},
        {"Hand_MaxSkinnable", "Hand_ThumbTip"},
        {"Body_LeftHandThumbMetacarpal", "Hand_IndexTip"},
        {"Hand_MiddleTip", "Hand_MiddleTip"},
        {"Hand_RingTip", "Hand_RingTip"},
        {"Body_LeftHandThumbTip", "Hand_PinkyTip"}
    };
    
    // For the same reason, remap the father joint with this dicionary
    public static Dictionary<int, string> BoneIdToNameMap = new Dictionary<int, string>
    {
        { -1, "Invalid" },
        { 0, "Hand_WristRoot" },
        { 1, "Hand_ForearmStub" },
        { 2, "Hand_Thumb0" },
        { 3, "Hand_Thumb1" },
        { 4, "Hand_Thumb2" },
        { 5, "Hand_Thumb3" },
        { 6, "Hand_Index1" },
        { 7, "Hand_Index2" },
        { 8, "Hand_Index3" },
        { 9, "Hand_Middle1" },
        { 10, "Hand_Middle2" },
        { 11, "Hand_Middle3" },
        { 12, "Hand_Ring1" },
        { 13, "Hand_Ring2" },
        { 14, "Hand_Ring3" },
        { 15, "Hand_Pinky0" },
        { 16, "Hand_Pinky1" },
        { 17, "Hand_Pinky2" },
        { 18, "Hand_Pinky3" },
        { 19, "Hand_MaxSkinnable" },
        { 20, "Hand_ThumbTip" },
        { 21, "Hand_IndexTip" },
        { 22, "Hand_MiddleTip" },
        { 23, "Hand_RingTip" },
        { 24, "Hand_PinkyTip" },
        { 25, "Hand_End" }
    };
    // cosine similarity to detect 

    //根据性别选择不同的文件
    public bool CompareHandPose(HandData handData)
    {
        string fileName = gender == "male" ? "man_gesture.json" : "standard_gesture.json";
        string stdFilePath = Path.Combine(Application.persistentDataPath, fileName);

        return CompareHandPose(handData, stdFilePath);
    }
    
    public bool CompareHandPose(HandData handData,string stdFilePath)
    {
        // 要抛弃欧拉角，转而使用四元数角度差和欧式距离结合的方法
        
        // 读取json文件，得到保存的标准姿势
        string jsonString = File.ReadAllText(stdFilePath);
        HandData stdHandData = JsonConvert.DeserializeObject<HandData>(jsonString);
        
        // 如果标准姿势或待比较待姿势为空，返回空
        if (handData == null || stdHandData == null)
        {
            Debug.LogError("传入的手势数据为空，无法比较。");
            return false;
        }
        // 确保两个手势数据的关节数量相同
        if (stdHandData.joints.Count != handData.joints.Count)
        {
            Debug.LogWarning("关节数量不一致，请检查手势追踪的模式。");
            return false;
        }
        // 初始状态被标记为匹配
        bool boneMatched = true;
        
        float rotationThreshold = 15.0f; // 定义四元数旋转角度阈值
        // 将标准姿势与进行比较
        for (int i = 0; i < stdHandData.joints.Count; i++)
        {
            JointData stdJoint = stdHandData.joints[i];
            JointData joint = handData.joints[i];
            // 忽略手腕根部，因为手腕根部的变化与握笔姿势正确与否无关
            // 忽略指尖，因为指尖的旋转没有物理含义
            string jointName = stdJoint.jointName.ToString();
            if (jointName.EndsWith("Root") || jointName.EndsWith("Start") || jointName.EndsWith("Tip"))
            {
                continue;
            }
            
            // 结合欧氏距离作为判断，尤其是针对某些容易混淆的关节
            // float distanceThreshold = 0.02f; // 距离阈值，单位：米（或根据你的数据单位调整）
            // float distanceDifference = Vector3.Distance(stdJoint.position, joint.position);
            // if (distanceDifference > distanceThreshold)
            // {
            //     boneMatched = false;
            //     OnJointMismatch?.Invoke(stdJoint.jointName);
            // }
            
            // 使用四元数旋转角 的 差值 作为 是否匹配的标准
            float angleDifference = GetQuaternionAngleDifference(stdJoint.rotation, joint.rotation);
            if (angleDifference < rotationThreshold)
            {
                // Debug.Log($"关节{stdJoint.jointName.ToString()}匹配");
                OnJointMatched?.Invoke(stdJoint.jointName);
            }
            // // 计算用户向量与标准化向量之间的角度差，四元数转欧拉角表示
            // Vector3 stdJointEuler = stdJointRotation.eulerAngles;
            // Vector3 jointEuler = jointRotation.eulerAngles;
            // Vector3 jointDifference = GetDegreeAngleDifferenceXYZ(stdJointEuler, jointEuler);
            // // if x上的差异，y上的差异，z上的差异都小于阈值（-参见ultraleap-HandPoseDetector.cs）
            //
            // if (Mathf.Abs(jointDifference.x) < xThreshold &&
            //      Mathf.Abs(jointDifference.y) < yThreshold )
            //      // Mathf.Abs(jointDifference.z) < zThreshold)
            // {
            //     // Debug.Log($"关节{stdJoint.jointName.ToString()}匹配");
            //     OnJointMatched?.Invoke(stdJoint.jointName);
            // }
            else
            {
                // Debug.Log($"关节{stdJoint.jointName.ToString()}不匹配");
                // 触发不匹配事件
                OnJointMismatch?.Invoke(stdJoint.jointName);
                boneMatched = false;
            }
        }

        if (boneMatched)
        {
            OnAllJointsMatched?.Invoke();
        }
        
        return boneMatched;
    }
    /**
     * 以欧拉角的角度差作为匹配标准
     * 计算欧拉角的角度差
     */
    private static Vector3 GetDegreeAngleDifferenceXYZ(Vector3 a, Vector3 b)
    {
        var angleX = Mathf.DeltaAngle(a.x, b.x);
        var angleY = Mathf.DeltaAngle(a.y, b.y);
        var angleZ = Mathf.DeltaAngle(a.z, b.z);

        return new Vector3(angleX, angleY, angleZ);
    }
    /**
     * 以四元数的角度差为标准
     * 四元数的点积表示其旋转的角度差
     * 返回值将弧度表示转为角度表示
     */
    private static float GetQuaternionAngleDifference(Quaternion a, Quaternion b)
    {
        float dot = Quaternion.Dot(a, b);
        return Mathf.Acos(Mathf.Min(Mathf.Abs(dot), 1.0f)) * 2.0f * Mathf.Rad2Deg;
    }

 public HandData CollectGripData(string userID = null, string gender = null, string fileName = null, bool saveToFile = true)
{
    HandData handData = new HandData();

    // 检查 OVRSkeleton 是否有效
    if (_ovrSkeleton == null || _ovrSkeleton.Bones == null || _ovrSkeleton.Bones.Count == 0)
    {
        Debug.LogWarning("OVRSkeleton 未初始化或无关节数据。");
        return handData;
    }

    // 遍历所有骨骼并收集数据
    foreach (var bone in _ovrSkeleton.Bones)
    {
        if (bone != null && bone.Transform != null)
        {
            JointData jointData = new JointData
            {
                jointName = JointMapping.ContainsKey(bone.Id.ToString()) ? JointMapping[bone.Id.ToString()] : bone.Id.ToString(),
                parentJointName = BoneIdToNameMap.ContainsKey(bone.ParentBoneIndex) ? BoneIdToNameMap[bone.ParentBoneIndex] : "Unknown",
                position = bone.Transform.localPosition,
                rotation = bone.Transform.localRotation
            };

            handData.joints.Add(jointData);
        }
    }
    // 如果需要保存文件，才执行保存逻辑
    if (saveToFile)
    {
        SaveGripDataToJson(handData, userID, fileName);
    }

    return handData;
}


    private void SaveGripDataToJson(HandData handData, string userID = null, string fileName = null)
    {
        // 序列化数据为JSON字符串
        string json = JsonUtility.ToJson(handData, true);

        // 判断是否保存到用户目录
        string saveFolderPath;
        if (string.IsNullOrEmpty(userID))
        {
            // 保存标准手势到固定目录
            saveFolderPath = Application.persistentDataPath;
            fileName = string.IsNullOrEmpty(fileName) ? "standard_new.json" : fileName;
        }
        else
        {
            // 保存用户手势数据到用户特定的目录
            saveFolderPath = Path.Combine(Application.persistentDataPath, userID);
            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
            }
        
            // 确保文件名唯一，添加时间戳
            if (string.IsNullOrEmpty(fileName))
            {
                string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                fileName = $"gripdata_{timestamp}.json";
            }
        }

        // 完整的文件路径
        string filePath = Path.Combine(saveFolderPath, fileName);

        // 保存文件
        try
        {
            File.WriteAllText(filePath, json);
            // Debug.Log($"数据保存到 {filePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"保存 JSON 文件失败: {e.Message}");
        }
    }

}
