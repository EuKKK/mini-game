using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using BaseObject;
using UnityEditor;
using UnityEngine.UI;
public class GlobalFuncMgr 
{
    static int sheep_id = 0;
    void Start()
    {
        //test_asset();
    }

    // Update is called once per frame
    public static void set_model_sprite(GameObject model, string sprite_name)
    {
        string path = "Image/" + sprite_name;
        SpriteRenderer sheep_renderer = model.GetComponent<SpriteRenderer>();
        Texture2D new_sprite = Resources.Load(path) as Texture2D;
        sheep_renderer.sprite = Sprite.Create(new_sprite, new Rect(0, 0, new_sprite.width, new_sprite.height), new Vector2(.5f, .5f));
    }
    public static void set_image(GameObject model, string sprite_name)
    {
        string path = "Image/" + sprite_name;
        Image sheep_renderer = model.GetComponent<Image>();
        Texture2D new_sprite = Resources.Load(path) as Texture2D;
        sheep_renderer.sprite = Sprite.Create(new_sprite, new Rect(0, 0, new_sprite.width, new_sprite.height), new Vector2(.5f, .5f));
    }
    public static int new_sheep_id()
    {
        sheep_id ++;
        return sheep_id;
    }
}
