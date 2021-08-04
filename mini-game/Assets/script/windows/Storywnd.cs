using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;

public class Storywnd : window
{
     
    public Button next_btn;
    public Text story_text;
    void Start()
    {
        next_btn.onClick.AddListener(push_next);
    }
    //注册按钮点击监听
    void register_btn_click()
    {
    }
    void exit()
    {

    }

    public override void redraw(GameObject window = null)
    {
        if(!window) 
            window = this.gameObject;

        window.SetActive(true);

        int story_num = StoryMgr.Instance.get_now_story_num();
        story_text.text = StoryMgr.Instance.get_story();
    }
    void push_next()
    {
        StoryMgr.Instance.push_stroy();
    }
}
