using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using BaseObject;
using UnityEditor;
public class GlobalFuncMgr 
{
    void Start()
    {
        //test_asset();
    }

    // Update is called once per frame
    public static void set_model_sprite(GameObject model, string sprite_name)
    {
        SpriteRenderer sheep_renderer = model.GetComponent<SpriteRenderer>();
        Texture2D new_sprite = Resources.Load(sprite_name) as Texture2D;
        sheep_renderer.sprite = Sprite.Create(new_sprite, new Rect(0, 0, new_sprite.width, new_sprite.height), new Vector2(.5f, .5f));
    }
}
