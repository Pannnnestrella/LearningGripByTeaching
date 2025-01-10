using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseSaver : MonoBehaviour
{
    public Transform handRoot; 
    public Transform poseContainer; 

    public void SavePose()
    {
        if (handRoot == null || poseContainer == null)
        {
            Debug.LogError("Hand root or pose container is not set!");
            return;
        }

        foreach (Transform child in poseContainer)
        {
            DestroyImmediate(child.gameObject);
        }
        
        CopyTransformsRecursively(handRoot, poseContainer);
    }

    private void CopyTransformsRecursively(Transform source, Transform destination)
    {
        foreach (Transform child in source)
        {
            GameObject newChild = new GameObject(child.name);
            newChild.transform.SetParent(destination);

            newChild.transform.localPosition = child.localPosition;
            newChild.transform.localRotation = child.localRotation;
            newChild.transform.localScale = child.localScale;

            CopyTransformsRecursively(child, newChild.transform);
        }
    }
}

