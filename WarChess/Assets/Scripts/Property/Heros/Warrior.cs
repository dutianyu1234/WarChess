using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Properties
{
    private int HpUp = 20;
    private int MpUp = 10;
    private int AttackUp = 2;
    private int DefenseUp = 2;
    private int MagicPowerUp = 1;
    private int MagicDefenseUp = 1;
    private int SpeedUp = 2;

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

    private void Update()
    {
        if (!(experience < 100))
        {
            Debug.Log("升级了");
            level += 1;
            experience -= 100;

            HP += HpUp;
            MP += MpUp;
            Attack += AttackUp;
            Defense += DefenseUp;
            MagicPower += MagicPowerUp;
            MagicDefense += MagicDefenseUp;
            Speed += SpeedUp;
        }

        if (level > 10)
        {

        }
    }
}
