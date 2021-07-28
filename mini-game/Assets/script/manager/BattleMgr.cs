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
    public int[,] MapInfo = new int[30, 30];
    public int[,] GamePlayer = new int[30, 30];
    public int maxX;
    public int maxY;
    private Direction[] DangerousTry = new Direction[10];
    private int wayCount;
    private int trapCount;
    private Direction[] way = new Direction[10];
    private int step = 0;
    public int[,] HighLight = new int[30, 30];
    private bool isWalk;
    private bool walkType;

    //private int locationX;
    //private int locationY;
    private int CharacterX;
    private int CharacterY;
    private float rotX;
    private float rotY;
    private float rotZ;

    private RouteObject[,] mapRoute = new RouteObject[30, 30];
    private bool check = true;
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
    public int GetLocation(float pos)
    {
        return ((int)pos - 20) / 40;
    }
    public int GetPosition(int loc)
    {
        return loc * 40 + 20;
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
        maxX = GetComponent<MapMgr>().maxX;
        maxY = GetComponent<MapMgr>().maxY;

    }









    /// <summary>
    /// 寻路
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CharacterMove(int x, int y)
    {
        SafeWaySearch(x, y, maxMove, true);
        HighLightSearch(true);
        MapInfoInit();
        mapRoute[x, y].movePoint = maxMove;
        SafeWaySearch(x, y, maxMove, false);
        HighLightSearch(false);
        HighLightShow();

    }
    public void EnemyMove()
    {

    }
    private void SafeWaySearch(int _x, int _y, int point, bool trapAfford)
    {
        int upPoint = 0;
        int downPoint = 0;
        int leftPoint = 0;
        int rightPoint = 0;
        if (_y + 1 < 30)
        {
            upPoint = MapInfo[_x, _y + 1];
            if (trapAfford && upPoint == 500) upPoint = 1;
        }
        if (_y - 1 >= 0)
        {
            downPoint = MapInfo[_x, _y - 1];
            if (trapAfford && downPoint == 500) downPoint = 1;
        }
        if (_x + 1 < 30)
        {
            rightPoint = MapInfo[_x + 1, _y];
            if (trapAfford && rightPoint == 500) rightPoint = 1;
        }
        if (_x - 1 >= 0)
        {
            leftPoint = MapInfo[_x - 1, _y];
            if (trapAfford && leftPoint == 500) leftPoint = 1;
        }
        if (point >= 0)
        {
            if (upPoint != 0)
            {
                if (point - upPoint > -100 && point - upPoint > mapRoute[_x, _y + 1].movePoint && GamePlayer[_x, _y + 1] == 0)
                {
                    mapRoute[_x, _y + 1].movePoint = point - upPoint;
                    mapRoute[_x, _y + 1].direction = Direction.up;
                    SafeWaySearch(_x, _y + 1, mapRoute[_x, _y + 1].movePoint, trapAfford);
                }
            }
            if (downPoint != 0)
            {
                if (point - downPoint > -100 && point - downPoint > mapRoute[_x, _y - 1].movePoint && GamePlayer[_x, _y - 1] == 0)
                {
                    mapRoute[_x, _y - 1].movePoint = point - downPoint;
                    mapRoute[_x, _y - 1].direction = Direction.down;
                    SafeWaySearch(_x, _y - 1, mapRoute[_x, _y - 1].movePoint, trapAfford);
                }
            }
            if (rightPoint != 0)
            {
                if (point - rightPoint > -100 && point - rightPoint > mapRoute[_x + 1, _y].movePoint && GamePlayer[_x + 1, _y] == 0)
                {
                    mapRoute[_x + 1, _y].movePoint = point - rightPoint;
                    mapRoute[_x + 1, _y].direction = Direction.right;
                    SafeWaySearch(_x + 1, _y, mapRoute[_x + 1, _y].movePoint, trapAfford);
                }
            }
            if (leftPoint != 0)
            {
                if (point - leftPoint > -100 && point - leftPoint > mapRoute[_x - 1, _y].movePoint && GamePlayer[_x - 1, _y] == 0)
                {
                    mapRoute[_x - 1, _y].movePoint = point - leftPoint;
                    mapRoute[_x - 1, _y].direction = Direction.left;
                    SafeWaySearch(_x - 1, _y, mapRoute[_x - 1, _y].movePoint, trapAfford);
                }
            }
        }
    }

    public void DangerousWayWalk(int _x, int _y, int targetX, int targetY, int point, int count)
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
            if (upPoint == 500)
            {
                upPoint = 1;
                signUp = 1;
            }
        }
        if (_y - 1 >= 0)
        {
            downPoint = MapInfo[_x, _y - 1];
            if (downPoint == 500)
            {
                downPoint = 1;
                signDown = 1;
            }
        }
        if (_x + 1 < 30)
        {
            rightPoint = MapInfo[_x + 1, _y];
            if (rightPoint == 500)
            {
                rightPoint = 1;
                signRight = 1;
            }
        }
        if (_x - 1 >= 0)
        {
            leftPoint = MapInfo[_x - 1, _y];
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
                    DangerousWayWalk(_x, _y + 1, targetX, targetY, mapRoute[_x, _y + 1].movePoint, count + signUp);
                    mapRoute[_x, _y + 1].movePoint = -999;
                }
            }
            if (downPoint != 0)
            {
                if (point - downPoint > -100 && mapRoute[_x, _y - 1].movePoint < 0 && GamePlayer[_x, _y - 1] == 0)
                {
                    mapRoute[_x, _y - 1].movePoint = point - downPoint;
                    DangerousTry[step] = Direction.down;
                    DangerousWayWalk(_x, _y - 1, targetX, targetY, mapRoute[_x, _y - 1].movePoint, count + signDown);
                    mapRoute[_x, _y - 1].movePoint = -999;
                }
            }
            if (rightPoint != 0)
            {
                if (point - rightPoint > -100 && mapRoute[_x + 1, _y].movePoint < 0 && GamePlayer[_x + 1, _y] == 0)
                {
                    mapRoute[_x + 1, _y].movePoint = point - rightPoint;
                    DangerousTry[step] = Direction.right;
                    DangerousWayWalk(_x + 1, _y, targetX, targetY, mapRoute[_x + 1, _y].movePoint, count + signRight);
                    mapRoute[_x + 1, _y].movePoint = -999;
                }
            }
            if (leftPoint != 0)
            {
                if (point - leftPoint > -100 && mapRoute[_x - 1, _y].movePoint < 0 && GamePlayer[_x - 1, _y] == 0)
                {
                    mapRoute[_x - 1, _y].movePoint = point - leftPoint;
                    DangerousTry[step] = Direction.left;
                    DangerousWayWalk(_x - 1, _y, targetX, targetY, mapRoute[_x - 1, _y].movePoint, count + signLeft);
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

    public void CharacterCheck(int locationX, int locationY, ref GameObject target)
    {

        if (GamePlayer[locationX, locationY] == 1)
        {
            character = target;
            MapInfoInit();
            HighLightDestroy();
            mapRoute[locationX, locationY].direction = Direction.stand;
            mapRoute[locationX, locationY].movePoint = maxMove;
            CharacterX = locationX;
            CharacterY = locationY;
            CharacterMove(locationX, locationY);
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
            MapInfoInit();
            mapRoute[CharacterX, CharacterY].direction = Direction.stand;
            mapRoute[CharacterX, CharacterY].movePoint = maxMove;
            DangerousWayWalk(CharacterX, CharacterY, locationX, locationY, maxMove, 0);

            GamePlayer[CharacterX, CharacterY] = 0;
            GamePlayer[locationX, locationY] = 1;
            step = wayCount;
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
    private void MapInfoInit()
    {
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                mapRoute[i, j].movePoint = -999;
                mapRoute[i, j].direction = Direction.stand;
            }
        }
        for (int i = 0; i < 10; i++)
        {
            way[i] = Direction.stand;
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
        OnMouseMove();

        if (isWalk)
        {
            Walk(walkType);
            isWalk = false;
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

                if (HighLight[locationX, locationY] == 3)
                {
                    SkillChoose(CharacterX, CharacterY, true);
                }
                else if (GamePlayer[locationX, locationY] == 1)
                {
                    HighLightDestroy();
                    SkillChoose(locationX, locationY, false);
                    CharacterX = locationX;
                    CharacterY = locationY;
                }
                else
                {
                    HighLightDestroy();
                }
            }

        }
    }
    private void SkillChoose(int x, int y, bool sure)
    {
        Rotate(x, y, sure);
    }

    /// <summary>
    /// 技能
    /// </summary>
    public void Attract()
    {

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
                int g1P = (x + 1) * MAX_NUMBER + y + 1;
                int g2P = (x - 1) * MAX_NUMBER + y + 1;
                int g3P = (x - 1) * MAX_NUMBER + y - 1;
                int g4P = (x + 1) * MAX_NUMBER + y - 1;
                GameObject g1 = landformPos[g1P];
                GameObject g2 = landformPos[g2P];
                GameObject g3 = landformPos[g3P];
                GameObject g4 = landformPos[g4P];
                landformPos.Remove(g1P);
                landformPos.Remove(g2P);
                landformPos.Remove(g3P);
                landformPos.Remove(g4P);
                g1.transform.position = g1.transform.position + new Vector3(0, -80, 0);
                g2.transform.position = g2.transform.position + new Vector3(80, 0, 0);
                g3.transform.position = g3.transform.position + new Vector3(0, 80, 0);
                g4.transform.position = g4.transform.position + new Vector3(-80, 0, 0);
                t = g1P;
                g1P = g2P;
                g2P = g3P;
                g3P = g4P;
                g4P = t;
                landformPos.Add(g1P, g1);
                landformPos.Add(g2P, g2);
                landformPos.Add(g3P, g3);
                landformPos.Add(g4P, g4);
                HighLightDestroy();
            }
        }
    }
    public void Push()
    {

    }
    public void jump()
    {

    }
}
