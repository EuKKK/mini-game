using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapDrawer : EditorWindow
{
    static int WIDE = 25;

    string maxRow = string.Empty;
    string maxCol = string.Empty;
    private int _select = 0;
    private Texture2D[] items = new Texture2D[12];
    private bool _drag = false;
    public int[,] map = new int[25, 25];
    private Object prefabTest;
    private bool isCreate;
    private bool isEliminate;
    private Dictionary<int, GameObject> prefabGO = new Dictionary<int, GameObject>();

    [MenuItem("地图/创建背景地块")]
    public static void OpenMapCreate()
    {
        MapDrawer window = EditorWindow.GetWindow<MapDrawer>("地图编辑器");
        window.Show();
        window.minSize = new Vector2(400, 800);//设置最大和最小
        window.maxSize = new Vector2(400, 1200);
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        //初始化一些东西
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }


    void OnSceneGUI(SceneView sceneView)
    {

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));//为scene响应添加默认事件,用来屏蔽以前的点击选中物体
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)//点击
        {
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)//抬起
        {
            if (!_drag)
            {
                OnMouseEvent();
            }

            _drag = false;
        }
        else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)//拖动
        {
            OnMouseEvent();
            _drag = true;
        }

    }

    private void OnMouseEvent()
    {

        Vector2 mousePos = Event.current.mousePosition;//获取鼠标坐标
        mousePos.y = Camera.current.pixelHeight - mousePos.y;//这里的鼠标原点在左上,而屏幕空间原点左下,翻转它

        Ray ray = Camera.current.ScreenPointToRay(mousePos);
        RaycastHit rh;
        int cost = 0;
        switch (_select)
        {
            case 0:
                cost = 1;
                prefabTest = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorPrefab/test1.prefab", typeof(Object));
                break;
            case 1:
                cost = 999;
                prefabTest = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorPrefab/test2.prefab", typeof(Object));
                break;
            case 2:
                cost = 999;
                prefabTest = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorPrefab/grass.prefab", typeof(Object));
                break;
        }
        bool hit = Physics.Raycast(ray, out rh, 3000f);
        if (hit && isCreate)
        {
            int horizontal = Mathf.FloorToInt(rh.point.x / 40);
            int vertical = Mathf.FloorToInt(rh.point.y / 40);
            if (map[horizontal, vertical] == 0)
            {
                map[horizontal, vertical] = cost;
                GameObject thisPrefab = (GameObject)Instantiate(prefabTest, new Vector3(horizontal * 40 + 20, vertical * 40 + 20), new Quaternion(0, 0, 0, 0));
                prefabGO.Add(horizontal + vertical * WIDE, thisPrefab);
            }
        }

        if (hit && isEliminate)
        {
            int horizontal = Mathf.FloorToInt(rh.point.x / 40);
            int vertical = Mathf.FloorToInt(rh.point.y / 40);
            if (map[horizontal, vertical] != 0)
            {
                map[horizontal, vertical] = 0;

                Object deleteObject = prefabGO[horizontal + vertical * WIDE];
                prefabGO.Remove(horizontal + vertical * WIDE);
                DestroyImmediate((GameObject)deleteObject);

            }
        }
    }




    void OnGUI()
    {

        //maxRow = EditorGUILayout.TextField("Row(最大行数)", maxRow);
        //maxCol = EditorGUILayout.TextField("Col(最大列数)", maxCol);
        if (GUILayout.Button("开始绘画"))
        {
            isCreate = true;
            isEliminate = false;
        }
        if (GUILayout.Button("擦除地图"))
        {
            isEliminate = true;
            isCreate = false;
        }
        if (GUILayout.Button("取消"))
        {
            isCreate = false;
            isEliminate = false;
        }
        if (GUILayout.Button("结束绘画"))
        {
            EndDraw();
        }
        EditorGUILayout.BeginHorizontal("box");
        int sizeY = 100 * Mathf.CeilToInt(items.Length / 4f);


        _select = GUI.SelectionGrid(new Rect(new Vector2(0, 155), new Vector2(100 * 4, sizeY)), _select, items, 4);//可以给出grid选择框,需要传入贴图数组_items


        items[0] = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorTexture/test1.jpeg", typeof(Texture2D));
        items[1] = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorTexture/test2.jpeg", typeof(Texture2D));
        items[2] = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorTexture/grass.png", typeof(Texture2D));

    }

    void EndDraw()
    {
        GameObject Map = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorPrefab/Map.prefab", typeof(Object)), Vector3.zero, new Quaternion(0, 0, 0, 0));
        foreach (GameObject g in prefabGO.Values)
        {
            g.transform.SetParent(Map.transform, false);
        }
        Close();
    }
}
