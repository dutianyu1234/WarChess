using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 该脚本控制游戏的进程
/// </summary>

public class GameController : MonoBehaviour
{
    //GameController的单例对象
    public static GameController instance = null;

    /// <summary>
    /// 宏观组件和变量
    /// </summary>
    public GameObject ActionMenu;//选择菜单
    public BoardManager boardScript;//棋盘对象脚本文件
    public static int nowRound;//现在的回合数
    private Properties.Group nowActionGroup;//现在能够移动的阵营
    public static Status status;//当前的状态


    /// <summary>
    /// 每回合需要的临时变量
    /// </summary>
    private GameObject NowObj = null;//被选中的单位

    private int SkillorItemNum = 0;//被选中的技能或物品序号
    GameObject SkillsMenu = null;//技能目录

    bool isMoving = false;//是否正在移动

    Vector3 beforeMovePos = Vector3.zero;    //记录移动前的位置

    //存储行动方格
    private List<GameObject> MoveGridList = new List<GameObject>();
    private List<GameObject> AttackGridList = new List<GameObject>();
    private List<GameObject> SkillGridList = new List<GameObject>();

    private List<GameObject> UnitList = new List<GameObject>();
    private int MoveOverNum = 0;


    /// <summary>
    /// 功能性变量
    /// </summary>
    public enum Status
    {
        OnTurnStart,
        None,
        PreMove,
        Moving,
        MenuDisplay,
        PreAttack,
        OnAttack,
        OnSkillList,//显示技能菜单
        PreSkill,//显示技能范围
        OnSkill,
        OnItemList,//显示物品菜单
        PreItem,//显示物品范围
        OnStandby,
        OnActionOver,
        OnTurnOver
    }
    public enum SquareType
    {
        Move,
        Battle,
        Skill
    }
    Vector3 mousePos;//鼠标位置
    GameObject message;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance!=this)
        {
            Destroy(gameObject);
        }

        boardScript = GetComponent<BoardManager>();

        ActionMenu = GameObject.FindGameObjectWithTag("ActionMenu");
        message = GameObject.FindGameObjectWithTag("Message");


        nowRound = 1;
        nowActionGroup = Properties.Group.Self;
        status = Status.OnTurnStart;
    }

    // Start is called before the first frame update
    void Start()
    {
        boardScript.SetBoard(1);
    }

    private void Update()
    {
        message.GetComponent<Text>().text = "当前回合数：" + nowRound + "\n当前行动阵营：" + nowActionGroup;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = IfCorrectPostion(mousePos);

        //回合开始时，初始化单位列表，并对Buff进行统一处理。
        if (status == Status.OnTurnStart)
        {
            //初始化单位列表
            if (nowActionGroup == Properties.Group.Self) UnitList = boardScript.SelvesList;
            else if (nowActionGroup == Properties.Group.Enemy) UnitList = boardScript.EnemiesList;
            else if (nowActionGroup == Properties.Group.Friend) UnitList = boardScript.FriendsList;

            for (int i = 0; i < UnitList.Count; i++)
            {
                UnitList[i].GetComponent<Properties>().ifmoved = false;
            }

            BuffHandler();
            status = Status.None;
        }

        //初始状态，等待单位选中
        if (status == Status.None) CheckClick();

        //单位已选中，等待移动指令
        else if (status == Status.PreMove)
        {
            if (MoveGridList.Count == 0) PreMove();
            CheckMoveCommand();
        }

        //移动中
        else if (status == Status.Moving)
        {
            CheckArrival();
        }

        //移动完毕，显示功能菜单
        else if (status == Status.MenuDisplay)
        {
            if (!ActionMenu.transform.GetChild(0).gameObject.activeSelf)
            DisplayMenu();
        }

        //选择攻击命令，显示攻击范围
        else if (status == Status.PreAttack)
        {
            if (ActionMenu.transform.GetChild(0).gameObject.activeSelf)
            {
                foreach (Transform child in ActionMenu.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            if (AttackGridList.Count == 0) AttackGridList = ShowAttackRange(NowObj.GetComponent<WeaponCarry>().Weapons[0].GetComponent<Weapon>());
            CheckCommand(SquareType.Battle);
        }

        //执行攻击命令
        else if (status == Status.OnAttack)
        {
            BattleHandler();
            status = Status.OnActionOver;
        }

        //选择技能命令，显示技能菜单
        else if (status == Status.OnSkillList)
        {
            if (SkillsMenu == null)
            {
                SkillsMenu = DisplaySkillList(NowObj);

                foreach (Transform child in ActionMenu.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        //已选择技能，显示技能范围
        else if (status == Status.PreSkill)
        {
            CheckCommand(SquareType.Skill);
        }

        //执行技能命令
        else if (status == Status.OnSkill)
        {
            if (SkillHangdler())
            {
                NowObj.GetComponent<Properties>().experience += 30;
                status = Status.OnActionOver;
            }

        }

        //执行待机命令
        else if (status == Status.OnStandby)
        {
            foreach (Transform child in ActionMenu.transform)
            {
                child.gameObject.SetActive(false);
            }
            status = Status.OnActionOver;
        }

        else if (status == Status.OnActionOver)
        {
            NowObj.GetComponent<Properties>().ifmoved = true;
            NowObj = null;
            MoveOverNum += 1;

            status = Status.None;
        }

        else if (status == Status.OnTurnOver)
        {
            if (NowObj != null) NowObj = null;

            MoveOverNum = 0;

            nowActionGroup = 1 - nowActionGroup;

            if (nowActionGroup == Properties.Group.Self) nowRound += 1;

            status = Status.OnTurnStart;
        }

        //如果所有单位行动完毕，此回合结束
        if (!(MoveOverNum < UnitList.Count))
        {
            status = Status.OnTurnOver;
        }

        //如果鼠标右键，返回上一个状态
        if (Input.GetMouseButtonDown(1))
        {
            ReverseStatus();
        }
    }


    #region  基本操作功能实现函数
    //Buff处理函数
    public void BuffHandler()
    {  
        if (UnitList.Count != 0)
        {
            for (int i = 0; i < UnitList.Count; i++)
            {
                List<GameObject> Buffs = UnitList[i].GetComponent<BuffCarry>().Buffs;
                if (Buffs.Count != 0)
                {
                    for (int j = 0; j < Buffs.Count; j++)
                    {
                        if (Buffs[j]!=null)
                        Buffs[j].GetComponent<Buff>().MyEffect(UnitList[i]);
                    }
                }
            }
        }

    }

    //检测输入，查看是否移动
    public void CheckClick()
    {
        if (NowObj == null)
        {
            if ((mousePos.x > -1 && mousePos.x < boardScript.board.GetLength(0)) && (mousePos.y > -1 && mousePos.y < boardScript.board.GetLength(1)))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 0] != null)
                    {
                        if (!boardScript.board[(int)mousePos.x, (int)mousePos.y, 0].GetComponent<Properties>().ifmoved)
                        {
                            if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 0].GetComponent<Properties>().group == nowActionGroup)
                            {
                                NowObj = boardScript.board[(int)mousePos.x, (int)mousePos.y, 0];
                                status = Status.PreMove;
                            }
                        }
                    }
                }
            }
        }
    }

    //准备移动
    public void PreMove()
    {
        MoveGridList = new List<GameObject>();
        MoveGridList = BirthSpace(NowObj.transform.position, 5, Type.Diamond, SquareType.Move);
    }

    //检测是否发出移动指令
    public void CheckMoveCommand()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 1] != null)
            {
                if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 1].tag == "MoveSpace")
                {
                    if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 0] == null)
                    {
                        MovingObj();
                        status = Status.Moving;
                    }
                }
            }
        }
    }

    //单位移动
    public void MovingObj()
    {
        beforeMovePos = NowObj.transform.position;

        List<Vector3> path = AStarInterface.GetInstance.GetPath(NowObj.transform.position, mousePos);

        DestroyGrid(MoveGridList);
        StartCoroutine(StepMove(NowObj, path));
    }

    //检测是否移动到目的地
    public void CheckArrival()
    {
        if (!isMoving)
        {
            boardScript.board[(int)beforeMovePos.x, (int)beforeMovePos.y, 0] = null;
            boardScript.board[(int)NowObj.transform.position.x, (int)NowObj.transform.position.y, 0] = NowObj;
            status = Status.MenuDisplay;
        }
    }

    //显示选项菜单
    public void DisplayMenu()
    {
        foreach(Transform child in ActionMenu.transform)
        {
            child.gameObject.SetActive(true);
        }
        ActionMenu.transform.position = Camera.main.WorldToScreenPoint(NowObj.transform.position);
    }

    //显示攻击范围
    public List<GameObject> ShowAttackRange(Weapon myWeapon)
    {
        List<GameObject> templist = new List<GameObject>();
        Type attackRangeType = myWeapon.myRangeType;
        int range = myWeapon.range;
        int sideRange = myWeapon.sideRange;

        templist = BirthSpace(NowObj.transform.position, range, attackRangeType, SquareType.Battle, sideRange);

        return templist;
    }

    //检测攻击或技能命令
    public void CheckCommand(SquareType squareType)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (squareType == SquareType.Battle)
            {
                if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 1].tag == "AttackSpace")
                {
                    if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 0] != null)
                    {
                        DestroyGrid(AttackGridList);
                        status = Status.OnAttack;
                    }

                }
            }

            else if (squareType == SquareType.Skill)
            {
                if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 1].tag == "SkillSpace")
                {
                    if (boardScript.board[(int)mousePos.x, (int)mousePos.y, 0] != null)
                    {
                        DestroyGrid(SkillGridList);
                        status = Status.OnSkill;
                    }
                }
            }
        }
    }

    //战斗处理
    public void BattleHandler()
    {
        GameObject DefObj = boardScript.board[(int)mousePos.x, (int)mousePos.y, 0];
        List<Vector3> moveEffect = new List<Vector3>() { DefObj.transform.position, DefObj.transform.position + new Vector3(0.5f, 0, 0), DefObj.transform.position, DefObj.transform.position + new Vector3(-0.5f, 0, 0) };

        StartCoroutine(StepMove(DefObj, moveEffect));

        //武器威力*攻击力-防御力
        float damage = NowObj.GetComponent<WeaponCarry>().Weapons[0].GetComponent<Weapon>().damage * NowObj.GetComponent<Properties>().Attack * NowObj.GetComponent<Properties>().AttackGain - DefObj.GetComponent<Properties>().Defense;
        DefObj.GetComponent<Properties>().HP -= (int)damage;


        //确定是否能够反击并计算反击伤害
        bool CanCounterFlag = false;
        AttackGridList = ShowAttackRange(DefObj.GetComponent<WeaponCarry>().Weapons[0].GetComponent<Weapon>());

        for(int i = 0; i < AttackGridList.Count; i++)
        {
            AttackGridList[i].GetComponent<MeshRenderer>().enabled = false;
            if (AttackGridList[i].transform.position.x == NowObj.transform.position.x && AttackGridList[i].transform.position.y == NowObj.transform.position.y) CanCounterFlag = true;
        }

        if (CanCounterFlag)
        {
            float counterdamage = DefObj.GetComponent<WeaponCarry>().Weapons[0].GetComponent<Weapon>().damage * DefObj.GetComponent<Properties>().Attack * DefObj.GetComponent<Properties>().AttackGain - NowObj.GetComponent<Properties>().Defense;
            NowObj.GetComponent<Properties>().HP -= (int)counterdamage;
        }
        DestroyGrid(AttackGridList);
    }

    public GameObject DisplaySkillList(GameObject Obj)
    {
        GameObject Menu;
        GameObject ViewPoint;
        GameObject Content;

        float height = 0, width = 0;

        Menu = Resources.Load("Menu") as GameObject;

        Menu.transform.localPosition = GameObject.Find("Canvas").transform.position;

        Menu = Instantiate(Menu);
        Menu.transform.SetParent(GameObject.Find("Canvas").transform);

        ViewPoint = Menu.transform.GetChild(1).GetChild(0).gameObject;
        Content = Menu.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

        List<GameObject> skillList = Obj.GetComponent<SkillCarry>().Skills;

        for (int i = 0; i < skillList.Count; i++)
        {
            GameObject skill = new GameObject(skillList[i].GetComponent<Skill>().myname, typeof(Image), typeof(Button));

            skill.GetComponent<Image>().sprite = skillList[i].GetComponent<Skill>().myTextuer;

            skill.transform.SetParent(Content.transform);

            height += skill.GetComponent<RectTransform>().sizeDelta.y;

            width = skill.GetComponent<RectTransform>().sizeDelta.x;

            GameObject temp = skillList[i];
            int k = i;

            skill.GetComponent<Button>().onClick.AddListener(delegate () { ShowSkillRange(temp,k);});
        }

        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        ViewPoint.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 2 * height / skillList.Count);


        Menu.transform.position = Camera.main.WorldToScreenPoint(Obj.transform.position);


        return Menu;
    }

    public void ShowSkillRange(GameObject Skill,int num)
    {
        Destroy(SkillsMenu);
        SkillsMenu = null;
        SkillGridList = BirthSpace(NowObj.transform.position, Skill.GetComponent<Skill>().range, Skill.GetComponent<Skill>().myRangeType, SquareType.Skill);
        SkillorItemNum = num;

        status = Status.PreSkill;
    }

    public bool SkillHangdler()
    {
        GameObject DefObj = boardScript.board[(int)mousePos.x, (int)mousePos.y, 0];

        NowObj.GetComponent<Properties>().LastSkill = NowObj.GetComponent<SkillCarry>().Skills[SkillorItemNum];

        bool ifsuccess = NowObj.GetComponent<SkillCarry>().Skills[SkillorItemNum].GetComponent<Skill>().MySkill(NowObj, DefObj);

        return ifsuccess;
    }

    #endregion

    #region   功能函数Utils
    //延时函数
    public IEnumerator WaitForSecend(float waitTime)
    {
        float time = 0;
        while (time < waitTime)
        {
            yield return null;
        }
    }

    //正则鼠标位置，若不为整数，调整为整数。
    public Vector3 IfCorrectPostion(Vector3 self)
    {
        if (self.x < 0) self.x = 0;
        if (self.y < 0) self.y = 0;
        if (self.z < 0) self.z = 0;
        float x = Mathf.Floor(self.x + 0.5f);
        float y = Mathf.Floor(self.y + 0.5f);
        float z = Mathf.Floor(self.z + 0.5f);
        return new Vector3(x, y, z);
    }

    //缓步移动
    IEnumerator StepMove(GameObject Obj, List<Vector3> path)
    {
        isMoving = true;
        int i = path.Count-1;
        while (i>-1)
        {
            Obj.transform.position = path[i];
            yield return new WaitForSeconds(0.2f);
            i -= 1;
        }
        isMoving = false;
    }


    /// <summary>
    /// 产生指定范围的方格
    /// </summary>
    /// 参数：
    /// Vector3 center:产生方格的中心点。
    /// int Range:产生的方格范围。
    /// enum Type:产生方格类型，分为菱形和矩形。
    /// enum SquareType:产生的方格类型，分为移动，攻击和技能
    /// int sideRange:内圈半径,诸如弓箭之类无法攻击邻近单位
    /// 返回值：
    /// 返回生成方格的索引列表，方便销毁。
    public List<GameObject> BirthSpace(Vector3 center, int Range, Type type, SquareType squareType, int sideRange = 0)
    {
        List<GameObject> tempList = new List<GameObject>();
        Transform Spaces = new GameObject("Spaces").transform;
        GameObject cell = Resources.Load("Prefabs/FunctionSpaces/" + squareType.ToString()) as GameObject;

        if (squareType == SquareType.Move)
        {
            List<Vector3> ConfirmedList = new List<Vector3>();
            List<Vector3> tobeConfirmedList = new List<Vector3>();
            List<Vector3> specialList = new List<Vector3>();
            int BirthNum = 0;

            for (int i = 0; i < Range + 1; i++)
            {
                //初始状态，将起始位置加入已确认列表
                if (ConfirmedList.Count == 0)
                {
                    ConfirmedList.Add(center);
                    BirthNum = 1;
                }
                else
                {
                    //搜索已确认列表周围方格，加入待确认列表
                    for (int k = ConfirmedList.Count - BirthNum; k < ConfirmedList.Count; k++)
                    {
                        Vector3 posUp = new Vector3(ConfirmedList[k].x, ConfirmedList[k].y + 1, -1);
                        Vector3 posDown = new Vector3(ConfirmedList[k].x, ConfirmedList[k].y - 1, -1);
                        Vector3 posLeft = new Vector3(ConfirmedList[k].x - 1, ConfirmedList[k].y, -1);
                        Vector3 posRight = new Vector3(ConfirmedList[k].x + 1, ConfirmedList[k].y, -1);

                        if (!ConfirmedList.Contains(posUp) && !tobeConfirmedList.Contains(posUp))
                            tobeConfirmedList.Add(posUp);

                        if (!ConfirmedList.Contains(posDown) && !tobeConfirmedList.Contains(posDown))
                            tobeConfirmedList.Add(posDown);

                        if (!ConfirmedList.Contains(posLeft) && !tobeConfirmedList.Contains(posLeft))
                            tobeConfirmedList.Add(posLeft);

                        if (!ConfirmedList.Contains(posRight) && !tobeConfirmedList.Contains(posRight))
                            tobeConfirmedList.Add(posRight);
                    }

                    BirthNum = 0;

                    //检测待确认列表的方格是否满足可移动
                    for (int j = 0; j < tobeConfirmedList.Count; j++)
                    {
                        //如果越界，显然不能移动
                        if (tobeConfirmedList[j].x < 0 || tobeConfirmedList[j].x > boardScript.Width - 1) continue;
                        else if (tobeConfirmedList[j].y < 0 || tobeConfirmedList[j].y > boardScript.Height - 1) continue;

                        //要满足可移动，需要两个条件：地形可移动，并且该区域无敌人
                        if (boardScript.board[(int)tobeConfirmedList[j].x, (int)tobeConfirmedList[j].y, 2].GetComponent<Terrain>().ifCanMove)
                        {
                            //区域无单位
                            if (boardScript.board[(int)tobeConfirmedList[j].x, (int)tobeConfirmedList[j].y, 0] == null)
                            {
                                ConfirmedList.Add(tobeConfirmedList[j]);
                                BirthNum += 1;

                            }
                            //区域单位不是敌人
                            else
                            {
                                if (boardScript.board[(int)tobeConfirmedList[j].x, (int)tobeConfirmedList[j].y, 0].GetComponent<Properties>().group == NowObj.GetComponent<Properties>().group)
                                {
                                    ConfirmedList.Add(tobeConfirmedList[j]);
                                    BirthNum += 1;
                                }
                            }
                        }
                    }

                    //清空待确认列表
                    tobeConfirmedList.Clear();
                }
            }

            for (int i = 0; i < ConfirmedList.Count; i++)
            {
                GameObject tempCell = Instantiate(cell);
                tempCell.transform.position = new Vector3(ConfirmedList[i].x, ConfirmedList[i].y, 0f);
                tempList.Add(tempCell);
                boardScript.board[(int)ConfirmedList[i].x, (int)ConfirmedList[i].y, 1] = tempCell;
                tempCell.transform.SetParent(Spaces);
            }
        }

        else if (type == Type.Square)
        {
            for (int i = -Range; i < Range + 1; ++i)
            {
                for (int j = -Range; j < Range + 1; ++j)
                {
                    if ((center.x + i) < 0 || (center.x + i) > boardScript.Width - 1) continue;
                    if ((center.y + j) < 0 || (center.y + j) > boardScript.Height - 1) continue;

                    GameObject tempCell = Instantiate(cell);
                    tempCell.transform.position = new Vector3(center.x + i, center.y + j, 0);
                    tempList.Add(tempCell);
                    boardScript.board[(int)center.x + i, (int)center.y + j, 1] = tempCell;
                    tempCell.transform.SetParent(Spaces);
                }
            }
        }

        else if (type == Type.Diamond)
        {
            int temp = 0;
            for (int i = -Range; i < Range + 1; ++i)
            {
                temp = Range - Mathf.Abs(i);
                for (int j = -temp; j < temp + 1; ++j)
                {
                    if ((Mathf.Abs(i) < (sideRange + 1) || Mathf.Abs(j) < (sideRange + 1)) && (Mathf.Abs(i) + Mathf.Abs(j) < sideRange + 1)) continue;
                    if ((center.x + i) < 0 || (center.x + i) > boardScript.Width - 1) continue;
                    if ((center.y + j) < 0 || (center.y + j) > boardScript.Height - 1) continue;

                    GameObject tempCell = Instantiate(cell);
                    tempCell.transform.position = new Vector3(center.x + i, center.y + j, 0f);
                    tempList.Add(tempCell);
                    boardScript.board[(int)center.x + i, (int)center.y + j, 1] = tempCell;
                    tempCell.transform.SetParent(Spaces);
                }
            }
        }

        return tempList;
    }

    //检测移动格周围环境
    public bool CheckAround(Vector3 center)
    {
        bool ifBayonet = false;
        bool LRBayonet = false;
        bool UDBayonet = false;

        LRBayonet = boardScript.board[(int)(center.x + 1), (int)center.y, 2].GetComponent<Terrain>().ifCanMove || boardScript.board[(int)(center.x - 1), (int)center.y, 2].GetComponent<Terrain>().ifCanMove;
        UDBayonet = boardScript.board[(int)center.x, (int)(center.y - 1), 2].GetComponent<Terrain>().ifCanMove || boardScript.board[(int)center.x, (int)(center.y + 1), 2].GetComponent<Terrain>().ifCanMove;

        if (LRBayonet || UDBayonet) ifBayonet = true;

        return ifBayonet;
    }

    //销毁行动格
    public void DestroyGrid(List<GameObject> Grids)
    {
        GameObject parent = Grids[0].transform.parent.gameObject;
        for (int i = 0; i < Grids.Count; i++)
        {
            Destroy(Grids[0]);
            boardScript.board[(int)Grids[0].transform.position.x, (int)Grids[0].transform.position.y, 1] = null;
        }
        Destroy(parent);
        Grids.Clear();
    }

    public void ReverseStatus()
    {
        switch (status)
        {
            case Status.None:
                break;

            case Status.PreMove:
                NowObj = null;
                DestroyGrid(MoveGridList);
                status = Status.None;
                break;

            case Status.Moving:
                boardScript.board[(int)beforeMovePos.x, (int)beforeMovePos.y, 0] = NowObj;
                boardScript.board[(int)NowObj.transform.position.x, (int)NowObj.transform.position.y, 0] = null;
                NowObj.transform.position = beforeMovePos;
                status = Status.PreMove;
                break;

            case Status.MenuDisplay:
                foreach (Transform child in ActionMenu.transform)
                {
                    child.gameObject.SetActive(false);
                }

                if (NowObj.transform.position != beforeMovePos)
                {
                    boardScript.board[(int)beforeMovePos.x, (int)beforeMovePos.y, 0] = NowObj;
                    boardScript.board[(int)NowObj.transform.position.x, (int)NowObj.transform.position.y, 0] = null;
                    NowObj.transform.position = beforeMovePos;
                }

                status = Status.PreMove;
                break;

            case Status.PreAttack:
                DestroyGrid(AttackGridList);
                status = Status.MenuDisplay;
                break;

            case Status.OnAttack:
                break;

            case Status.OnSkillList:
                Destroy(SkillsMenu);
                status = Status.MenuDisplay;
                break;

            case Status.PreSkill:
                DestroyGrid(SkillGridList);
                status = Status.OnSkillList;
                break;

            case Status.OnSkill:
                status = Status.OnSkillList;
                break;

            default:
                break;
        }
    }
    #endregion




}
