using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
