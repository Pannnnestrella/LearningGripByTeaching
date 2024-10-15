using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class writeI : MonoBehaviour
{
       private LineRenderer lineRenderer;
       private int pointIndex = 0;
       
   
       void Start()
       {
           lineRenderer = GetComponent<LineRenderer>();
           lineRenderer.startWidth = 0.001f;  // 非常细的线条
           lineRenderer.endWidth = 0.001f;
   
           // 初始化LineRenderer的点数量
           lineRenderer.positionCount = 0;
       }
   
       void Update()
       {
           if (lineRenderer.positionCount == 0) 
           {
               // 如果没有点，则初始化第一个点
               lineRenderer.positionCount = 1;
               lineRenderer.SetPosition(0, transform.position);
           }
           
           // 笔尖移动时，动态添加点到LineRenderer中
           if (Vector3.Distance(transform.position, lineRenderer.GetPosition(lineRenderer.positionCount - 1)) > 0.01f)
           {
               lineRenderer.positionCount += 1;
               lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
           }
       }
}
