using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Buff
{
    private int OnRound;//Buff附身的回合
    private int StayNum;//Buff剩余回合数

    [HideInInspector]public float damage;

    private void Awake()
    {
        duration = 1;
        damage = 15;
        OnRound = GameController.nowRound;
    }

    public override void MyEffect(GameObject Obj)
    {
        if (GameController.status == GameController.Status.OnTurnStart)
            Obj.GetComponent<Properties>().HP -= (int)damage;
    }



    public override void OnDestroyBuff()
    {
        StayNum = duration - (GameController.nowRound - OnRound);
        if (!( StayNum > 0))
        {
            Destroy(gameObject);
        }
    }

    public override void MyOnEffect(GameObject Obj)
    {
        
    }
}
