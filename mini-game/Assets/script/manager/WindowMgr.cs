using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
/////////////////
定义UI窗口管理类

*/
public class WindowMgr : MonoBehaviour
{
    public static WindowMgr Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switch_window(string to_window_name)
    {
        

    }
}
