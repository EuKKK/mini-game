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
    int story_num = 0;
    int now_stroy_num = -1;
    void Awake()
    {
        Instance = this;
    }
    public void start_story(int story_id)
    {
        story_num = 0;
        now_stroy_num = story_id;
        for(int i = 0; i < story_id -1; i++)
        {
            story_num += story_chapters[i];
        }
        WindowMgr.Instance.active_window("Story");
    }
    public void push_stroy()
    {
        if(now_stroy_num!=-1)
        {
            story_num ++;
            if(story_num == story_chapters[now_stroy_num])
                end_stroy();
            else
                WindowMgr.Instance.active_window("Story");
        }
    }
    public void end_stroy()
    {
        WindowMgr.Instance.close_window("Story");
    }

    public int get_now_story_num()
    {
        return now_stroy_num;
    }

    public string get_story()
    {
        return stroy_lines[story_num];
    }

}
