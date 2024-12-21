using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandJointVisualizer : MonoBehaviour
{
    public GameObject handTrackingObject; // 手部追踪对象
    public GameObject jointPrefab;       // 默认关节预制件
    public GameObject redJointPrefab;    // 不匹配关节预制件
    public bool isActive = false;        // 控制脚本是否激活
    public GripDataCollector gripDataCollector; // 引用 GripDataCollector

    private List<GameObject> _jointVisuals = new List<GameObject>();
    private OVRSkeleton _ovrSkeleton;
    private Dictionary<string, int> _mappedJoints = new Dictionary<string, int>();

    void Start()
    {
        InitializeComponents();

        if (_ovrSkeleton != null)
        {
            InitializeMappedJoints();
            StartCoroutine(InitializeJointVisuals());
        }

        if (gripDataCollector != null)
        {
            // 注册 GripDataCollector 的事件
            gripDataCollector.OnJointMismatch += HighlightMismatchedJoint;
            gripDataCollector.OnJointMatched += ResetJointToOriginal;
        }
        else
        {
            Debug.LogError("GripDataCollector 未绑定，请在 Inspector 中设置。");
        }
    }

    void Update()
    {
        if (isActive && _ovrSkeleton != null && _ovrSkeleton.Bones.Count > 0)
        {
            UpdateJointVisuals();
        }
    }

    public void ToggleActivation()
    {
        isActive = !isActive;
        if (isActive)
        {
            InitializeJointVisuals();
        }
        else
        {
            foreach (var jointVisual in _jointVisuals)
            {
                if (jointVisual != null)
                {
                    jointVisual.SetActive(false);
                }
            }
        }
    }

    private void InitializeComponents()
    {
        if (handTrackingObject != null)
        {
            _ovrSkeleton = handTrackingObject.GetComponentInChildren<OVRSkeleton>();
        }

        if (_ovrSkeleton == null)
        {
            Debug.LogError("OVRSkeleton 组件未找到，请检查 handTrackingObject 设置。");
        }
    }

    private void InitializeMappedJoints()
    {
        if (_ovrSkeleton.Bones == null || _ovrSkeleton.Bones.Count == 0)
        {
            Debug.LogError("OVRSkeleton 的 Bones 列表为空，无法初始化关节映射。");
            return;
        }

        foreach (var bone in _ovrSkeleton.Bones)
        {
            string boneName = bone.Id.ToString();
            if (GripDataCollector.JointMapping.ContainsKey(boneName))
            {
                boneName = GripDataCollector.JointMapping[boneName];
            }

            int boneIndex = _ovrSkeleton.Bones.IndexOf(bone);
            if (!_mappedJoints.ContainsKey(boneName))
            {
                _mappedJoints[boneName] = boneIndex;
            }
            Debug.Log($"关节映射: {bone.Id} -> {boneName}");
        }
    }

    private IEnumerator InitializeJointVisuals()
    {
        while (_ovrSkeleton.Bones.Count == 0)
        {
            yield return null;
        }

        foreach (var bone in _ovrSkeleton.Bones)
        {
            GameObject jointVisual = Instantiate(jointPrefab);
            jointVisual.transform.parent = this.transform;
            jointVisual.SetActive(false);
            _jointVisuals.Add(jointVisual);
        }

        Debug.Log("关节可视化初始化完成！");
    }

    private void UpdateJointVisuals()
    {
        for (int i = 0; i < _ovrSkeleton.Bones.Count; i++)
        {
            var bone = _ovrSkeleton.Bones[i];
            if (bone.Transform != null && i < _jointVisuals.Count)
            {
                var jointVisual = _jointVisuals[i];
                if (jointVisual != null)
                {
                    jointVisual.transform.position = bone.Transform.position;
                    jointVisual.transform.rotation = bone.Transform.rotation;
                    jointVisual.SetActive(true);
                }
            }
        }
    }

    public void HighlightMismatchedJoint(string jointName)
    {
        if (_mappedJoints.TryGetValue(jointName, out int jointIndex) && jointIndex < _jointVisuals.Count)
        {
            // Debug.Log($"关节 {jointName} 不匹配，替换为红色预制件。");
            Destroy(_jointVisuals[jointIndex]);
            GameObject redJointVisual = Instantiate(redJointPrefab);
            redJointVisual.transform.parent = this.transform;
            redJointVisual.transform.position = _ovrSkeleton.Bones[jointIndex].Transform.position;
            redJointVisual.transform.rotation = _ovrSkeleton.Bones[jointIndex].Transform.rotation;
            _jointVisuals[jointIndex] = redJointVisual;
        }
        else
        {
            Debug.LogWarning($"未找到关节 {jointName} 的映射！");
        }
    }

    public void ResetJointToOriginal(string jointName)
    {
        if (_mappedJoints.TryGetValue(jointName, out int jointIndex) && jointIndex < _jointVisuals.Count)
        {
            // Debug.Log($"关节 {jointName} 恢复为默认预制件。");
            Destroy(_jointVisuals[jointIndex]);
            GameObject originalJointVisual = Instantiate(jointPrefab);
            originalJointVisual.transform.parent = this.transform;
            originalJointVisual.transform.position = _ovrSkeleton.Bones[jointIndex].Transform.position;
            originalJointVisual.transform.rotation = _ovrSkeleton.Bones[jointIndex].Transform.rotation;
            _jointVisuals[jointIndex] = originalJointVisual;
        }
        else
        {
            Debug.LogWarning($"未找到关节 {jointName} 的映射！");
        }
    }
}
