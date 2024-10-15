using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging; 
using UnityEngine;

public class Write : MonoBehaviour
{
    
    //public Transform penTip;  // 笔尖对象的 Transform
    
    public TwoBoneIKConstraint handIKConstraint;  // 引用Two Bone IK 组件
    //public Rig rig;  // IK系统的Rig
    
    private Vector3 startPosition;
    private Vector3 verticalEndPosition;
    private Vector3 horizontalEndPosition;
    private int currentSegment = 0;
    private bool _isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = handIKConstraint.data.target.position; 
        verticalEndPosition = startPosition + new Vector3(0, -0.05f, 0);  // 竖线终点
       
    }

    // Update is called once per frame
    void Update()
    {
        // 按下空格键开始运动
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isMoving = true;
        }
        
        // 控制手部运动
        if (_isMoving)
        {
            MoveHand(startPosition, verticalEndPosition);
        }
        
    }
    void MoveHand(Vector3 from, Vector3 to)
    {
        // 让手部逐渐移动到目标位置
        handIKConstraint.data.target.position = Vector3.MoveTowards(handIKConstraint.data.target.position, to, Time.deltaTime * 0.001f);

        // 当手部到达目标位置时，切换到下一个线段
        if (Vector3.Distance(handIKConstraint.data.target.position, to) < 0.001f)
        {
            currentSegment++;
        }

        // 当完成所有段落后，停止运动
        if (currentSegment > 0)
        {
            _isMoving = false;
        }
    }
}
