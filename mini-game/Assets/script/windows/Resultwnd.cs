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
    public GameObject text;
    public bool textCheck;
    public int battleResult;

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
 if (battleResult == 1) Game.Instance.player_save();Game.Instance.switch_music("normal");        MapMgr.Instance.leave_battle();
        WindowMgr.Instance.switch_window("Mode");

    }

    void win()
    {
        battleResult = 1;
        text.GetComponent<Text>().text = "胜利";
    }
    void lose()
    {
        battleResult = 0;
        text.GetComponent<Text>().text = "失败";
    }
    // Update is called once per frame
    void Update()
    {
        if (MapMgr.Instance.isWin)
        {
            win();
        }
        else if (MapMgr.Instance.isLose)
        {
            lose();
        }
    }
}
