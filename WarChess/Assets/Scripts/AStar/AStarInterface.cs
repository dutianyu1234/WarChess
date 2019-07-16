using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarInterface : MonoBehaviour
{
    private static AStarInterface instance;

    public static AStarInterface GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new AStarInterface();
            }
            return instance;
        }
    }

    public List<Vector3> GetPath(Vector3 startPos,Vector3 endPos)
    {
        AStarAlgorithm.GetInsatnce.InitPoint();

        AStarPoint startPoint = AStarAlgorithm.GetInsatnce.mPointGrid[(int)startPos.x, (int)startPos.y];
        AStarPoint endPoint = AStarAlgorithm.GetInsatnce.mPointGrid[(int)endPos.x, (int)endPos.y];

        List<AStarPoint> AStarPath = new List<AStarPoint>();
        List<Vector3> Path = new List<Vector3>();

        if (startPos.x == endPos.x && startPos.y == endPos.y)
        {
            AStarPath.Add(new AStarPoint((int)startPos.x, (int)startPos.y));
        }
        else
            AStarPath = AStarAlgorithm.GetInsatnce.FindPath(startPoint, endPoint);

        for (int i = 0; i < AStarPath.Count; i++)
        {
            Path.Add(new Vector3(AStarPath[i].mPositionX, AStarPath[i].mPositionY, -1));
        }

        AStarAlgorithm.GetInsatnce.ClearGrid();
        Destroy(AStarAlgorithm.GetInsatnce.Path);
        
        return Path;
    }
}
