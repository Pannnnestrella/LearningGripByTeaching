using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardingController : MonoBehaviour
{
    // 拖入需要隐藏的面板
    public GameObject uiCanvas;
    public void HideUI()
    {
        uiCanvas.SetActive(false);
    }
}
