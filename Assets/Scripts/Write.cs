using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging; 
using UnityEngine;

public class Write : MonoBehaviour
{
    
    //public Transform penTip;  
    
    public TwoBoneIKConstraint handIKConstraint;  
    //public Rig rig;  
    
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
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isMoving = true;
        }
        
        if (_isMoving)
        {
            MoveHand(startPosition, verticalEndPosition);
        }
        
    }
    void MoveHand(Vector3 from, Vector3 to)
    {
        handIKConstraint.data.target.position = Vector3.MoveTowards(handIKConstraint.data.target.position, to, Time.deltaTime * 0.001f);

        if (Vector3.Distance(handIKConstraint.data.target.position, to) < 0.001f)
        {
            currentSegment++;
        }

        if (currentSegment > 0)
        {
            _isMoving = false;
        }
    }
}
