﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using System;

public class User : MonoBehaviour
{
    public int money;
    public string level;
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
    // public void add_test_sheep()
    // {
    //     for(int i=1;i<=5;i++)
    //     {
    //         sheep new_sheep = new sheep();
    //         new_sheep.test();
    //         sheep_map[i] = new_sheep;
    //         new_sheep.set_id(i);
    //     }
    // }
    public Dictionary <int, sheep> get_sheeps()
    {
        return sheep_map;
    }
    public void add_sheep(sheep u_sheep)
    {
        int sheep_id = u_sheep.get_id();
        sheep_map[sheep_id] = u_sheep;
    }

    public sheep get_sheep_by_id(int id)
    {
        return sheep_map[id];
    }
    public void init()
    {
        level = "6001";
        money = 0;
        sheep new_sheep_1 = new sheep();
        sheep new_sheep_2 = new sheep();
        sheep new_sheep_3 = new sheep();
        new_sheep_1.load_data("1201");
        new_sheep_2.load_data("1005");
        new_sheep_3.load_data("1007");
        sheep_map[1] = new_sheep_1;
        sheep_map[2] = new_sheep_2;
        sheep_map[3] = new_sheep_3;
    }

}
