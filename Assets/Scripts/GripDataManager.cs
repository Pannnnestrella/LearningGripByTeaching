using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GripDataManager : MonoBehaviour
{
    private GripDataCollector _dataCollector; 
    private GripDataSender _dataSender;
    private OVRHand _hand; 
    public GameObject handTrackingObject;

    // button control
    public Button startStage1Button, stopStage1Button; 
    public Button startStage2Button, stopStage2Button; 

    public float pinchThreshold = 0.8f; // Pinch threshold
    private bool _isDataLocked = false; // Pinch lock sign

    private bool isCollectingStage1 = false; // Phase 1 Flag Bit
    private bool isCollectingStage2 = false; // Phase 2 Flag Bit

    [SerializeField] private bool usePinchUpdate = false; // Decide if you want to keep the standard hand signals

    private int _frameCountStage1 = 0; // Phase 1 frame count
    private int _frameCountStage2 = 0; // Phase 2 Frame Count

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


        startStage1Button.onClick.AddListener(() => StartDataCollection(1));
        stopStage1Button.onClick.AddListener(() => StopDataCollection(1));
        startStage2Button.onClick.AddListener(() => StartDataCollection(2));
        stopStage2Button.onClick.AddListener(() => StopDataCollection(2));

        Debug.Log($"The hand types detected are: {_hand.GetHand()}");
        StartCoroutine(WaitForHandTracking());
    }

    IEnumerator WaitForHandTracking()
    {
        while (_hand != null && !_hand.IsTracked)
        {
            Debug.Log("Gesture tracking not ready");
            yield return null;
        }

        Debug.Log("Gesture tracking is ready and waiting for a button to initiate data collection...");
    }

    void Update()
    {
        if (_hand == null || !_hand.IsTracked) {
            Debug.Log("Gesture tracking not ready");
            return;
        }

        if (isCollectingStage1)
        {
            SaveDataForStage(1);
        }

        if (isCollectingStage2)
        {
            SaveDataForStage(2);
        }

        // Check Pinch gestures for saving standard gesture data
        if(usePinchUpdate){
            PinchUpdate();
        }
    }

    void PinchUpdate()
    {
        if (_hand != null && _hand.IsTracked && !_isDataLocked)
        {
            if (_hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold)
            {
                Debug.Log("Pinch gestures are detected and standard gesture data is saved.");
                // string fileName = "standard_gesture.json";
                _isDataLocked = true;
                StartCoroutine(UnlockDataCollectionAfterDelay(1.0f));
            }
        }
    }

    // Save phase data per frame
    void SaveDataForStage(int stage)
    {
        if (stage == 1)
        {
            _frameCountStage1++;
            string fileName = $"gripdata_stage1_frame_{_frameCountStage1}.json";
           _dataCollector.CollectGripData(_dataCollector.GetUserID(), null, fileName);
            Debug.Log($"stage1: save data to {fileName}");
        }
        else if (stage == 2)
        {
            _frameCountStage2++;
            string fileName = $"gripdata_stage2_frame_{_frameCountStage2}.json";
            _dataCollector.CollectGripData(_dataCollector.GetUserID(), null, fileName);
            Debug.Log($"stage 2: save data to {fileName}");
        }
    }

    // Initiate data acquisition
    void StartDataCollection(int stage)
    {
        if (stage == 1)
        {
            isCollectingStage1 = true;

        }
        else if (stage == 2)
        {
            isCollectingStage2 = true;
            
        }
    }

    // stop collect data
    void StopDataCollection(int stage)
    {
        if (stage == 1)
        {
            isCollectingStage1 = false;
           
        }
        else if (stage == 2)
        {
            isCollectingStage2 = false;
           
        }
    }

    IEnumerator UnlockDataCollectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isDataLocked = false;
        Debug.Log("Pinch Data Acquisition Unlocked.");
    }
}
