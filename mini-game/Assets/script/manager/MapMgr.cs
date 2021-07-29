using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    public const int MAX_NUMBER = 30;
    public int[,] MapInfo = new int[35, 35];
    public int[,] GamePlayer = new int[35, 35];
    public Dictionary<int, GameObject> landformPos = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> enemyPos = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> characterPos = new Dictionary<int, GameObject>();
    public GameObject mapInfo;
    public GameObject map;
    public GameObject landform;
    public int locationX;
    public int locationY;
    public int maxX;
    public int maxY;
    public string MgrCheck;

    void Start()
    {
        GetMap();
        MgrCheck = "BattleMgr";
    }

    public void GetMap()
    {
        mapInfo = GameObject.FindGameObjectWithTag("MapInfo").gameObject;
        map = mapInfo.transform.Find("Map(Clone)").gameObject;
        landform = mapInfo.transform.Find("Landform(Clone)").gameObject;
        maxX = 0;
        maxY = 0;
        for (int i = 0; i < 35; i++)
        {
            for (int j = 0; j < 35; j++)
            {
                MapInfo[i, j] = 0;
                GamePlayer[i, j] = 0;
            }
        }
        for (int i = 0; i < map.transform.childCount; i++)
        {
            GameObject g = map.transform.GetChild(i).gameObject;
            int x = GetLocation(g.transform.position.x);
            int y = GetLocation(g.transform.position.y);
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
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
                        g.transform.position = g.transform.position + new Vector3(0, 0, -0.9f);
                        characterPos.Add(getDic(x, y), g);
                        break;
                    case "enemy":
                        GamePlayer[x, y] = -1;
                        g.transform.position = g.transform.position + new Vector3(0, 0, -0.9f);
                        enemyPos.Add(getDic(x, y), g);
                        break;
                    case "barrier":
                        MapInfo[x, y] = 900;
                        landformPos.Add(getDic(x, y), g);
                        break;
                    case "jungle":
                        MapInfo[x, y] = 2;
                        landformPos.Add(getDic(x, y), g);
                        break;
                    case "water":
                        MapInfo[x, y] = 9999;
                        landformPos.Add(getDic(x, y), g);
                        break;
                    case "trap":
                        MapInfo[x, y] = 500;
                        landformPos.Add(getDic(x, y), g);
                        break;
                }
            }
        }

    }

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

    void Update()
    {
        OnMouseMove();
    }
    private void OnMouseMove()
    {

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rh;
            bool hit = Physics.Raycast(ray, out rh);
            if (hit)
            {
                GameObject target = rh.collider.gameObject;
                locationX = GetLocation(target.transform.position.x);
                locationY = GetLocation(target.transform.position.y);

                if (MgrCheck == "BattleMgr")
                {
                    GetComponent<BattleMgr>().CharacterCheck(locationX, locationY, ref target);
                }
            }

        }
    }


}
