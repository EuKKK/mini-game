using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;

public class MapMgr : MonoBehaviour
{
    public const int MAX_NUMBER = 30;
    public int[,] MapInfo;
    public int[,] GamePlayer;
    public Dictionary<int, GameObject> landformPos;
    public Dictionary<int, GameObject> enemyPos;
    public Dictionary<int, GameObject> characterPos;
    public Dictionary<GameObject, sheep> enemy_GO = new Dictionary<GameObject, sheep>();
    public GameObject mapInfo;
    public GameObject map;
    public GameObject landform;
    public int locationX;
    public int locationY;
    public int maxX;
    public int maxY;

    public static MapMgr Instance { get; private set; }
    public Camera BattleCamera;


    private bool isMouseDown;
    private Vector3 lastMousePosition;
    private bool Playing = false;
    List<int[]> play_pos ;
    int monster_num;

    void Start()
    {
        Instance = this;

    }

    public void GetMap()
    {
        //初始化地图数据
        map_init();

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
                        int[] pos = new int[2];
                        pos[0] = GetPosition(x);
                        pos[1] = GetPosition(y);
                        play_pos.Add(pos);
                        //GamePlayer[x, y] = 1;
                        //g.transform.position = g.transform.position + new Vector3(0, 0, -0.9f);
                        //characterPos.Add(getDic(x, y), g);
                        break;
                    
                    case "enemy":
                        GamePlayer[x, y] = -1;
                        g.transform.position = g.transform.position + new Vector3(0, 0, -0.9f);
                        sheep enter_sheep = new sheep(i.ToString());
                        enter_sheep.hp = 10;
                        enter_sheep.move_range = 5;
                        enter_sheep.cordon = 5;
                        enter_sheep.this_sheep = g;
                        enter_sheep.this_sheep.layer = 9;
                        Transform[] father = enter_sheep.this_sheep.GetComponentsInChildren<Transform>();
                        foreach (Transform child in father)
                            child.gameObject.layer = 9;
                        enemy_GO[enter_sheep.this_sheep] = enter_sheep;

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

        //特殊处理第一关和第二关的不选人
        if(User.Instance.level == "6001" )
        {
            for(int i = 1;i <= 3; i++)
            {
                FormationMgr.Instance.enter_team(play_pos[i-1][0], play_pos[i-1][1], User.Instance.get_sheep_by_id(i));
            }
            GetSheep();
            WindowMgr.Instance.switch_window("Battle");
            BattleMgr.Instance.GameStart();
        }
        if (User.Instance.level == "6002")
        {
            for(int i = 1;i <= 4; i++)
            {
                FormationMgr.Instance.enter_team(play_pos[i-1][0], play_pos[i-1][1], User.Instance.get_sheep_by_id(i));
            }
            GetSheep();
            WindowMgr.Instance.switch_window("Battle");
            BattleMgr.Instance.GameStart();
        }

    }

    public void GetSheep()
    {
        foreach (int i in FormationMgr.Instance.sheep_position.Keys)
        {
            int x = DicX(i);
            int y = DicY(i);

            GamePlayer[x, y] = 1;
            characterPos.Add(i, FormationMgr.Instance.sheep_position[i].this_sheep);

        }
        Playing = true;
    }

    void map_init()
    {
        string map_name = ExcMgr.Instance.get_data("stage", User.Instance.level, "地图");
        mapInfo = Instantiate((GameObject)Resources.Load("MapPrefab/" + map_name));
        Transform[] father = mapInfo.GetComponentsInChildren<Transform>();
        foreach (Transform child in father)
            child.gameObject.layer = 9;

        map = mapInfo.transform.Find("Map(Clone)").gameObject;
        landform = mapInfo.transform.Find("Landform(Clone)").gameObject;
        landformPos = new Dictionary<int, GameObject>();
        enemyPos = new Dictionary<int, GameObject>();
        characterPos = new Dictionary<int, GameObject>();

        maxX = 0;
        maxY = 0;
        monster_num = 1;

        MapInfo = new int[35, 35];
        GamePlayer = new int[35, 35];
        play_pos = new List<int[]>();
    }

    public int GetLocation(float pos)
    {
        return ((int)pos - 20) / 40 + 1;
    }
    public int GetPosition(int loc)
    {
        return (loc - 1) * 40 + 20;
    }
    public int getDic(int x, int y)
    {
        return y * MAX_NUMBER + x;
    }
    public int DicY(int dicNumber)
    {
        return (dicNumber - 1) / MAX_NUMBER;
    }
    public int DicX(int dicNumber)
    {
        return dicNumber - DicY(dicNumber) * MAX_NUMBER;
    }


    void Update()
    {
        if (Playing)
        {
            OnMouseMove();
        }
        // if (!GetComponent<BattleMgr>().ScreenLock)
        // {
        //     ScreenMove();
        // }
    }
    private void OnMouseMove()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = BattleCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rh;
            bool hit = Physics.Raycast(ray, out rh);
            
            if (hit)
            {

                GameObject target = rh.collider.gameObject;
                Debug.Log(target.name);
                locationX = GetLocation(target.transform.position.x);
                locationY = GetLocation(target.transform.position.y);

                BattleMgr.Instance.CenterManager(locationX, locationY, ref target, 0);

            }
        }
        if (Input.GetMouseButtonDown(1))
        {

            Ray ray = BattleCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rh;
            bool hit = Physics.Raycast(ray, out rh);
            if (hit)
            {
                GameObject target = rh.collider.gameObject;
                locationX = GetLocation(target.transform.position.x);
                locationY = GetLocation(target.transform.position.y);


                BattleMgr.Instance.CenterManager(locationX, locationY, ref target, 1);

            }

        }
    }

    public int[] GetChracPos(Vector3 pos)
    {
        int[] map_pos = null;
        Ray ray = BattleCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rh;
        bool hit = Physics.Raycast(ray, out rh);
        if (hit)
        {
            map_pos = new int[2];
            GameObject target = rh.collider.gameObject;
            
            if (target.layer != 9) return null;
            if (!target.name.Contains("character")) return null; 

            locationX = GetLocation(target.transform.position.x);
            locationY = GetLocation(target.transform.position.y);
            map_pos[0] = locationX;
            map_pos[1] = locationY;
        }
        return map_pos;
    }


    private void ScreenMove()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isMouseDown = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isMouseDown = false;
            lastMousePosition = Vector3.zero;
        }
        if (isMouseDown)
        {
            if (lastMousePosition != Vector3.zero)
            {
                Vector3 offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;
                mapInfo.transform.position += offset;
            }
            lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    public void leave_battle()
    {
        if (mapInfo)
            Destroy(mapInfo);

        mapInfo = null;
        GamePlayer = null;
        landformPos = null;
        enemyPos = null;
        characterPos = null;
        map = null;
    }
}
