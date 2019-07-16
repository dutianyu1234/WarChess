using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffCarry : MonoBehaviour
{
    public List<GameObject> Buffs;
    private int num = 0;

    private void Awake()
    {
        if (Buffs == null) Buffs = new List<GameObject>();
    }

    private void Update()
    {
        if (GameController.status == GameController.Status.None)
        {
            if (Buffs.Count != 0)
            {
                for (int i = 0; i < Buffs.Count; i++)
                {
                    if (Buffs[i] != null)
                    {
                        Buffs[i].GetComponent<Buff>().OnDestroyBuff();
                    }
                    else num += 1;
                }

                if (num == Buffs.Count)
                {
                    Buffs.Clear();
                }
            }
        }
    }
}
