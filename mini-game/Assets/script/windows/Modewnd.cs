using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;

public class Modewnd : window
{
    public GameObject menu;
    public GameObject battle_btn_ob;
    public GameObject shop_btn_ob;
    public GameObject guide_btn_ob;
    public GameObject back_btn_ob;
    public InputField level_text;

    // Start is called before the first frame update
    void Start()
    {
        register_btn_click();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        Button battle_btn = battle_btn_ob.GetComponent<Button>();
        Button shop_btn = shop_btn_ob.GetComponent<Button>();
        Button guide_btn = guide_btn_ob.GetComponent<Button>();
        Button back_btn = back_btn_ob.GetComponent<Button>();

        battle_btn.onClick.AddListener(battle_start);
        shop_btn.onClick.AddListener(open_shop);
        guide_btn.onClick.AddListener(open_guide);
        back_btn.onClick.AddListener(back);
    }

    //开始战斗
    void battle_start()
    {
        Game.Instance.switch_music("battle");
        if(level_text.text != "1")
            User.Instance.level = User.Instance.level + int.Parse(level_text.text) - 1;
        WindowMgr.Instance.switch_window("Battlestandby");
        MapMgr.Instance.GetMap();
    }

    //打开商店
    void open_shop()
    {
        WindowMgr.Instance.switch_window("Shop");
    }
    //打开指引
    void open_guide()
    {

    }

    void back()
    {
        WindowMgr.Instance.switch_window("Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
