using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GripDataManager : MonoBehaviour
{
    private GripDataCollector _dataCollector; // 引用 GripDataCollector
    private GripDataSender _dataSender;
    private OVRHand _hand; // 手势检测
    public GameObject handTrackingObject;

    // 阶段按钮
    public Button startStage1Button, stopStage1Button; // 阶段1按钮
    public Button startStage2Button, stopStage2Button; // 阶段2按钮

    public float pinchThreshold = 0.8f; // Pinch手势触发阈值
    private bool _isDataLocked = false; // Pinch锁标志

    private bool isCollectingStage1 = false; // 阶段1标志位
    private bool isCollectingStage2 = false; // 阶段2标志位

    [SerializeField] private bool usePinchUpdate = false;

    private int _frameCountStage1 = 0; // 阶段1帧计数
    private int _frameCountStage2 = 0; // 阶段2帧计数

    void Start()
    {
        _dataCollector = GetComponent<GripDataCollector>();
        _dataSender = GetComponent<GripDataSender>();

        if (_dataCollector == null)
        {
            Debug.LogError("未找到 GripDataCollector 组件！");
            return;
        }
        if (_dataSender == null)
        {
            Debug.LogError("未找到 GripDataSender 组件！");
            return;
        }

        if (handTrackingObject != null)
        {
            _hand = handTrackingObject.GetComponent<OVRHand>();
        }
        if (_hand == null)
        {
            Debug.LogError("在指定对象上未找到 OVRHand 组件。");
            return;
        }

        // 注册按钮事件
        startStage1Button.onClick.AddListener(() => StartDataCollection(1));
        stopStage1Button.onClick.AddListener(() => StopDataCollection(1));
        startStage2Button.onClick.AddListener(() => StartDataCollection(2));
        stopStage2Button.onClick.AddListener(() => StopDataCollection(2));

        Debug.Log($"检测的手部类型为: {_hand.GetHand()}");
        StartCoroutine(WaitForHandTracking());
    }

    IEnumerator WaitForHandTracking()
    {
        while (_hand != null && !_hand.IsTracked)
        {
            yield return null;
        }

        Debug.Log("手势追踪已就绪，等待按钮启动数据采集...");
    }

    void Update()
    {
        if (_hand == null || !_hand.IsTracked) return;

        // 每帧检查阶段1和阶段2的采集状态
        if (isCollectingStage1)
        {
            SaveDataForStage(1);
        }

        if (isCollectingStage2)
        {
            SaveDataForStage(2);
        }

        // 检查 Pinch 手势，用于保存标准手势数据
        if(usePinchUpdate){
            PinchUpdate();
        }
    }

    // Pinch 手势触发保存标准数据
    void PinchUpdate()
    {
        if (_hand != null && _hand.IsTracked && !_isDataLocked)
        {
            if (_hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold)
            {
                Debug.Log("检测到捏合手势，保存标准手势数据。");
                string fileName = "standard_gesture.json";
                _dataCollector.CollectGripData(null, null, fileName);

                _isDataLocked = true;
                StartCoroutine(UnlockDataCollectionAfterDelay(1.0f));
            }
        }
    }

    // 每帧保存阶段数据
    void SaveDataForStage(int stage)
    {
        if (stage == 1)
        {
            _frameCountStage1++;
            string fileName = $"gripdata_stage1_frame_{_frameCountStage1}.json";
           _dataCollector.CollectGripData(_dataCollector.GetUserID(), null, fileName);
            // Debug.Log($"阶段1: 保存数据 {fileName}");
        }
        else if (stage == 2)
        {
            _frameCountStage2++;
            string fileName = $"gripdata_stage2_frame_{_frameCountStage2}.json";
            _dataCollector.CollectGripData(_dataCollector.GetUserID(), null, fileName);
            // Debug.Log($"阶段2: 保存数据 {fileName}");
        }
    }

    // 启动数据采集
    void StartDataCollection(int stage)
    {
        if (stage == 1)
        {
            isCollectingStage1 = true;
            // Debug.Log("开始阶段1的数据采集...");
        }
        else if (stage == 2)
        {
            isCollectingStage2 = true;
            // Debug.Log("开始阶段2的数据采集...");
        }
    }

    // 停止数据采集
    void StopDataCollection(int stage)
    {
        if (stage == 1)
        {
            isCollectingStage1 = false;
            // Debug.Log("阶段1的数据采集已停止。");
        }
        else if (stage == 2)
        {
            isCollectingStage2 = false;
            // Debug.Log("阶段2的数据采集已停止。");
        }
    }

    IEnumerator UnlockDataCollectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isDataLocked = false;
        Debug.Log("Pinch数据采集解锁。");
    }
}
