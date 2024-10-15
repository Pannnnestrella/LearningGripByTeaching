using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AgentPoseTransition : MonoBehaviour
{
    public Transform agentHandRoot; // 虚拟代理的手部根Transform
    public Transform poorPose; // 保存错误姿势的容器
    public Transform referencePose; // 保存正确姿势的容器
    public float transitionDuration = 5.0f; // 过渡时间

    private float elapsedTime = 0.0f;
    private bool isTransitioning = false;
    private List<Transform> agentTransforms = new List<Transform>();
    private List<Transform> poorPoseTransforms = new List<Transform>();
    private List<Transform> referencePoseTransforms = new List<Transform>();

    void Start()
    {
        // 初始化姿势
        if (agentHandRoot == null || poorPose == null || referencePose == null)
        {
            Debug.LogError("Agent Hand Root, Poor Pose, or Reference Pose is not set!");
            return;
        }

        InitializeTransforms(agentHandRoot, poorPose, agentTransforms, poorPoseTransforms);
        InitializeTransforms(agentHandRoot, referencePose, agentTransforms, referencePoseTransforms);

        SetPose(poorPoseTransforms); // 设置为初始的错误姿势

        // 自动启动姿势过渡
        StartPoseTransition();
    }

    void Update()
    {
        if (isTransitioning)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / transitionDuration);
            
            Debug.Log($"Transition progress: {progress}");
            // 在姿势之间过渡
            for (int i = 0; i < agentTransforms.Count; i++)
            {
                agentTransforms[i].localPosition = Vector3.Lerp(poorPoseTransforms[i].localPosition, referencePoseTransforms[i].localPosition, progress);
                agentTransforms[i].localRotation = Quaternion.Lerp(poorPoseTransforms[i].localRotation, referencePoseTransforms[i].localRotation, progress);
            }

            // 检查过渡是否完成
            if (progress >= 1.0f)
            {
                isTransitioning = false;
                elapsedTime = 0.0f;
                Debug.Log("Pose transition completed.");
            }
        }
    }

    public void StartPoseTransition()
    {
        isTransitioning = true;
        elapsedTime = 0.0f;
        Debug.Log("Pose transition started");
    }

    private void InitializeTransforms(Transform agent, Transform pose, List<Transform> agentList, List<Transform> poseList)
    {
        agentList.Clear();
        poseList.Clear();
        FindTransformsRecursively(agent, pose, agentList, poseList);
    }

    private void FindTransformsRecursively(Transform agent, Transform pose, List<Transform> agentList, List<Transform> poseList)
    {
        agentList.Add(agent);
        poseList.Add(pose);

        for (int i = 0; i < agent.childCount; i++)
        {
            Transform agentChild = agent.GetChild(i);
            Transform poseChild = pose.Find(agentChild.name);

            if (poseChild != null)
            {
                FindTransformsRecursively(agentChild, poseChild, agentList, poseList);
            }
        }
    }

    private void SetPose(List<Transform> poseTransforms)
    {
        for (int i = 0; i < agentTransforms.Count; i++)
        {
            agentTransforms[i].localPosition = poseTransforms[i].localPosition;
            agentTransforms[i].localRotation = poseTransforms[i].localRotation;
            Debug.Log($"Setting position and rotation for: {agentTransforms[i].name}");
        }
    }
}
