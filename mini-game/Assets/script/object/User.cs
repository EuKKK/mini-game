using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;

public class User : MonoBehaviour
{
    public int money;
    public int level;
    public int character_num;
    public static User Instance { get; private set; }
    public Dictionary <int, sheep> sheep_map = new Dictionary<int, sheep>();
    public int sheep_id = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        add_test_sheep();
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
        }
    }
    public Dictionary <int, sheep> get_sheeps()
    {
        return sheep_map;
    }
}
