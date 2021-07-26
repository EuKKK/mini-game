﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;

public class Battlestandbywnd : window
{
    public GameObject menu;
    public GameObject battle_btn_ob;

    // Start is called before the first frame update
    void Start()
    {
        register_btn_click();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        Button battle_btn = battle_btn_ob.GetComponent<Button>();

        battle_btn.onClick.AddListener(battle_start);
    }

    //开始战斗
    void battle_start()
    {
        WindowMgr.Instance.switch_window("Battle");
    }

    void back()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
