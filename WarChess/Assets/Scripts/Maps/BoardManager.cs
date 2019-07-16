using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 该脚本用于生成完整的地图和单位。
/// </summary>


public class BoardManager : MonoBehaviour
{
    public GameObject[,,] board;
    int[,] BoardData;
    public int Height, Width;

    //存储单位的引用
    public List<GameObject> HerosList = new List<GameObject>();
    public List<GameObject> SelvesList = new List<GameObject>();
    public List<GameObject> EnemiesList = new List<GameObject>();
    public List<GameObject> FriendsList = new List<GameObject>();

    public enum Terrain
    {
        None,
        Fire,
        Grass,
        Water,
        Plane
    }

    //读取棋盘数据
    //参数：
    //boardnumber：储存的棋盘编号
    void Readcsv(int boardnumber)
    {
        string filename = Application.dataPath + "/Maps/Board" + boardnumber + ".csv";
        string[] fileData = File.ReadAllLines(filename);

        Height = fileData.Length;
        Width = fileData[0].Split(',').Length;

        BoardData = new int[Width, Height];

        for (int i = fileData.Length - 1; i > -1; i--)
        {
            string[] keys = fileData[i].Split(',');
            for (int j = 0; j < keys.Length; j++)
            {
                BoardData[j, fileData.Length - i - 1] = int.Parse(keys[j]);
            }
        }
    }

    //将单位加入相应的列表，方便索引
    void AddList(GameObject Obj)
    {
        Properties.Group myGroup = Obj.GetComponent<Properties>().group;
        switch (myGroup)
        {
            case Properties.Group.Self:HerosList.Add(Obj);SelvesList.Add(Obj); break;
            case Properties.Group.Enemy: HerosList.Add(Obj); EnemiesList.Add(Obj); break;
            case Properties.Group.Friend: HerosList.Add(Obj); FriendsList.Add(Obj); break;
        }
    }

    //生成棋盘
    public void SetBoard(int level)
    {
        Readcsv(level);

        board = new GameObject[Width, Height, 3];

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                Terrain terrainNum = (Terrain)BoardData[i,j];

                GameObject terrain = Resources.Load("Prefabs/Terrains/" + terrainNum.ToString()) as GameObject;
                terrain = Instantiate(terrain, new Vector3(i, j, 1), Quaternion.identity);
                terrain.transform.SetParent(gameObject.transform);

                board[i, j, 2] = terrain;

                board[i, j, 0] = null;
                board[i, j, 1] = null;
            }
        }

        GameObject hero = Resources.Load("Prefabs/Heros/Warrior") as GameObject;
        hero = Instantiate(hero, new Vector3(2, 1, -1), Quaternion.identity);
        board[2, 1, 0] = hero;
        AddList(hero);
        

        GameObject hero2 = Resources.Load("Prefabs/Heros/Archer") as GameObject;
        hero2 = Instantiate(hero2, new Vector3(3, 3, -1), Quaternion.identity);
        board[3, 3, 0] = hero2;
        AddList(hero2);

        GameObject hero3 = Resources.Load("Prefabs/Heros/SkillMaster") as GameObject;
        hero3 = Instantiate(hero3, new Vector3(0, 0, -1), Quaternion.identity);
        board[0, 0, 0] = hero3;
        AddList(hero3);
    }
}
