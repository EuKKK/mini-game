﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class startwnd : MonoBehaviour
{
    public GameObject menu;
    public GameObject new_game_btn_ob;
    public GameObject continue_game_btn_ob;
    public GameObject exit_btn_ob;

    // Start is called before the first frame update
    void Start()
    {
        register_btn_click();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        Button start_btn = new_game_btn_ob.GetComponent<Button>();
        Button continue_btn = continue_game_btn_ob.GetComponent<Button>();
        Button exit_btn = exit_btn_ob.GetComponent<Button>();

        start_btn.onClick.AddListener(start_game);
        continue_btn.onClick.AddListener(continue_game);
        exit_btn.onClick.AddListener(exit_game);
    }

    //开始游戏
    void start_game()
    {
         SceneManager.LoadScene("battle");
    }
    //继续游戏
    void continue_game()
    {

    }
    //退出游戏
    void exit_game()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
