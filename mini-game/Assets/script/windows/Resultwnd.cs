using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;

public class Resultwnd : window
{
    public GameObject menu;
    public GameObject back_ob;

    // Start is called before the first frame update
    void Start()
    {
        register_btn_click();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        Button back_btn = back_ob.GetComponent<Button>();

        back_btn.onClick.AddListener(back);
    }

    //结束战斗
    void back()
    {
        Game.Instance.player_save();
        WindowMgr.Instance.switch_window("Mode");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
