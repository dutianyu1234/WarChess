using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCarry : MonoBehaviour
{
    public List<GameObject> Weapons;

    private int maxNum;//允许携带武器的最大数量

    private void Awake()
    {
        maxNum = 2;

        if (Weapons==null)
        Weapons = new List<GameObject>();
    }
}
