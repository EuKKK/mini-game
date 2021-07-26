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
    public Dictionary <string, sheep> sheep_map = new Dictionary<string, sheep>();
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<string, sheep> get_sheeps()
    {
        return sheep_map;
    }
}
