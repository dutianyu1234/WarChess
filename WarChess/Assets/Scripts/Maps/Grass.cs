using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : Terrain
{
    public float AvoidRate;

    Vector3 selfPos;
    GameObject OnUnit = null;
    bool OnFlag;

    public override void Effect(GameObject TargetObj)
    {
    }

    private void Awake()
    {
        selfPos = transform.position;
        AvoidRate = 0.5f;
        OnFlag = false;
    }

    private void Update()
    {
        if (GameController.instance.boardScript.board[(int)selfPos.x, (int)selfPos.y, 0] != null)
        {
            OnUnit = GameController.instance.boardScript.board[(int)selfPos.x, (int)selfPos.y, 0];
            if (!OnFlag)
            {
                OnUnit.GetComponent<Properties>().AvoidRate += AvoidRate;
                OnFlag = true;
            }
        }

        else
        {
            if (OnFlag)
            {
                OnFlag = false;
                OnUnit.GetComponent<Properties>().AvoidRate -= AvoidRate;
                OnUnit = null;
            }
        }
    }
}
