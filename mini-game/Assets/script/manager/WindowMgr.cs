using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseObject;
using System.IO;


/*
/////////////////
定义UI窗口管理类

*/
public class WindowMgr : MonoBehaviour
{

    public Dictionary<string, window> window_map =  new Dictionary<string, window>();
    public static WindowMgr Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
    }
    void Start()
    {
        get_all_windows();
        switch_window("Start");
    }

    //获取所有窗口脚本
    void get_all_windows()
    {
        string component_name;
        GameObject now_child;
        Transform father = this.transform;
        for(int i = 0;i < father.childCount; i++)
        {
            now_child = father.GetChild(i).gameObject;
            component_name = now_child.name + "wnd";
            window_map[now_child.name] = now_child.GetComponent<window>();
        }
    }

    public void switch_window(string to_window_name)
    {
        foreach(string key in window_map.Keys)
        {
            window now_window = window_map[key];
            if(key == to_window_name)
                now_window.redraw();
            else
                now_window.close();
        }
    }

    public void active_window(string window_name)
    {
        window_map[window_name].redraw();
    }
}
