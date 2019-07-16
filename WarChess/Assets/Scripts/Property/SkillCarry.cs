using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCarry : MonoBehaviour
{
    public List<GameObject> Skills;

    private void Awake()
    {
        if (Skills == null) Skills = new List<GameObject>();
    }
}
