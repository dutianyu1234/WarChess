using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassDictionary : MonoBehaviour
{
    public static Dictionary<string, System.Type> dic;

    // Start is called before the first frame update
    void Start()
    {
        dic = new Dictionary<string, System.Type>();

        
        dic.Add("FireBall", typeof(FireBall));
        dic.Add("Hypertoxicity", typeof(Hypertoxicity));
        dic.Add("SkillSteal", typeof(SkillSteal));
        dic.Add("Spark", typeof(Spark));
        /*
        dic.Add("", typeof());
        dic.Add("", typeof());*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
