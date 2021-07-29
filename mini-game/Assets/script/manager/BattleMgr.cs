using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BattleMgr : MonoBehaviour
{
    public const int MAX_NUMBER = 30;
    public GameObject highLight;
    public GameObject highLightD;
    public GameObject highLightS;
    public GameObject character;
    public GameObject ex;
    public Dictionary<int, GameObject> highLightObj = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> landformPos = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> enemyPos = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> characterPos = new Dictionary<int, GameObject>();
    public int characterAttackX = 0;
    public int characterAttackY = 0;
    public int health = 10;
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
    private bool isWalk;
    private bool enemyWalk;
    private bool walkType;

    //private int locationX;
    //private int locationY;
    private int CharacterX;
    private int CharacterY;
    private float rotX;
    private float rotY;
    private float rotZ;

    private RouteObject[,] mapRoute = new RouteObject[35, 35];
    private bool check = true;
    int enemyRange = 4;
    int enemyAttackRange = 1;
    int enemyMaxMove = 3;
    int maxMove = 3;

    private int locationX;
    private int locationY;

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
        rotX = ex.transform.eulerAngles.x;
        rotY = ex.transform.eulerAngles.y;
        rotZ = ex.transform.eulerAngles.z;
        highLight = (GameObject)Resources.Load("Prefab/HighLight");
        highLightD = (GameObject)Resources.Load("Prefab/HighLightD");
        highLightS = (GameObject)Resources.Load("Prefab/HighLightS");
        MapInfo = GetComponent<MapMgr>().MapInfo;
        GamePlayer = GetComponent<MapMgr>().GamePlayer;
        landformPos = GetComponent<MapMgr>().landformPos;
        enemyPos = GetComponent<MapMgr>().enemyPos;
        characterPos = GetComponent<MapMgr>().characterPos;
        maxX = GetComponent<MapMgr>().maxX;
        maxY = GetComponent<MapMgr>().maxY;

    }


    /// <summary>
    /// 角色寻路
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CharacterCheck(int locationX, int locationY, ref GameObject target)
    {

        if (GamePlayer[locationX, locationY] == 1)
        {
            character = target;
            MapRouteInit();
            HighLightDestroy();
            mapRoute[locationX, locationY].direction = Direction.stand;
            mapRoute[locationX, locationY].movePoint = maxMove;
            CharacterX = locationX;
            CharacterY = locationY;
            CharacterMove(locationX, locationY, true);
            check = true;
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
            trapCount = 999;
            MapRouteInit();
            mapRoute[CharacterX, CharacterY].direction = Direction.stand;
            mapRoute[CharacterX, CharacterY].movePoint = maxMove;
            DangerousWayWalk(CharacterX, CharacterY, locationX, locationY, maxMove, 0, true);

            GamePlayer[CharacterX, CharacterY] = 0;
            GamePlayer[locationX, locationY] = 1;
            //step = wayCount;
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
    public void CharacterMove(int x, int y, bool sheep)
    {
        SafeWaySearch(x, y, maxMove, true, sheep);
        HighLightSearch(true);
        MapRouteInit();
        mapRoute[x, y].movePoint = maxMove;
        SafeWaySearch(x, y, maxMove, false, sheep);
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
    /// <summary>
    /// 敌人寻路
    /// </summary>
    public void EnemyMove()
    {
        MapRouteInit();
        enemyAttackSearch(13, 2, enemyRange);
        if (characterAttackX > -100)
        {
            step = 0;
            trapCount = 100;
            DangerousWayWalk(13, 2, characterAttackX, characterAttackY, 19, 0, false);
            moveEnemy = enemyPos[getDic(13, 2)];
            step = wayCount;
            enemyWalk = true;

            GamePlayer[13, 2] = 0;
            //GamePlayer[characterAttackX, characterAttackY] = -1;
        }
        else
        {
            Debug.Log("找不到攻击角色");
        }
    }
    private void enemyAttackSearch(int _x, int _y, int point)
    {
        characterAttackX = -100;
        characterAttackY = -100;
        for (int i = -point; i <= point; i++)
        {
            int absI = point - Mathf.Abs(i);
            for (int j = -absI; j <= absI; j++)
            {
                if (_x + i >= 1 && _x + i <= 30 && _y + j >= 1 && _y + j <= 30 && GamePlayer[_x + i, _y + j] == 1)
                {
                    //if (health <= 10)
                    //{
                    characterAttackX = i + _x;
                    characterAttackY = j + _y;
                    // }
                }
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
    private void EnemyWalk(GameObject g, int maxMove)
    {
        step = 1;
        bool getPoint = false;
        int _x = GetLocation(g.transform.position.x);
        int _y = GetLocation(g.transform.position.y);
        while (step <= wayCount && step <= maxMove && !getPoint)
        {
            switch (way[step])
            {
                case Direction.up:
                    _x++;
                    if (GamePlayer[_x, _y] == 1)
                    {
                        getPoint = true;
                    }
                    else
                    {
                        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y + 40, g.transform.position.z);
                    }
                    break;
                case Direction.down:
                    g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y - 40, g.transform.position.z);

                    break;
                case Direction.right:
                    g.transform.position = new Vector3(g.transform.position.x + 40, g.transform.position.y, g.transform.position.z);

                    break;
                case Direction.left:
                    g.transform.position = new Vector3(g.transform.position.x - 40, g.transform.position.y, g.transform.position.z);

                    break;
            }
            step++;
        }

    }
    private void Walk(bool safe)
    {

        if (safe)
        {
            while (step > 0)
            {
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
                step--;
            }
        }
        else
        {
            step = 0;
            while (step < wayCount)
            {
                step++;
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

            }
        }

    }

    /// <summary>
    /// 高光显示和销毁
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
                    int insX = GetPosition(i);
                    int insY = GetPosition(j);
                    GameObject g = Instantiate(highLight, new Vector3(insX, insY, -1), new Quaternion(0, 0, 0, 0));
                    g.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
                    t++;
                    highLightObj.Add(t, g);
                }
                if (HighLight[i, j] == 2)
                {
                    int insX = GetPosition(i);
                    int insY = GetPosition(j);
                    GameObject g = Instantiate(highLightD, new Vector3(insX, insY, -1), new Quaternion(0, 0, 0, 0));
                    g.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
                    t++;
                    highLightObj.Add(t, g);
                }
                if (HighLight[i, j] == 3)
                {
                    int insX = GetPosition(i);
                    int insY = GetPosition(j);
                    GameObject g = Instantiate(highLightS, new Vector3(insX, insY, -1), new Quaternion(0, 0, 0, 0));
                    g.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
                    t++;
                    highLightObj.Add(t, g);
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


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            EnemyMove();
        }

        OnMouseMove();

        if (isWalk)
        {
            Walk(walkType);
            isWalk = false;
        }

        if (enemyWalk)
        {
            Debug.Log("get");
            EnemyWalk(moveEnemy, maxMove);
            enemyWalk = false;
        }
    }

    private void OnMouseMove()
    {

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rh;
            bool hit = Physics.Raycast(ray, out rh);
            if (hit)
            {
                GameObject target = rh.collider.gameObject;
                locationX = GetLocation(target.transform.position.x);
                locationY = GetLocation(target.transform.position.y);

                Debug.Log(target.name);

                if (HighLight[locationX, locationY] == 3)
                {
                    SkillChoose(CharacterX, CharacterY, locationX, locationY, true);
                }
                else if (GamePlayer[locationX, locationY] == 1)
                {
                    HighLightDestroy();
                    CharacterX = locationX;
                    CharacterY = locationY;
                    SkillChoose(CharacterX, CharacterY, locationX, locationY, false);

                }
                else
                {
                    HighLightDestroy();
                }
            }

        }
    }

    /// <summary>
    /// 技能
    /// </summary>
    /// 
    private void SkillChoose(int CharacterX, int CharacterY, int locationX, int locationY, bool sure)
    {
        Push(CharacterX, CharacterY, locationX, locationY, sure);
        //Attract(CharacterX, CharacterY, sure);
        //Rotate(CharacterX, CharacterY, sure);


    }
    public void Attract(int x, int y, bool sure)
    {
        if (x > 1 && x < maxX - 1 && y > 1 && y < maxY - 1)
        {
            if (!sure)
            {
                HighLight[x + 2, y] = 3;
                HighLight[x - 2, y] = 3;
                HighLight[x, y + 2] = 3;
                HighLight[x, y - 2] = 3;
                HighLightShow();
            }
            else
            {
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

                HighLightDestroy();
            }
        }
    }
    public void Push(int x, int y, int PushX, int PushY, bool sure)
    {
        if (!sure)
        {
            if (x + 1 <= maxX) HighLight[x + 1, y] = 3;
            if (x - 1 >= 0) HighLight[x - 1, y] = 3;
            if (y + 1 <= maxY) HighLight[x, y + 1] = 3;
            if (y - 1 >= 0) HighLight[x, y - 1] = 3;
            HighLightShow();
        }
        else
        {
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
    private void PushCheck(int x, int y, int PushX, int PushY)
    {
        if (MapInfo[PushX, PushY] == 0)
        {
            Debug.Log("推出地图");
        }
        else if (GamePlayer[PushX, PushY] == 1)
        {
            Debug.Log("撞到人伤害");
        }
        else if (GamePlayer[PushX, PushY] == -1)
        {
            Debug.Log("撞到敌人伤害");
        }
        else if (MapInfo[PushX, PushY] == 900)
        {
            Debug.Log("撞到栅栏伤害");
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
        }
    }
    public void jump()
    {

    }

    /// <summary>
    /// 位置工具
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int GetLocation(float pos)
    {
        return ((int)pos - 20) / 40 + 1;
    }
    public int GetPosition(int loc)
    {
        return (loc - 1) * 40 + 20;
    }
    private int getDic(int x, int y)
    {
        return y * MAX_NUMBER + x;
    }
}
