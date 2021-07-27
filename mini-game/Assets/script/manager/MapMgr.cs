using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    public int[,] MapInfo = new int[30, 30];
    public int[,] GamePlayer = new int[30, 30];
    public GameObject mapInfo;
    public GameObject map;
    public GameObject landform;
    public GameObject highLight;
    public GameObject highLightD;
    public GameObject ex;
    public Dictionary<int, GameObject> highLightObj = new Dictionary<int, GameObject>();
    public int[,] HighLight = new int[30, 30];
    public bool stop;
    private int locationX;
    private int locationY;
    private float rotX;
    private float rotY;
    private float rotZ;

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
    private RouteObject[,] mapRoute = new RouteObject[30, 30];
    public bool change = true;
    int maxMove = 2;

    void Start()
    {
        GetMap();
        rotX = ex.transform.eulerAngles.x;
        rotY = ex.transform.eulerAngles.y;
        rotZ = ex.transform.eulerAngles.z;
        highLight = (GameObject)Resources.Load("Prefab/HighLight");
        highLightD = (GameObject)Resources.Load("Prefab/HighLightD");
        stop = false;
    }

    public void GetMap()
    {
        mapInfo = GameObject.FindGameObjectWithTag("MapInfo").gameObject;
        map = mapInfo.transform.Find("Map(Clone)").gameObject;
        landform = mapInfo.transform.Find("Landform(Clone)").gameObject;
        for (int i = 0; i < map.transform.childCount; i++)
        {
            GameObject g = map.transform.GetChild(i).gameObject;
            int x = GetLocation(g.transform.position.x);
            int y = GetLocation(g.transform.position.y);
            MapInfo[x, y] = 1;
        }
        for (int i = 0; i < landform.transform.childCount; i++)
        {
            GameObject g = landform.transform.GetChild(i).gameObject;
            string tag = g.tag;
            int x = GetLocation(g.transform.position.x);
            int y = GetLocation(g.transform.position.y);
            if (MapInfo[x, y] == 1)
            {
                switch (tag)
                {
                    case "character":
                        GamePlayer[x, y] = 1;
                        break;
                    case "enemy":
                        GamePlayer[x, y] = 2;
                        break;
                    case "barrier":
                        MapInfo[x, y] = 900;
                        break;
                    case "jungle":
                        MapInfo[x, y] = 2;
                        break;
                    case "water":
                        MapInfo[x, y] = 9999;
                        break;
                    case "trap":
                        MapInfo[x, y] = 500;
                        break;
                }
            }
        }

    }

    public int GetLocation(float pos)
    {
        return ((int)pos - 20) / 40;
    }
    public int GetPosition(int loc)
    {
        return loc * 40 + 20;
    }

    public void Routing()
    {

    }

    public void CharacterMove(int x, int y)
    {
        waySearch(x, y, maxMove, true);
        HighLightSearch(true);
        waySearch(x, y, maxMove, false);
        HighLightSearch(false);
        HighLightShow();

    }

    public void EnemyMove()
    {

    }
    private void waySearch(int _x, int _y, int point, bool trapAfford)
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
                    waySearch(_x, _y + 1, mapRoute[_x, _y + 1].movePoint, trapAfford);
                }
            }
            if (downPoint != 0)
            {
                if (point - downPoint > -100 && point - downPoint > mapRoute[_x, _y - 1].movePoint && GamePlayer[_x, _y - 1] == 0)
                {
                    mapRoute[_x, _y - 1].movePoint = point - downPoint;
                    mapRoute[_x, _y - 1].direction = Direction.down;
                    waySearch(_x, _y - 1, mapRoute[_x, _y - 1].movePoint, trapAfford);
                }
            }
            if (rightPoint != 0)
            {
                if (point - rightPoint > -100 && point - rightPoint > mapRoute[_x + 1, _y].movePoint && GamePlayer[_x + 1, _y] == 0)
                {
                    mapRoute[_x + 1, _y].movePoint = point - rightPoint;
                    mapRoute[_x + 1, _y].direction = Direction.right;
                    waySearch(_x + 1, _y, mapRoute[_x + 1, _y].movePoint, trapAfford);
                }
            }
            if (leftPoint != 0)
            {
                if (point - leftPoint > -100 && point - leftPoint > mapRoute[_x - 1, _y].movePoint && GamePlayer[_x - 1, _y] == 0)
                {
                    mapRoute[_x - 1, _y].movePoint = point - leftPoint;
                    mapRoute[_x - 1, _y].direction = Direction.left;
                    waySearch(_x - 1, _y, mapRoute[_x - 1, _y].movePoint, trapAfford);
                }
            }
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
    /*
    private void wayWalk(int targetX, int targetY)
    {
        if (targetX == x && targetY == y)
        {
            return;
        }
        else if (mapRoute[targetX, targetY].movePoint >= 0)
        {
            if (t == 0)
            {
                Debug.Log(targetX + "," + targetY);
                t = 1;
            }
            switch (mapRoute[targetX, targetY].direction)
            {
                case Direction.up:
                    wayWalk(targetX, targetY - 1);
                    break;
                case Direction.down:
                    wayWalk(targetX, targetY + 1);
                    break;
                case Direction.right:
                    wayWalk(targetX - 1, targetY);
                    break;
                case Direction.left:
                    wayWalk(targetX + 1, targetY);
                    break;
            }
        }
        else
        {
            switch (mapRoute[targetX, targetY].direction)
            {
                case Direction.up:
                    wayWalk(targetX, targetY - 1);
                    break;
                case Direction.down:
                    wayWalk(targetX, targetY + 1);
                    break;
                case Direction.right:
                    wayWalk(targetX - 1, targetY);
                    break;
                case Direction.left:
                    wayWalk(targetX + 1, targetY);
                    break;
            }
        }
    
    }
    */
    void Update()
    {
        OnMouseMove();
    }
    private void OnMouseMove()
    {

        if (Input.GetMouseButtonUp(0))
        {
            HighLightDestroy();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit rh;
            bool hit = Physics.Raycast(ray, out rh);

            if (hit)
            {
                GameObject target = rh.collider.gameObject;
                locationX = GetLocation(target.transform.position.x);
                locationY = GetLocation(target.transform.position.y); ;
            }
            //stop = true;
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    mapRoute[i, j].movePoint = -999;
                    mapRoute[i, j].direction = Direction.stand;
                }
            }

            mapRoute[locationX, locationY].direction = Direction.stand;
            mapRoute[locationX, locationY].movePoint = maxMove;
            CharacterMove(locationX, locationY);
        }
    }
}
