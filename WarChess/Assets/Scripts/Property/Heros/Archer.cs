using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Properties
{
    private void Awake()
    {
        AttackGain = 1;
        DefenseGain = 1;
        MagicPowerGain = 1;
        MagicDefenseGain = 1;
        SpeedGain = 1;
        MoveGain = 1;

        PowerGain = 1;
        BuffEffectGain = 1;
        BuffDurationGain = 1;
        RegenerationGain = 1;
    }
}
