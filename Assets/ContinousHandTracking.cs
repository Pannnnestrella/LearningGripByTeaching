using UnityEngine;
using System.IO;

public class ContinuousHandMatching : MonoBehaviour
{
    public GripDataCollector gripDataCollector; // 引用 GripDataCollector
    private bool isMatching = false; // 是否正在进行实时手势匹配

    void Start()
    {
        if (gripDataCollector == null)
        {
            Debug.LogError("GripDataCollector 未绑定！");
            return;
        }

        // 注册事件
        gripDataCollector.OnAllJointsMatched += HandleAllJointsMatched;
        gripDataCollector.OnJointMismatch += HandleJointMismatch;
    }

    void Update()
    {
        if (isMatching)
        {
            PerformMatching(); // 每帧执行匹配
        }
    }

    public void ToggleMatching()
    {
        isMatching = !isMatching;
        // Debug.Log(isMatching ? "实时手势匹配已启动。" : "实时手势匹配已停止。");
    }

    void PerformMatching()
    {
        if (gripDataCollector == null) return;

        // 收集实时手势数据
        GripDataCollector.HandData currentHandData = gripDataCollector.CollectGripData(saveToFile: false);

        // 获取标准手势文件路径
        string standardFileName = gripDataCollector.GetGender() == "male" ? "man_gesture.json" : "standard_gesture.json";
        string standardFilePath = Path.Combine(Application.persistentDataPath, standardFileName);

        // 执行比对
        bool isMatched = gripDataCollector.CompareHandPose(currentHandData, standardFilePath);

        if (isMatched)
        {
            // Debug.Log("实时手势匹配成功！");
        }
        else
        {
            // Debug.Log("实时手势匹配失败！");
        }
    }


    // 事件处理函数：所有关节匹配时调用
    private void HandleAllJointsMatched()
    {
        // Debug.Log("事件：所有关节匹配成功！");
        // 在这里可以进一步扩展功能，例如更新 UI 或触发其他逻辑
    }

    // 事件处理函数：单个关节不匹配时调用
    private void HandleJointMismatch(string jointName)
    {
        // Debug.Log($"事件：关节 {jointName} 不匹配！");
        // 在这里可以进一步扩展功能，例如记录日志或提示用户
    }

    void OnDestroy()
    {
        // 注销事件，防止内存泄漏
        if (gripDataCollector != null)
        {
            gripDataCollector.OnAllJointsMatched -= HandleAllJointsMatched;
            gripDataCollector.OnJointMismatch -= HandleJointMismatch;
        }
    }
}
