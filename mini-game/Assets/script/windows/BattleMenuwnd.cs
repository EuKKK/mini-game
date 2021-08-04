using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;

public class BattleMenuwnd : window
{
    public Button exit_btn;
    public Button cancel_btn;
    void Start()
    {
        register_btn_click();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        exit_btn.onClick.AddListener(exit);
        cancel_btn.onClick.AddListener(cancel);
    }
    void exit()
    {
        Game.Instance.switch_music("normal");   
        BattleMgr.Instance.BattleExit();
        WindowMgr.Instance.switch_window("Start");
        MapMgr.Instance.leave_battle();
    }
    void cancel()
    {
        this.gameObject.SetActive(false);
    }
}
