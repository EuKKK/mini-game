using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using BaseObject;
using UnityEditor;
public class StoryMgr:MonoBehaviour 
{
    public static StoryMgr Instance { get; private set; }

    public string [] stroy_lines = new string[14];
    public int[] story_chapters = new int[5];
    public bool need_result = false;
    int story_num = 0;
    int now_stroy_id = -1;
    void Awake()
    {
        Instance = this;
    }
    public void start_story(int story_id)
    {
        story_num = 0;
        now_stroy_id = story_id;
        for(int i = 0; i < story_id -1; i++)
            story_num += story_chapters[i];
        WindowMgr.Instance.active_window("Story");
    }
    public void push_stroy()
    {
        if(now_stroy_id!=-1)
        {
            story_num ++;
            int total = 0;
            for(int i=0;i<now_stroy_id;i++)
                total += story_chapters[i];
            if(story_num == total)
                end_stroy();
            else
                WindowMgr.Instance.active_window("Story");
        }
    }
    public void end_stroy()
    {
        WindowMgr.Instance.close_window("Story");
        if(need_result)
        {
            WindowMgr.Instance.active_window("Result");
            need_result = false;
        }
    }
    public string get_story()
    {
        return stroy_lines[story_num];
    }

}
