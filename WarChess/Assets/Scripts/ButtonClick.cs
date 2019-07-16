using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    public void OnAttack()
    {
        GameController.status = GameController.Status.PreAttack;
    }

    public void OnSkill()
    {
        GameController.status = GameController.Status.OnSkillList;
    }

    public void OnStandby()
    {
        GameController.status = GameController.Status.OnStandby;
    }

    public void OnTurnOver()
    {
        GameController.status = GameController.Status.OnTurnOver;
    }
}
