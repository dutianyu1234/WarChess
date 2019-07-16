using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSteal : Skill
{
    private int NowNum = 0;
    private int MaxNum = 3;

    //所有窃取的技能施法距离变为1，方形，威力增加
    public override bool MySkill(GameObject FromObj, GameObject TargetObj)
    {
        //如果作用对象不对
        if (!CheckTarget(TargetObj,target))
        {
            GameController.instance.ReverseStatus();
            return false;
        }

        //如果目标上一次没有使用法术
        if (TargetObj.GetComponent<Properties>().LastSkill == null)
        {
            GameController.instance.ReverseStatus();
            return false;
        }


        //方便统一管理的skill根对象，放到施法者目录下
        GameObject skillList = null;
        if (FromObj.transform.childCount==0) skillList = new GameObject("Skill");
        else
        {
            foreach(Transform child in FromObj.transform)
            {
                if (child.name == "Skill")
                {
                    skillList = child.gameObject;
                }
            }

            if (skillList == null)
            {
                skillList = new GameObject("Skill");
            }
        }
        skillList.transform.SetParent(FromObj.transform);

        //创建窃取技能的副本
        GameObject lastSkill = TargetObj.GetComponent<Properties>().LastSkill;
        GameObject stealedSkill = Instantiate(lastSkill, new Vector3(0, 0, 0), Quaternion.identity);
     
        //修改窃取技能的属性
        stealedSkill.GetComponent<Skill>().range = 1;
        stealedSkill.GetComponent<Skill>().myRangeType = Type.Square;

        if (NowNum < MaxNum)
        {
            FromObj.GetComponent<SkillCarry>().Skills.Add(stealedSkill);
            stealedSkill.transform.SetParent(skillList.transform);
            NowNum += 1;
        }

        else
        {
            GameObject FirstSkill = FromObj.transform.GetChild(0).GetChild(0).gameObject;

            FromObj.GetComponent<SkillCarry>().Skills.Remove(FirstSkill);
            Destroy(FirstSkill);
            FromObj.GetComponent<SkillCarry>().Skills.Add(stealedSkill);
            stealedSkill.transform.SetParent(skillList.transform);
        }

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
            else
            {
                flag = false;
                Debug.Log("该技能只能作用于我方");
            }

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
