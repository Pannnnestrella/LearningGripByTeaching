using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandJointVisualizer : MonoBehaviour
{
    public GameObject handTrackingObject; 
    public GameObject jointPrefab;      
    public GameObject redJointPrefab;   
    public bool isActive = false;        
    public GripDataCollector gripDataCollector; // reference GripDataCollector

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
            // Registering Events for the GripDataCollector
            gripDataCollector.OnJointMismatch += HighlightMismatchedJoint;
            gripDataCollector.OnJointMatched += ResetJointToOriginal;
        }
        else
        {
            Debug.LogError("GripDataCollector is not bound, please set it in Inspector.");
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
            Debug.LogError("OVRSkeleton component not found, check handTrackingObject setting.");
        }
    }

    private void InitializeMappedJoints()
    {
        if (_ovrSkeleton.Bones == null || _ovrSkeleton.Bones.Count == 0)
        {
            Debug.LogError("The Bones list of OVRSkeleton is empty and the joint mapping cannot be initialised.");
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

        Debug.Log("Joint visualisation initialisation is complete!");
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
            
            Destroy(_jointVisuals[jointIndex]);
            GameObject redJointVisual = Instantiate(redJointPrefab);
            redJointVisual.transform.parent = this.transform;
            redJointVisual.transform.position = _ovrSkeleton.Bones[jointIndex].Transform.position;
            redJointVisual.transform.rotation = _ovrSkeleton.Bones[jointIndex].Transform.rotation;
            _jointVisuals[jointIndex] = redJointVisual;
        }
        else
        {
            Debug.LogWarning($"No mapping found for joint {jointName}!");
        }
    }

    public void ResetJointToOriginal(string jointName)
    {
        if (_mappedJoints.TryGetValue(jointName, out int jointIndex) && jointIndex < _jointVisuals.Count)
        {
           
            Destroy(_jointVisuals[jointIndex]);
            GameObject originalJointVisual = Instantiate(jointPrefab);
            originalJointVisual.transform.parent = this.transform;
            originalJointVisual.transform.position = _ovrSkeleton.Bones[jointIndex].Transform.position;
            originalJointVisual.transform.rotation = _ovrSkeleton.Bones[jointIndex].Transform.rotation;
            _jointVisuals[jointIndex] = originalJointVisual;
        }
        else
        {
            Debug.LogWarning($"No mapping found for joint {jointName}!");
        }
    }
}
