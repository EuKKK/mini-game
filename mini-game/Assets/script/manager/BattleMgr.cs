using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using sheeps;

public class BattleMgr : MonoBehaviour
{
    public static BattleMgr Instance { get; private set; }
    public const int MAX_NUMBER = 30;
    public GameObject highLight;
    public GameObject highLightD;
    public GameObject highLightS;
    public GameObject character;
    public GameObject ex;
    public GameObject mapInfo;
    public GameObject testPartical;
    public GameObject rotatePartical;
    public Dictionary<int, GameObject> highLightObj = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> landformPos = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> enemyPos = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> characterPos = new Dictionary<int, GameObject>();
    public Dictionary<GameObject, sheep> characterSheep = new Dictionary<GameObject, sheep>();
    public Dictionary<GameObject, sheep> characterMonster = new Dictionary<GameObject, sheep>();
    public int characterAttackX = 0;
    public int characterAttackY = 0;
    public int[,] MapInfo = new int[35, 35];
    public int[,] GamePlayer = new int[35, 35];
    public int maxX;
    public int maxY;
    private Direction[] DangerousTry = new Direction[20];
    private int wayCount;
    private int trapCount;
    private Direction[] way = new Direction[20];
    private int step = 0;
    public int[,] HighLight = new int[35, 35];
    private GameObject moveEnemy;
    private bool isWalk = false;
    private bool isEnemyWalk = false;
    private bool walkType;
    private List<int> enemyPosList = new List<int>();
    public bool ScreenLock = false;
    private float rotX;
    private float rotY;
    private float rotZ;
    public float InitX;
    public float InitY;
    private int sleep;
    public bool exit;


    private RouteObject[,] mapRoute = new RouteObject[35, 35];
    private bool check = true;
    public int CharacterX;
    public int CharacterY;

    private int t;
    private int round;
    private bool camp;
    private int _x;
    private int _y;
    private bool getPoint;
    private GameObject enemy;
    private int enemyCount;
    private bool enemyWalkAdimit = false;



    private int trapDamage = 192;
    private int characterDamge = 180;
    private int barrierDamage = 180;



    enum Direction
    {
        up,
        down,
        left,
        right,
        stand
    }
    struct RouteObject
    {
        public int movePoint;
        public Direction direction;
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

    }

    public void GameStart()
    {
        characterSheep = FormationMgr.Instance.sheep_GO;
        characterMonster = MapMgr.Instance.enemy_GO;
        rotX = ex.transform.eulerAngles.x;
        rotY = ex.transform.eulerAngles.y;
        rotZ = ex.transform.eulerAngles.z;
        highLight = (GameObject)Resources.Load("Prefab/HighLight");
        highLightD = (GameObject)Resources.Load("Prefab/HighLightD");
        highLightS = (GameObject)Resources.Load("Prefab/HighLightS");
        testPartical = (GameObject)Resources.Load("Prefab/testPartical");
        rotatePartical = (GameObject)Resources.Load("Prefab/rotatePartical");
        MapInfo = MapMgr.Instance.MapInfo;
        mapInfo = MapMgr.Instance.mapInfo;
        GamePlayer = MapMgr.Instance.GamePlayer;
        landformPos = MapMgr.Instance.landformPos;
        enemyPos = MapMgr.Instance.enemyPos;
        characterPos = MapMgr.Instance.characterPos;
        maxX = MapMgr.Instance.maxX;
        maxY = MapMgr.Instance.maxY;
        InitX = MapMgr.Instance.InitX;
        InitY = MapMgr.Instance.InitY;
        RoundStart();
        exit = false;

        round = 1;
        camp = true;
    }


    /// <summary>
    /// 角色寻路模块
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CharacterCheck(int locationX, int locationY, ref GameObject target)
    {
        bool sheep = false;
        if (target.tag == "character")
        {
            sheep = false;
        }
        else if (target.tag == "sheep")
        {
            sheep = true;
        }

        if (GamePlayer[locationX, locationY] == 1)
        {
            if (!characterSheep[target].isUsed && !characterSheep[target].isSkilled)
            {
                character = target;
                MapRouteInit();
                HighLightDestroy();
                mapRoute[locationX, locationY].direction = Direction.stand;
                mapRoute[locationX, locationY].movePoint = characterSheep[character].move_range;
                CharacterX = locationX;
                CharacterY = locationY;
                CharacterMove(locationX, locationY, true, characterSheep[character].move_range);
                check = true;
            }
        }
        else if (HighLight[locationX, locationY] == 1)
        {
            step = 0;
            GamePlayer[CharacterX, CharacterY] = 0;
            GamePlayer[locationX, locationY] = 1;

            wayWalk(locationX, locationY, CharacterX, CharacterY);
            isWalk = true;
            walkType = true;
            HighLightDestroy();
        }
        else if (HighLight[locationX, locationY] == 2)
        {
            step = 0;
            wayCount = 0;
            trapCount = 999;
            MapRouteInit();
            mapRoute[CharacterX, CharacterY].direction = Direction.stand;
            mapRoute[CharacterX, CharacterY].movePoint = characterSheep[character].move_range;
            DangerousWayWalk(CharacterX, CharacterY, locationX, locationY, characterSheep[character].move_range, 0, true);

            GamePlayer[CharacterX, CharacterY] = 0;
            GamePlayer[locationX, locationY] = 1;

            step = 0;
            isWalk = true;
            walkType = false;
            HighLightDestroy();
        }
        else
        {
            HighLightDestroy();
            check = false;
        }
    }
    public void CharacterMove(int x, int y, bool sheep, int move)
    {
        SafeWaySearch(x, y, move, true, sheep);
        HighLightSearch(true);
        MapRouteInit();
        mapRoute[x, y].movePoint = move;
        SafeWaySearch(x, y, move, false, sheep);
        HighLightSearch(false);
        HighLightShow();

    }
    private void SafeWaySearch(int _x, int _y, int point, bool trapAfford, bool sheep)
    {
        int upPoint = 0;
        int downPoint = 0;
        int leftPoint = 0;
        int rightPoint = 0;
        if (_y + 1 <= 30)
        {
            upPoint = MapInfo[_x, _y + 1];
            if (trapAfford && upPoint == 500) upPoint = 1;
            if (sheep && upPoint == 900) upPoint = 1;
        }
        if (_y - 1 > 0)
        {
            downPoint = MapInfo[_x, _y - 1];
            if (trapAfford && downPoint == 500) downPoint = 1;
            if (sheep && downPoint == 900) downPoint = 1;
        }
        if (_x + 1 <= 30)
        {
            rightPoint = MapInfo[_x + 1, _y];
            if (trapAfford && rightPoint == 500) rightPoint = 1;
            if (sheep && rightPoint == 900) rightPoint = 1;
        }
        if (_x - 1 > 0)
        {
            leftPoint = MapInfo[_x - 1, _y];
            if (trapAfford && leftPoint == 500) leftPoint = 1;
            if (sheep && leftPoint == 900) leftPoint = 1;
        }
        if (point >= 0)
        {
            if (upPoint != 0)
            {
                if (point - upPoint > -100 && point - upPoint > mapRoute[_x, _y + 1].movePoint && GamePlayer[_x, _y + 1] == 0)
                {
                    mapRoute[_x, _y + 1].movePoint = point - upPoint;
                    mapRoute[_x, _y + 1].direction = Direction.up;
                    SafeWaySearch(_x, _y + 1, mapRoute[_x, _y + 1].movePoint, trapAfford, sheep);
                }
            }
            if (downPoint != 0)
            {
                if (point - downPoint > -100 && point - downPoint > mapRoute[_x, _y - 1].movePoint && GamePlayer[_x, _y - 1] == 0)
                {
                    mapRoute[_x, _y - 1].movePoint = point - downPoint;
                    mapRoute[_x, _y - 1].direction = Direction.down;
                    SafeWaySearch(_x, _y - 1, mapRoute[_x, _y - 1].movePoint, trapAfford, sheep);
                }
            }
            if (rightPoint != 0)
            {
                if (point - rightPoint > -100 && point - rightPoint > mapRoute[_x + 1, _y].movePoint && GamePlayer[_x + 1, _y] == 0)
                {
                    mapRoute[_x + 1, _y].movePoint = point - rightPoint;
                    mapRoute[_x + 1, _y].direction = Direction.right;
                    SafeWaySearch(_x + 1, _y, mapRoute[_x + 1, _y].movePoint, trapAfford, sheep);
                }
            }
            if (leftPoint != 0)
            {
                if (point - leftPoint > -100 && point - leftPoint > mapRoute[_x - 1, _y].movePoint && GamePlayer[_x - 1, _y] == 0)
                {
                    mapRoute[_x - 1, _y].movePoint = point - leftPoint;
                    mapRoute[_x - 1, _y].direction = Direction.left;
                    SafeWaySearch(_x - 1, _y, mapRoute[_x - 1, _y].movePoint, trapAfford, sheep);
                }
            }
        }
    }
    public void DangerousWayWalk(int _x, int _y, int targetX, int targetY, int point, int count, bool sheep)
    {

        int upPoint = 0;
        int downPoint = 0;
        int leftPoint = 0;
        int rightPoint = 0;
        int signUp = 0;
        int signDown = 0;
        int signRight = 0;
        int signLeft = 0;
        if (_y + 1 < 30)
        {
            upPoint = MapInfo[_x, _y + 1];
            if (sheep && upPoint == 900) upPoint = 1;
            if (upPoint == 500)
            {
                upPoint = 1;
                signUp = 1;
            }
        }
        if (_y - 1 >= 0)
        {
            downPoint = MapInfo[_x, _y - 1];
            if (sheep && downPoint == 900) downPoint = 1;
            if (downPoint == 500)
            {
                downPoint = 1;
                signDown = 1;
            }
        }
        if (_x + 1 < 30)
        {
            rightPoint = MapInfo[_x + 1, _y];
            if (sheep && rightPoint == 900) rightPoint = 1;
            if (rightPoint == 500)
            {
                rightPoint = 1;
                signRight = 1;
            }
        }
        if (_x - 1 >= 0)
        {
            leftPoint = MapInfo[_x - 1, _y];
            if (sheep && leftPoint == 900) leftPoint = 1;
            if (leftPoint == 500)
            {
                leftPoint = 1;
                signLeft = 1;
            }
        }

        if (_x == targetX && _y == targetY)
        {

            if (count < trapCount)
            {
                wayCount = step;

                for (int i = 0; i < wayCount; i++)
                {
                    way[i + 1] = DangerousTry[i + 1];
                }

                trapCount = count;

            }
            else if (step < wayCount && count == trapCount)
            {
                wayCount = step;

                for (int i = 0; i < wayCount; i++)
                {
                    way[i + 1] = DangerousTry[i + 1];
                }

                trapCount = count;
            }
        }
        else if (point > 0)
        {
            step++;

            if (upPoint != 0)
            {
                if (point - upPoint > -100 && mapRoute[_x, _y + 1].movePoint < 0 && GamePlayer[_x, _y + 1] == 0)
                {
                    mapRoute[_x, _y + 1].movePoint = point - upPoint;
                    DangerousTry[step] = Direction.up;
                    DangerousWayWalk(_x, _y + 1, targetX, targetY, mapRoute[_x, _y + 1].movePoint, count + signUp, sheep);
                    mapRoute[_x, _y + 1].movePoint = -999;
                }
            }
            if (downPoint != 0)
            {
                if (point - downPoint > -100 && mapRoute[_x, _y - 1].movePoint < 0 && GamePlayer[_x, _y - 1] == 0)
                {
                    mapRoute[_x, _y - 1].movePoint = point - downPoint;
                    DangerousTry[step] = Direction.down;
                    DangerousWayWalk(_x, _y - 1, targetX, targetY, mapRoute[_x, _y - 1].movePoint, count + signDown, sheep);
                    mapRoute[_x, _y - 1].movePoint = -999;
                }
            }
            if (rightPoint != 0)
            {
                if (point - rightPoint > -100 && mapRoute[_x + 1, _y].movePoint < 0 && GamePlayer[_x + 1, _y] == 0)
                {
                    mapRoute[_x + 1, _y].movePoint = point - rightPoint;
                    DangerousTry[step] = Direction.right;
                    DangerousWayWalk(_x + 1, _y, targetX, targetY, mapRoute[_x + 1, _y].movePoint, count + signRight, sheep);
                    mapRoute[_x + 1, _y].movePoint = -999;
                }
            }
            if (leftPoint != 0)
            {
                if (point - leftPoint > -100 && mapRoute[_x - 1, _y].movePoint < 0 && GamePlayer[_x - 1, _y] == 0)
                {
                    mapRoute[_x - 1, _y].movePoint = point - leftPoint;
                    DangerousTry[step] = Direction.left;
                    DangerousWayWalk(_x - 1, _y, targetX, targetY, mapRoute[_x - 1, _y].movePoint, count + signLeft, sheep);
                    mapRoute[_x - 1, _y].movePoint = -999;
                }
            }
            step--;
        }
    }
    public void HighLightSearch(bool trapAfford)
    {
        int t;
        if (trapAfford)
        {
            t = 2;
        }
        else
        {
            t = 1;
        }
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (mapRoute[i, j].movePoint >= 0)
                {
                    HighLight[i, j] = t;
                }
            }
        }
    }
    private void wayWalk(int targetX, int targetY, int CharacterX, int CharacterY)
    {
        if (targetX == CharacterX && targetY == CharacterY)
        {
            return;
        }
        else
        {
            step++;
            way[step] = mapRoute[targetX, targetY].direction;
            switch (mapRoute[targetX, targetY].direction)
            {
                case Direction.up:
                    wayWalk(targetX, targetY - 1, CharacterX, CharacterY);
                    break;
                case Direction.down:
                    wayWalk(targetX, targetY + 1, CharacterX, CharacterY);
                    break;
                case Direction.right:
                    wayWalk(targetX - 1, targetY, CharacterX, CharacterY);
                    break;
                case Direction.left:
                    wayWalk(targetX + 1, targetY, CharacterX, CharacterY);
                    break;
            }
        }

    }




    private void MapRouteInit()
    {
        for (int i = 0; i < 35; i++)
        {
            for (int j = 0; j < 35; j++)
            {
                mapRoute[i, j].movePoint = -999;
                mapRoute[i, j].direction = Direction.stand;
            }
        }
        for (int i = 0; i < 20; i++)
        {
            way[i] = Direction.stand;
        }
    }




    /// <summary>
    /// 敌人寻路模块
    /// </summary>    
    public void EnemySearch()
    {
        enemyPosList.Clear();
        foreach (int pos in enemyPos.Keys)
        {
            enemyPosList.Add(pos);
        }
        enemyCount = enemyPosList.Count;
        t = 0;
        enemyWalkAdimit = true;
    }
    public void EnemyMove()
    {
        if (t < enemyCount)
        {

            int enemyPosY = DicY(enemyPosList[t]);
            int enemyPosX = DicX(enemyPosList[t]);
            enemy = enemyPos[enemyPosList[t]];

            bool attack = EnemyAttack(enemyPosX, enemyPosY, characterMonster[enemy].attack_range, false);
            if (!attack)
            {

                EnemyAttackSearch(enemyPosX, enemyPosY, characterMonster[enemy].cordon);
                if (characterAttackX > -100)
                {

                    MapRouteInit();
                    step = 0;
                    wayCount = 0;
                    trapCount = 100;
                    mapRoute[enemyPosX, enemyPosY].movePoint = characterMonster[enemy].move_range;
                    int s = GamePlayer[characterAttackX, characterAttackY];
                    GamePlayer[characterAttackX, characterAttackY] = 0;
                    DangerousWayWalk(enemyPosX, enemyPosY, characterAttackX, characterAttackY, characterMonster[enemy].move_range + characterMonster[enemy].attack_range, 0, false);
                    GamePlayer[characterAttackX, characterAttackY] = s;
                    if (wayCount == 0)
                    {
                        Debug.Log(1);
                        MapRouteInit();
                        step = 0;
                        wayCount = 0;
                        trapCount = 100;
                        mapRoute[enemyPosX, enemyPosY].movePoint = characterMonster[enemy].move_range;
                        s = GamePlayer[characterAttackX, characterAttackY];
                        GamePlayer[characterAttackX, characterAttackY] = 0;
                        DangerousWayWalk(enemyPosX, enemyPosY, characterAttackX, characterAttackY, characterMonster[enemy].cordon, 0, false);
                        GamePlayer[characterAttackX, characterAttackY] = s;
                    }
                    step = wayCount;
                    EnemyWalkSetting();
                }
                else
                {
                    t++;
                }

            }
        }
        else
        {
            enemyWalkAdimit = false;
            camp = !camp;
            RoundStart();
            round++;
            Debug.Log("我方回合");
        }
    }
    private void EnemyAttackSearch(int _x, int _y, int point)
    {
        characterAttackX = -100;
        characterAttackY = -100;

        int healthC = 999;
        int healthM = 999;
        for (int i = -point; i <= point; i++)
        {
            int absI = point - Mathf.Abs(i);
            for (int j = -absI; j <= absI; j++)
            {

                if (_x + i >= 1 && _x + i <= 30 && _y + j >= 1 && _y + j <= 30 && GamePlayer[_x + i, _y + j] == 1)
                {

                    int pos = getDic(_x + i, _y + j);
                    GameObject g = characterPos[pos];
                    //bool t = false;
                    bool r = EnemyReach(_x, _y, _x + i, _y + j, characterMonster[enemy].move_range + characterMonster[enemy].attack_range, 0, false);
                    if (r)
                    {

                        if (characterSheep[g].hp <= healthM)
                        {
                            characterAttackX = i + _x;
                            characterAttackY = j + _y;
                            healthM = characterSheep[g].hp;

                        }
                    }
                    else
                    {

                        r = EnemyReach(_x, _y, _x + i, _y + j, characterMonster[enemy].cordon, 0, false);
                        if (characterSheep[g].hp <= healthC && r && healthM == 999)
                        {
                            characterAttackX = i + _x;
                            characterAttackY = j + _y;
                            healthC = characterSheep[g].hp;

                        }
                    }
                }
            }
        }
    }
    private bool EnemyReach(int _x, int _y, int targetX, int targetY, int point, int count, bool sheep)
    {
        MapRouteInit();
        step = 0;
        wayCount = 0;
        trapCount = 100;
        mapRoute[_x, _y].movePoint = point;
        int s = GamePlayer[targetX, targetY];
        GamePlayer[targetX, targetY] = 0;
        DangerousWayWalk(_x, _y, targetX, targetY, point, count, sheep);
        GamePlayer[targetX, targetY] = s;
        step = wayCount;

        if (step > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void EnemyWalkSetting()
    {
        step = 1;
        getPoint = false;
        _x = GetLocationX(enemy.transform.position.x);
        _y = GetLocationY(enemy.transform.position.y);
        //enemyPos.Remove(getDic(_x, _y));
        isEnemyWalk = true;
    }




    /// <summary>
    /// 高光显示和销毁模块
    /// </summary>
    public void HighLightShow()
    {
        int t = 0;
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (HighLight[i, j] == 1)
                {
                    float insX = GetPositionX(i);
                    float insY = GetPositionY(j);
                    GameObject g = Instantiate(highLight, new Vector3(insX, insY, -1), new Quaternion(0, 0, 0, 0));
                    g.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
                    g.layer = 9;
                    t++;
                    highLightObj.Add(t, g);
                    g.transform.SetParent(mapInfo.transform);
                }
                if (HighLight[i, j] == 2)
                {
                    float insX = GetPositionX(i);
                    float insY = GetPositionY(j);
                    GameObject g = Instantiate(highLightD, new Vector3(insX, insY, -1), new Quaternion(0, 0, 0, 0));
                    g.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
                    g.layer = 9;
                    t++;
                    highLightObj.Add(t, g);
                    g.transform.SetParent(mapInfo.transform);
                }
                if (HighLight[i, j] == 3)
                {
                    float insX = GetPositionX(i);
                    float insY = GetPositionY(j);
                    GameObject g = Instantiate(highLightS, new Vector3(insX, insY, -1), new Quaternion(0, 0, 0, 0));
                    g.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
                    g.layer = 9;
                    t++;
                    highLightObj.Add(t, g);
                    g.transform.SetParent(mapInfo.transform);
                }
            }
        }
    }
    public void HighLightDestroy()
    {
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                HighLight[i, j] = 0;
            }
        }
        if (highLightObj.Count > 0)
        {

            foreach (GameObject g in highLightObj.Values)
            {
                DestroyImmediate(g);

            }
            highLightObj.Clear();
        }
    }



    /// <summary>
    /// Update与走路显示模块
    /// </summary>
    void Update()
    {
        if (MapMgr.Instance.Playing == true)
        {
            if (!isWalk && !isEnemyWalk)
            {
                if (!camp && t == -1)
                {
                    EnemySearch();
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    camp = !camp;
                    t = -1;
                    Debug.Log("敌人回合");
                    HighLightDestroy();
                }
            }

            if (isWalk)
            {
                Walk(walkType);
            }
            if (enemyWalkAdimit)
            {
                if (isEnemyWalk)
                {
                    EnemyWalk();
                }
                else
                {
                    EnemyMove();
                }
            }

            WinCheck();
            LoseCheck();
        }
    }



    /// <summary>
    /// Update方法
    /// </summary>
    private void WinCheck()
    {
        bool win = true;
        foreach (GameObject g in characterMonster.Keys)
        {
            if (g != null)
            {
                if (g.tag == "enemy") win = false;
            }
        }
        if (win)
        {
            foreach (int i in enemyPos.Keys)
            {
                if (enemyPos[i] != null)
                {
                    Destroy(enemyPos[i]);
                }
            }
            WindowMgr.Instance.active_window("Result");
            User.Instance.level_up();
            MapMgr.Instance.Playing = false;
            MapMgr.Instance.isWin = true;
        }
    }
    private void LoseCheck()
    {
        bool lose = true;
        foreach (GameObject g in characterSheep.Keys)
        {
            if (g != null)
            {
                if (g.tag == "character") lose = false;
            }
        }
        if (lose || exit)
        {
            foreach (int i in characterPos.Keys)
            {
                Destroy(characterPos[i]);
            }
            WindowMgr.Instance.active_window("Result");
            User.Instance.level_up();
            MapMgr.Instance.Playing = false;
            MapMgr.Instance.isWin = false;
        }
    }
    private void RoundStart()
    {
        foreach (GameObject g in characterSheep.Keys)
        {
            characterSheep[g].isUsed = false;
            characterSheep[g].isSkilled = false;
        }
    }
    private void Walk(bool safe)
    {
        ScreenLock = true;
        if (safe)
        {
            if (step > 0)
            {
                if (sleep >= (int)(0.05f / Time.deltaTime))
                {
                    characterPos.Remove(getDic(GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y)));
                    switch (way[step])
                    {
                        case Direction.up:
                            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + 40, character.transform.position.z);
                            break;
                        case Direction.down:
                            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y - 40, character.transform.position.z);
                            break;
                        case Direction.right:
                            character.transform.position = new Vector3(character.transform.position.x + 40, character.transform.position.y, character.transform.position.z);
                            break;
                        case Direction.left:
                            character.transform.position = new Vector3(character.transform.position.x - 40, character.transform.position.y, character.transform.position.z);
                            break;
                    }
                    characterPos.Add(getDic(GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y)), character);
                    if (MapInfo[GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y)] == 500)
                    {
                        SheepDamage(GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y), trapDamage);
                    }
                    step--;
                    sleep = 0;
                }
                else
                {
                    sleep++;
                }
            }
            else
            {
                isWalk = false;
                characterSheep[character].isUsed = true;
                sleep = 0;
            }
        }
        else
        {
            if (step < wayCount)
            {
                if (sleep >= (int)(0.05f / Time.deltaTime))
                {
                    step++;
                    characterPos.Remove(getDic(GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y)));
                    switch (way[step])
                    {
                        case Direction.up:
                            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + 40, character.transform.position.z);

                            break;
                        case Direction.down:
                            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y - 40, character.transform.position.z);

                            break;
                        case Direction.right:
                            character.transform.position = new Vector3(character.transform.position.x + 40, character.transform.position.y, character.transform.position.z);

                            break;
                        case Direction.left:
                            character.transform.position = new Vector3(character.transform.position.x - 40, character.transform.position.y, character.transform.position.z);
                            break;
                    }
                    characterPos.Add(getDic(GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y)), character);
                    if (MapInfo[GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y)] == 500)
                    {
                        SheepDamage(GetLocationX(character.transform.position.x), GetLocationY(character.transform.position.y), trapDamage);
                    }
                    sleep = 0;
                }
                else
                {
                    sleep++;
                }
            }
            else
            {
                isWalk = false;
                characterSheep[character].isUsed = true;
                sleep = 0;
            }
        }
        ScreenLock = false;
    }
    private void EnemyWalk()
    {
        ScreenLock = true;
        if (step <= wayCount && step <= characterMonster[enemy].move_range && !getPoint)
        {


            if (sleep >= (int)(0.05f / Time.deltaTime))
            {
                enemyPos.Remove(getDic(GetLocationX(enemy.transform.position.x), GetLocationY(enemy.transform.position.y)));

                switch (way[step])
                {
                    case Direction.up:
                        GamePlayer[_x, _y] = 0;
                        _y++;
                        if (GamePlayer[_x, _y] == 1)
                        {
                            GamePlayer[_x, _y - 1] = -1;
                            getPoint = true;
                        }
                        else
                        {
                            GamePlayer[_x, _y] = -1;
                            enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 40, enemy.transform.position.z);
                        }
                        break;
                    case Direction.down:
                        GamePlayer[_x, _y] = 0;
                        _y--;
                        if (GamePlayer[_x, _y] == 1)
                        {
                            GamePlayer[_x, _y + 1] = -1;
                            getPoint = true;
                        }
                        else
                        {
                            GamePlayer[_x, _y] = -1;
                            enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y - 40, enemy.transform.position.z);
                        }
                        break;
                    case Direction.right:
                        GamePlayer[_x, _y] = 0;
                        _x++;
                        if (GamePlayer[_x, _y] == 1)
                        {
                            GamePlayer[_x - 1, _y] = -1;
                            getPoint = true;
                        }
                        else
                        {
                            GamePlayer[_x, _y] = -1;
                            enemy.transform.position = new Vector3(enemy.transform.position.x + 40, enemy.transform.position.y, enemy.transform.position.z);
                        }
                        break;
                    case Direction.left:
                        GamePlayer[_x, _y] = 0;
                        _x--;
                        if (GamePlayer[_x, _y] == 1)
                        {
                            GamePlayer[_x + 1, _y] = -1;
                            getPoint = true;
                        }
                        else
                        {
                            GamePlayer[_x, _y] = -1;
                            enemy.transform.position = new Vector3(enemy.transform.position.x - 40, enemy.transform.position.y, enemy.transform.position.z);
                        }
                        break;
                }
                step++;
                enemyPos.Add(getDic(GetLocationX(enemy.transform.position.x), GetLocationY(enemy.transform.position.y)), enemy);
                if (MapInfo[GetLocationX(enemy.transform.position.x), GetLocationY(enemy.transform.position.y)] == 500)
                {
                    EnemyDamage(GetLocationX(enemy.transform.position.x), GetLocationY(enemy.transform.position.y), trapDamage);
                }
                sleep = 0;
            }
            else
            {
                sleep++;
            }
        }
        else
        {
            isEnemyWalk = false;
            EnemyAttack(GetLocationX(enemy.transform.position.x), GetLocationY(enemy.transform.position.y), characterMonster[enemy].attack_range, true);
            sleep = 0;
            t++;
        }
        ScreenLock = false;
    }
    public void BattleExit()
    {
        exit = true;
    }
    public int GetRound()
    {
        return round;
    }



    public void CenterManager(int locationX, int locationY, ref GameObject gameObject, int mouse)
    {
        if (!isWalk && !isEnemyWalk)
        {
            if (camp)
            {
                if (mouse == 0)
                {
                    CharacterCheck(locationX, locationY, ref gameObject);
                }
                else if (mouse == 1)
                {
                    Skill(locationX, locationY, gameObject);
                }

            }
        }
    }






    /// <summary>
    /// 技能模块
    /// </summary>
    public void Skill(int locationX, int locationY, GameObject gameObject)
    {
        if (HighLight[locationX, locationY] == 3)
        {
            SkillUse(CharacterX, CharacterY, locationX, locationY, character, true);

        }
        else if (GamePlayer[locationX, locationY] == 1)
        {
            HighLightDestroy();
            CharacterX = locationX;
            CharacterY = locationY;
            character = gameObject;
            SkillUse(CharacterX, CharacterY, locationX, locationY, character, false);
        }
        else
        {
            HighLightDestroy();
        }
    }
    public void SkillUse(int CharacterX, int CharacterY, int locationX, int locationY, GameObject gameObject, bool sure)
    {

        if (!characterSheep[gameObject].isSkilled)
        {
            switch (characterSheep[gameObject].skill)
            {
                case "attract":
                    Attract(CharacterX, CharacterY, sure);
                    break;
                case "rotate":
                    Rotate(CharacterX, CharacterY, sure);
                    break;
                case "push":
                    Push(CharacterX, CharacterY, locationX, locationY, sure);
                    break;
                case "attack":
                    GameObject g = characterPos[getDic(CharacterX, CharacterY)];
                    int point = characterSheep[g].attack_range;
                    Attack(CharacterX, CharacterY, locationX, locationY, point, sure);
                    break;
                case "cannon":
                    Cannon(CharacterX, CharacterY, locationX, locationY, sure);
                    break;
                case "stay":
                    break;
            }
        }
        try
        {
            //if (sure) characterSheep[gameObject].isSkilled = true;
        }
        catch { }
    }
    public void Attract(int x, int y, bool sure)
    {
        if (x > 1 && x < maxX && y > 1 && y < maxY)
        {
            if (!sure)
            {
                if (x + 2 <= maxX) HighLight[x + 2, y] = 3;
                if (x - 2 > 0) HighLight[x - 2, y] = 3;
                if (y + 2 <= maxY) HighLight[x, y + 2] = 3;
                if (y - 2 > 0) HighLight[x, y - 2] = 3;
                HighLightShow();
            }
            else
            {

                GameObject par1 = Instantiate(testPartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par1.transform.position = new Vector3(GetPositionX(x - 1), GetPositionY(y), -1);
                par1.layer = 9;
                par1.transform.eulerAngles = new Vector3(0, 90, -90);
                Destroy(par1, 0.4f);
                GameObject par2 = Instantiate(testPartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par2.transform.position = new Vector3(GetPositionX(x + 1), GetPositionY(y), -1);
                par2.layer = 9;
                par2.transform.eulerAngles = new Vector3(180, 90, -90);
                Destroy(par2, 0.4f);
                GameObject par3 = Instantiate(testPartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par3.transform.position = new Vector3(GetPositionX(x), GetPositionY(y - 1), -1);
                par3.layer = 9;
                par3.transform.eulerAngles = new Vector3(-90, 90, -90);
                Destroy(par3, 0.4f);
                GameObject par4 = Instantiate(testPartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par4.transform.position = new Vector3(GetPositionX(x), GetPositionY(y + 1), -1);
                par4.layer = 9;
                par4.transform.eulerAngles = new Vector3(90, 90, -90);
                Destroy(par4, 0.4f);

                if (MapInfo[x + 1, y] == 1 && MapInfo[x + 2, y] > 1)
                {
                    int gEx = getDic(x + 2, y);
                    int g = getDic(x + 1, y);
                    GameObject go = landformPos[gEx];
                    landformPos.Remove(gEx);
                    go.transform.position = go.transform.position + new Vector3(-40, 0, 0);
                    landformPos.Add(g, go);
                    MapInfo[x + 1, y] = MapInfo[x + 2, y];
                    MapInfo[x + 2, y] = 1;
                    SkillEffectCheck(x + 1, y);
                }
                if (MapInfo[x - 1, y] == 1 && MapInfo[x - 2, y] > 1)
                {
                    int gEx = getDic(x - 2, y);
                    int g = getDic(x - 1, y);
                    GameObject go = landformPos[gEx];
                    landformPos.Remove(gEx);
                    go.transform.position = go.transform.position + new Vector3(40, 0, 0);
                    landformPos.Add(g, go);
                    MapInfo[x - 1, y] = MapInfo[x - 2, y];
                    MapInfo[x - 2, y] = 1;
                    SkillEffectCheck(x - 1, y);
                }
                if (MapInfo[x, y + 1] == 1 && MapInfo[x, y + 2] > 1)
                {
                    int gEx = getDic(x, y + 2);
                    int g = getDic(x, y + 1);
                    GameObject go = landformPos[gEx];
                    landformPos.Remove(gEx);
                    go.transform.position = go.transform.position + new Vector3(0, -40, 0);
                    landformPos.Add(g, go);
                    MapInfo[x, y + 1] = MapInfo[x, y + 2];
                    MapInfo[x, y + 2] = 1;
                    SkillEffectCheck(x, y + 1);
                }
                if (MapInfo[x, y - 1] == 1 && MapInfo[x, y - 2] > 1)
                {
                    int gEx = getDic(x, y - 2);
                    int g = getDic(x, y - 1);
                    GameObject go = landformPos[gEx];
                    landformPos.Remove(gEx);
                    go.transform.position = go.transform.position + new Vector3(0, 40, 0);
                    landformPos.Add(g, go);
                    MapInfo[x, y - 1] = MapInfo[x, y - 2];
                    MapInfo[x, y - 2] = 1;
                    SkillEffectCheck(x, y - 1);
                }
                HighLightDestroy();
            }
        }
    }
    public void Rotate(int x, int y, bool sure)
    {
        if (x > 0 && x < maxX && y > 0 && y < maxY)
        {
            if (!sure)
            {
                HighLight[x + 1, y + 1] = 3;
                HighLight[x - 1, y + 1] = 3;
                HighLight[x + 1, y - 1] = 3;
                HighLight[x - 1, y - 1] = 3;
                HighLightShow();
            }
            else
            {
                GameObject par1 = Instantiate(rotatePartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par1.transform.position = new Vector3(GetPositionX(x + 1), GetPositionY(y + 1), -1);
                par1.layer = 9;
                Destroy(par1, 1);
                GameObject par2 = Instantiate(rotatePartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par2.transform.position = new Vector3(GetPositionX(x + 1), GetPositionY(y - 1), -1);
                par2.layer = 9;
                Destroy(par2, 1);
                GameObject par3 = Instantiate(rotatePartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par3.transform.position = new Vector3(GetPositionX(x - 1), GetPositionY(y + 1), -1);
                par3.layer = 9;
                Destroy(par3, 1);
                GameObject par4 = Instantiate(rotatePartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par4.transform.position = new Vector3(GetPositionX(x - 1), GetPositionY(y - 1), -1);
                par4.layer = 9;
                Destroy(par4, 1);

                int t;
                t = MapInfo[x + 1, y + 1];
                MapInfo[x + 1, y + 1] = MapInfo[x - 1, y + 1];
                MapInfo[x - 1, y + 1] = MapInfo[x - 1, y - 1];
                MapInfo[x - 1, y - 1] = MapInfo[x + 1, y - 1];
                MapInfo[x + 1, y - 1] = t;

                int g1P = getDic(x + 1, y + 1);
                int g2P = getDic(x - 1, y + 1);
                int g3P = getDic(x - 1, y - 1);
                int g4P = getDic(x + 1, y - 1);

                GameObject g1 = null;
                GameObject g2 = null;
                GameObject g3 = null;
                GameObject g4 = null;
                try
                {
                    g1 = landformPos[g1P];
                    landformPos.Remove(g1P);
                }
                catch { }
                try
                {
                    g2 = landformPos[g2P];
                    landformPos.Remove(g2P);
                }
                catch { }
                try
                {
                    g3 = landformPos[g3P];
                    landformPos.Remove(g3P);
                }
                catch { }
                try
                {
                    g4 = landformPos[g4P];
                    landformPos.Remove(g4P);
                }
                catch { }

                if (g1 != null)
                {
                    g1.transform.position = g1.transform.position + new Vector3(0, -80, 0);
                    g1P = getDic(x + 1, y - 1);
                    landformPos.Add(g1P, g1);
                }

                if (g2 != null)
                {
                    g2.transform.position = g2.transform.position + new Vector3(80, 0, 0);
                    g2P = getDic(x + 1, y + 1);
                    landformPos.Add(g2P, g2);
                }
                if (g3 != null)
                {
                    g3.transform.position = g3.transform.position + new Vector3(0, 80, 0);
                    g3P = getDic(x - 1, y + 1);
                    landformPos.Add(g3P, g3);
                }
                if (g4 != null)
                {
                    g4.transform.position = g4.transform.position + new Vector3(-80, 0, 0);
                    g4P = getDic(x - 1, y - 1);
                    landformPos.Add(g4P, g4);
                }
                SkillEffectCheck(x + 1, y + 1);
                SkillEffectCheck(x + 1, y - 1);
                SkillEffectCheck(x - 1, y + 1);
                SkillEffectCheck(x - 1, y - 1);
                HighLightDestroy();
            }
        }
    }
    public void Push(int x, int y, int PushX, int PushY, bool sure)
    {
        if (!sure)
        {
            if (x + 1 <= maxX) HighLight[x + 1, y] = 3;
            if (x - 1 > 0) HighLight[x - 1, y] = 3;
            if (y + 1 <= maxY) HighLight[x, y + 1] = 3;
            if (y - 1 > 0) HighLight[x, y - 1] = 3;
            HighLightShow();
        }
        else
        {

            GameObject g = Instantiate(testPartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            g.transform.position = new Vector3(GetPositionX(x), GetPositionY(y), -1);
            g.layer = 9;
            if (PushX - x == 1) g.transform.eulerAngles = new Vector3(0, 90, -90);
            if (PushX - x == -1) g.transform.eulerAngles = new Vector3(180, 90, -90);
            if (PushY - y == 1) g.transform.eulerAngles = new Vector3(-90, 90, -90);
            if (PushY - y == -1) g.transform.eulerAngles = new Vector3(90, 90, -90);
            Destroy(g, 0.4f);


            if (GamePlayer[PushX, PushY] == -1)
            {
                int PushDisX = PushX - x;
                int PushDisY = PushY - y;
                if (PushDisX != 0)
                {
                    PushCheck(x + PushDisX, y, x + 2 * PushDisX, y);
                    if (GamePlayer[x + PushDisX, y + 1] == -1)
                    {
                        PushCheck(x + PushDisX, y + 1, x + 2 * PushDisX, y + 1);
                    }
                    if (GamePlayer[x + PushDisX, y - 1] == -1)
                    {
                        PushCheck(x + PushDisX, y - 1, x + 2 * PushDisX, y - 1);
                    }
                }
                if (PushDisY != 0)
                {
                    PushCheck(x, y + PushDisY, x, y + 2 * PushDisY);
                    if (GamePlayer[x + 1, y + PushDisY] == -1)
                    {
                        PushCheck(x + 1, y + PushDisY, x + 1, y + 2 * PushDisY);
                    }
                    if (GamePlayer[x - 1, y + PushDisY] == -1)
                    {
                        PushCheck(x - 1, y + PushDisY, x - 1, y + 2 * PushDisY);
                    }
                }
            }
            HighLightDestroy();
        }

    }
    public void Cannon(int x, int y, int targetX, int targetY, bool sure)
    {
        if (!sure)
        {

            if (x + 1 <= maxX && MapInfo[x + 1, y] > 1)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (x + 1 + i <= maxX) HighLight[x + 1 + i, y] = 3;
                }
            }
            if (x - 1 > 0 && MapInfo[x - 1, y] > 1)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (x - 1 - i > 0) HighLight[x - 1 - i, y] = 3;
                }
            }
            if (y + 1 <= maxY && MapInfo[x, y + 1] > 1)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (y + 1 + i <= maxY) HighLight[x, y + 1 + i] = 3;
                }
            }
            if (y - 1 > 0 && MapInfo[x, y - 1] > 1)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (y - 1 - i > 0) HighLight[x, y - 1 - i] = 3;
                }
            }
            HighLightShow();
        }
        else
        {
            int _x = targetX - x;
            int _y = targetY - y;
            if (_x != 0) _x = _x / Mathf.Abs(_x);
            if (_y != 0) _y = _y / Mathf.Abs(_y);
            Debug.Log(_x + "," + _y);
            int PushX = targetX - _x;
            int PushY = targetY - _y;
            Push(PushX, PushY, targetX, targetY, sure);
        }
    }
    private void PushCheck(int x, int y, int PushX, int PushY)
    {
        if (MapInfo[PushX, PushY] == 0)
        {
            EnemyDie(x, y);
        }
        else if (GamePlayer[PushX, PushY] == 1)
        {
            Debug.Log("撞到人伤害");
            EnemyDamage(x, y, characterDamge);
            SheepDamage(PushX, PushY, characterDamge);
        }
        else if (GamePlayer[PushX, PushY] == -1)
        {
            Debug.Log("撞到敌人伤害");
            EnemyDamage(x, y, characterDamge);
            EnemyDamage(PushX, PushY, characterDamge);
        }
        else if (MapInfo[PushX, PushY] == 900)
        {
            Debug.Log("撞到栅栏伤害");
            EnemyDamage(x, y, barrierDamage);
        }
        else
        {
            int enemyG = getDic(x, y);
            GameObject g = enemyPos[enemyG];
            enemyPos.Remove(enemyG);
            g.transform.position = g.transform.position + new Vector3((PushX - x) * 40, (PushY - y) * 40, 0);
            enemyG = getDic(PushX, PushY);
            enemyPos.Add(enemyG, g);
            GamePlayer[x, y] = 0;
            GamePlayer[PushX, PushY] = -1;
            SkillEffectCheck(PushX, PushY);
        }
    }
    private void Attack(int x, int y, int AttackX, int AttackY, int Attack_Range, bool sure)
    {
        if (!sure)
        {
            for (int i = -Attack_Range; i <= Attack_Range; i++)
            {
                int absI = Attack_Range - Mathf.Abs(i);
                for (int j = -absI; j <= absI; j++)
                {
                    if (x + i >= 1 && x + i <= 30 && y + j >= 1 && y + j <= 30)
                    {
                        HighLight[x + i, y + j] = 3;
                    }
                }
            }
            HighLightShow();
        }
        else
        {
            if (GamePlayer[AttackX, AttackY] == -1)
            {
                GameObject par1 = Instantiate(rotatePartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                par1.transform.position = new Vector3(GetPositionX(AttackX), GetPositionY(AttackY), -1);
                par1.layer = 9;
                Destroy(par1, 1);
                GameObject g = characterPos[getDic(x, y)];
                EnemyDamage(AttackX, AttackY, characterSheep[g].attack);
                try
                {
                    GameObject _g = enemyPos[getDic(AttackX, AttackY)];
                    if (Mathf.Abs(x - AttackX) + Mathf.Abs(y - AttackY) <= characterMonster[_g].attack_range)
                    {
                        SheepDamage(x, y, characterMonster[_g].attack);
                    }
                }
                catch { }
            }
            HighLightDestroy();
        }
    }
    private bool EnemyAttack(int x, int y, int point, bool walking)
    {
        EnemyAttackSearch(x, y, point);
        if (characterAttackX > -100)
        {
            GameObject par1 = Instantiate(rotatePartical, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            par1.transform.position = new Vector3(GetPositionX(characterAttackX), GetPositionY(characterAttackY), -1);
            par1.layer = 9;
            Destroy(par1, 1);
            SheepDamage(characterAttackX, characterAttackY, characterMonster[enemy].attack);
            try
            {
                GameObject _g = characterPos[getDic(characterAttackX, characterAttackY)];
                if (Mathf.Abs(x - characterAttackX) + Mathf.Abs(y - characterAttackY) <= characterSheep[_g].attack_range)
                {
                    EnemyDamage(x, y, characterSheep[_g].attack);
                }
            }
            catch { }
            if (!walking) t++;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SkillEffectCheck(int x, int y)
    {
        GameObject g;
        int pos = getDic(x, y);
        try
        {
            g = enemyPos[pos];
            int lanformInfo = MapInfo[x, y];
            switch (lanformInfo)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 900:
                    EnemyDie(x, y);
                    break;
                case 500:
                    Debug.Log("怪物受伤");
                    EnemyDamage(x, y, trapDamage);
                    break;
                case 9999:
                    EnemyDie(x, y);
                    break;
            }
        }
        catch
        {
            g = null;
        }
        try
        {
            g = characterPos[pos];
            int lanformInfo = MapInfo[x, y];
            switch (lanformInfo)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 900:
                    break;
                case 500:
                    Debug.Log("我方受伤");
                    SheepDamage(x, y, trapDamage);
                    break;
                case 9999:
                    SheepDie(x, y);
                    break;
            }
        }
        catch
        {
            g = null;
        }
    }






    /// <summary>
    /// 伤害模块
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="damage"></param>
    public void EnemyDamage(int x, int y, int damage)
    {
        int pos = getDic(x, y);
        GameObject g = enemyPos[pos];
        characterMonster[g].hp = characterMonster[g].hp - damage;
        Debug.Log("怪物受伤:剩余血量为" + characterMonster[g].hp);
        if (characterMonster[g].hp <= 0) EnemyDie(x, y);
    }
    public void EnemyDie(int x, int y)
    {
        int pos = getDic(x, y);
        GamePlayer[x, y] = 0;
        GameObject g = enemyPos[pos];
        characterMonster.Remove(g);
        enemyPos.Remove(pos);
        DestroyImmediate(g);
        Debug.Log("怪物死亡");
        isEnemyWalk = false;
        t++;
    }
    public void SheepDamage(int x, int y, int damage)
    {
        int pos = getDic(x, y);
        GameObject g = characterPos[pos];
        characterSheep[g].hp = characterSheep[g].hp - damage;
        Debug.Log("我方角色受伤：剩余血量为" + characterSheep[g].hp);
        if (characterSheep[g].hp <= 0) SheepDie(x, y);
    }
    public void SheepDie(int x, int y)
    {
        int pos = getDic(x, y);
        GamePlayer[x, y] = 0;
        GameObject g = characterPos[pos];
        characterSheep.Remove(g);
        characterPos.Remove(pos);
        DestroyImmediate(g);
        Debug.Log("我方角色死亡");
        isWalk = false;
    }





    /// <summary>
    /// 位置工具
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int GetLocationX(float pos) //得到坐标信息
    {
        return ((int)(pos + InitX - mapInfo.transform.position.x) - 20) / 40 + 1;
    }
    public int GetLocationY(float pos) //得到坐标信息
    {
        return ((int)(pos + InitY - mapInfo.transform.position.y) - 20) / 40 + 1;
    }
    public float GetPositionX(int loc) //得到位置信息
    {
        return (loc - 1) * 40 + 20 - InitX + mapInfo.transform.position.x;
    }
    public float GetPositionY(int loc) //得到位置信息
    {
        return (loc - 1) * 40 + 20 - InitY + mapInfo.transform.position.y;
    }
    private int getDic(int x, int y)//得到索引数字
    {
        return y * MAX_NUMBER + x;
    }
    private int DicY(int dicNumber)//从索引数字得到坐标信息Y
    {
        return (dicNumber - 1) / MAX_NUMBER;
    }
    private int DicX(int dicNumber)//从索引数字得到坐标信息X
    {
        return dicNumber - DicY(dicNumber) * MAX_NUMBER;
    }
}