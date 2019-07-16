using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/// <summary>/// 存储寻路点信息/// </summary>/// 
public class AStarPoint
{    //父“格子”    
    public AStarPoint mParentPoint { get; set; }
    //格子显示对象    
    public GameObject mGameObject { get; set; }

    public float mF { get; set; }
    public float mG { get; set; }
    public float mH { get; set; }

    //点的位置    
    public Vector2 mPosition { get; set; }
    public int mPositionX { get; set; }
    public int mPositionY { get; set; }

    //该点是否处于障碍物   
    public bool mIsObstacle { get; set; }
    public AStarPoint(int positionX,int positionY)
    {
        this.mPositionX = positionX;
        this.mPositionY = positionY;
        this.mPosition = new Vector2(mPositionX, mPositionY);
        this.mParentPoint = null;
    }
}

