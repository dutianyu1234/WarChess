using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hypertoxicity : Skill
{
    float param = 100;

    public override bool MySkill(GameObject FromObj, GameObject TargetObj)
    {
        //如果作用对象不对
        if (!CheckTarget(TargetObj, target))
        {
            GameController.instance.ReverseStatus();
            return false;
        }

        GameObject poison = Resources.Load("Prefabs/Buffs/Poison") as GameObject;
        poison = Instantiate(poison);

        //方便统一管理的Buff根对象，放到作用者目录下
        GameObject BuffList = null;
        if (TargetObj.transform.childCount == 0) BuffList = new GameObject("Buff");
        else
        {
            foreach (Transform child in TargetObj.transform)
            {
                if (child.name == "Buff")
                {
                    BuffList = child.gameObject;
                }
            }

            if (BuffList == null)
            {
                BuffList = new GameObject("Buff");
            }
        }
        BuffList.transform.SetParent(TargetObj.transform);


        poison.GetComponent<Buff>().duration = DurationCalculate(FromObj, poison.GetComponent<Buff>().duration);

        TargetObj.GetComponent<BuffCarry>().Buffs.Add(poison);
        poison.transform.SetParent(BuffList.transform);

        return true;
    }

    //持续时间计算公式：原持续时间*时间增益*（（魔力+param）/param）
    private int DurationCalculate(GameObject Obj, int originDuration)
    {
        float duration = 0;
        Properties properties = Obj.GetComponent<Properties>();

        duration = properties.BuffDurationGain * originDuration * (properties.MagicPower + param)/param;

        return (int)duration;
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
