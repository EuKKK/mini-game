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
    public int locationX;
    public int locationY;
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
                        g.transform.position = g.transform.position + new Vector3(0, 0, -0.9f);
                        break;
                    case "enemy":
                        GamePlayer[x, y] = -1;
                        g.transform.position = g.transform.position + new Vector3(0, 0, -0.9f);
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
