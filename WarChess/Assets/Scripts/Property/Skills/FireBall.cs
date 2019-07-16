using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Skill
{
    public float power;

    public override bool MySkill(GameObject FromObj, GameObject TargetObj)
    {
        //如果作用对象不对
        if (!CheckTarget(TargetObj, target))
        {
            GameController.instance.ReverseStatus();
            return false;
        }

        float damage = (FromObj.GetComponent<Properties>().MagicPower * power) - TargetObj.GetComponent<Properties>().MagicDefense;
        TargetObj.GetComponent<Properties>().HP -= (int)damage;

        return true;
    }

    private bool CheckTarget(GameObject Obj, Target target)
    {
        bool flag = false;
        Properties.Group group = Obj.GetComponent<Properties>().group;

        if (target == Target.All) flag = true;

        else if (target == Target.Self)
        {
            if (group == Properties.Group.Self) flag = true;
            else flag = false;
        }

        else if (target == Target.Enemy)
        {
            if (group == Properties.Group.Enemy) flag = true;
            else flag = false;
        }

        else if (target == Target.Friend)
        {
            if (group == Properties.Group.Friend) flag = true;
            else flag = false;
        }

        else if (target == Target.SelfandFriend)
        {
            if (group == Properties.Group.Self || group == Properties.Group.Friend) flag = true;
            else flag = false;
        }

        return flag;
    }
}
