using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using System;

public class User : MonoBehaviour
{
    public int money;
    public int level;
    public int character_num;
    public static User Instance { get; private set; }
    public Dictionary <int, sheep> sheep_map = new Dictionary<int, sheep>();
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //Game.Instance.load_saves();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void test()
    {

    }
    public void add_test_sheep()
    {
        for(int i=1;i<=5;i++)
        {
            sheep new_sheep = new sheep();
            new_sheep.test();
            sheep_map[i] = new_sheep;
            new_sheep.set_id(i);
        }
    }
    public Dictionary <int, sheep> get_sheeps()
    {
        return sheep_map;
    }
    public void add_sheep(int id, sheep u_sheep)
    {
        sheep_map[id] = u_sheep;
    }

}
