using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMaster : Properties
{
    public float Gain;//近战法师独占，所有技能增益提升

    private void Awake()
    {
        AttackGain = 1;
        DefenseGain = 1;
        MagicPowerGain = 1;
        MagicDefenseGain = 1;
        SpeedGain = 1;
        MoveGain = 1;

        PowerGain = Gain;
        BuffEffectGain = Gain;
        BuffDurationGain = Gain;
        RegenerationGain = Gain;
    }
}
