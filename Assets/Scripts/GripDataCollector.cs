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
    
    
    // Delegated events - when detection fails
    public delegate void JointMismatchHandler(string jointName);
    public event JointMismatchHandler OnJointMismatch;
    
    // Delegate events if the joint matches the boneMatched
    public delegate void JointMatched(string jointName);
    public event JointMatched OnJointMatched;
    
    // Delegated event: all joints matched up
    public delegate void AllJointsMatchedHandler();
    public event AllJointsMatchedHandler OnAllJointsMatched;
    
    void Start()
    {
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
        Debug.Log($"UserID is set to: {userID}");
    }

    public void SetGender(string userGender)
    {
        gender = userGender;
        Debug.Log($"Gender is set to: {gender}");
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

    //Selection of different documents according to gender
    public bool CompareHandPose(HandData handData)
    {
        // Files are saved in the resources directory by default
        string fileName = gender == "male" ? "standard_gesture_male" : "standard_gesture_female";
        // string stdFilePath = Path.Combine(Application.persistentDataPath, fileName);
        return CompareHandPose(handData, fileName);
    }
    
    public bool CompareHandPose(HandData handData,string jsonFileName)
    {
        // To discard Euler angles in favour of a combination of quaternion angle differences and Euclidean distances
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFileName);
        if (jsonTextAsset == null)
        {
            Debug.LogError($"Failed to load JSON file '{jsonFileName}' from Resources.");
            return false; // Exit early to avoid subsequent access to null objects
        }
        // Read the contents of the JSON file to get the saved standard pose
        string jsonString = jsonTextAsset.text;
        HandData stdHandData = JsonConvert.DeserializeObject<HandData>(jsonString);  
        if (jsonTextAsset == null)
        {
            Debug.LogError($"JSON file {jsonFileName} not found in Resources.");
        }
        
        // string jsonString = File.ReadAllText(stdFilePath);
        
        
        // Returns null if the standard pose or the pose to be compared is null.
        if (handData == null || stdHandData == null)
        {
            Debug.LogError("The incoming gesture data is empty and cannot be compared.");
            return false;
        }
        // Make sure both gesture data have the same number of connectors
        if (stdHandData.joints.Count != handData.joints.Count)
        {
            Debug.LogWarning("The number of joints is not consistent, check the mode of gesture tracking.");
            return false;
        }
      
        bool boneMatched = true;
        
        float rotationThreshold = 15.0f; // threshold of the angle

        for (int i = 0; i < stdHandData.joints.Count; i++)
        {
            JointData stdJoint = stdHandData.joints[i];
            JointData joint = handData.joints[i];
            // Ignore the base of the wrist, because changes in the base of the wrist have nothing to do with a correct pen grip.
            // Ignore the fingertips, because rotation of the fingertips has no physical meaning.
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
            
            // Use the difference of quaternion rotation angles as a criterion for matching.
            float angleDifference = GetQuaternionAngleDifference(stdJoint.rotation, joint.rotation);
            if (angleDifference < rotationThreshold)
            {
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
                // Debug.Log($"jonits{stdJoint.jointName.ToString()}dismatch");
    
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
     * Using the angular difference of Euler angles as a matching criterion
     * Calculate the angular difference of Euler angles
     */
    private static Vector3 GetDegreeAngleDifferenceXYZ(Vector3 a, Vector3 b)
    {
        var angleX = Mathf.DeltaAngle(a.x, b.x);
        var angleY = Mathf.DeltaAngle(a.y, b.y);
        var angleZ = Mathf.DeltaAngle(a.z, b.z);

        return new Vector3(angleX, angleY, angleZ);
    }
    /**
     * Expressed as the angular difference of quaternions
     * The dot product of quaternions expresses the angular difference of their rotation
     * The return value converts the radian representation to an angular representation
     */
    private static float GetQuaternionAngleDifference(Quaternion a, Quaternion b)
    {
        float dot = Quaternion.Dot(a, b);
        return Mathf.Acos(Mathf.Min(Mathf.Abs(dot), 1.0f)) * 2.0f * Mathf.Rad2Deg;
    }

 public HandData CollectGripData(string userID = null, string gender = null, string fileName = null, bool saveToFile = true)
{
    HandData handData = new HandData();

    if (_ovrSkeleton == null || _ovrSkeleton.Bones == null || _ovrSkeleton.Bones.Count == 0)
    {
        Debug.LogWarning("OVRSkeleton not initialised or no joint data.");
        return handData;
    }

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
    // Execute the save logic only if the file needs to be saved
    if (saveToFile)
    {
        SaveGripDataToJson(handData, userID, fileName);
    }

    return handData;
}


    private void SaveGripDataToJson(HandData handData, string userID = null, string fileName = null)
    {

        string json = JsonUtility.ToJson(handData, true);

        // Determine whether to save to the user directory
        string saveFolderPath;
        if (string.IsNullOrEmpty(userID))
        {
            // Save standard gestures to a fixed directory
            saveFolderPath = Application.persistentDataPath;
            fileName = string.IsNullOrEmpty(fileName) ? "standard_new.json" : fileName;
        }
        else
        {
            // Save user gesture data to a user-specific directory
            saveFolderPath = Path.Combine(Application.persistentDataPath, userID);
            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
            }
        
            // Ensure file names are unique, add timestamps
            if (string.IsNullOrEmpty(fileName))
            {
                string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                fileName = $"gripdata_{timestamp}.json";
            }
        }

        string filePath = Path.Combine(saveFolderPath, fileName);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"The data is saved to the {filePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save JSON file. {e.Message}");
        }
    }

}
