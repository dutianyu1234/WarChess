using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 该脚本定义了多个单位属性的抽象类。
/// </summary>

//人物属性的抽象类
[System.Serializable]
public class Properties:MonoBehaviour
{
    public enum Group
    {
        Self,
        Enemy,
        Friend
    }

    public enum Occupation
    {
        Warrior,
        Mage,
        Assassin
    }

    //人物基本属性
    public string myName;//名字
    public Group group;//阵营
    public Occupation occupation;//职业
    public int level;//当前等级
    public int experience;//当前经验值


    public bool ifmoved;//是否行动完毕

    //人物战斗属性
    public int HP;//生命值
    public int MP;//魔法值
    public int Attack;//攻击
    public int Defense;//防御
    public int MagicPower;//魔力
    public int MagicDefense;//魔防
    public int Speed;//速度
    public int move;//行动力


    //人物Buff属性
    [HideInInspector] public float AttackGain;
    [HideInInspector] public float DefenseGain;
    [HideInInspector] public float MagicPowerGain;
    [HideInInspector] public float MagicDefenseGain;
    [HideInInspector] public float SpeedGain;
    [HideInInspector] public float MoveGain;

    [HideInInspector] public float PowerGain;
    [HideInInspector] public float BuffEffectGain;
    [HideInInspector] public float BuffDurationGain;
    [HideInInspector] public float RegenerationGain;

    public float AvoidRate;//闪避率

    public GameObject LastSkill = null;//上一次使用的技能

    public Properties() { }
}


//范围的类型
public enum Type
{
    Square,
    Diamond
}

//武器的抽象类
[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    //武器类型的枚举
    public enum WeaponType
    {
        Sword,
        Spear,
        Axe,
        Bow,
        Staff,
        Firegun
    }

    //伤害类型的枚举
    public enum DamageType
    {
        physics,
        magic
    }



    public string myName;
    public WeaponType weaponType;//武器的类别。剑，枪，斧，弓，法杖，火枪
    public Sprite myTexture;//武器贴图
    public string description;//武器的描述

    public DamageType damageType;//伤害类型
    public float damage;//武器的威力
    public int range;//武器的攻击范围
    public int sideRange;
    public Type myRangeType;//武器攻击范围的类型

    public abstract void MySkill(GameObject FromObj, GameObject TargetObj);//武器特性
}



//技能的抽象类
[System.Serializable]
public abstract class Skill : MonoBehaviour
{
    public enum Target
    {
        Self,
        Enemy,
        Friend,
        SelfandFriend,
        All
    }

    public enum SkillType
    {
        Damage,
        Buff,
        Recover
    }

    public string myname;//技能名称
    public Sprite myTextuer;//技能贴图
    public string description; //技能描述

    public Target target;//技能作用对象
    public SkillType skillType;//技能类型

    public int range;//施法范围
    public Type myRangeType;//施法范围类型

    public abstract bool MySkill(GameObject FromObj, GameObject TargetObj);
}



//Buff的抽象类
[System.Serializable]
public abstract class Buff : MonoBehaviour
{
    public string myname;  //Buff的名字
    public Sprite myTexture;//Buff的贴图
    public string description;//Buff的描述

    public int duration;   //Buff的持续时间

    public Buff() {}

    public abstract void MyEffect(GameObject Obj);//Buff特效 
    public abstract void MyOnEffect(GameObject Obj);//Buff附身时效果
    public abstract void OnDestroyBuff();//Buff消失时间
}


//地形的抽象类
[System.Serializable]
public abstract class Terrain : MonoBehaviour
{
    public string myName;//地形的名称

    public bool ifCanMove;//是否可移动
    public int MoveConsumption;//进入该地形的移动力消耗

    public abstract void Effect(GameObject TargetObj);
}