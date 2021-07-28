using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using excel_d;
using System.IO;
public class BuildAssets : EditorWindow
{
    static string file_path = "Assets/Resources/doc/";
    [MenuItem("BuildAsset/Build Scriptable Asset")]
    public static void ExcuteBuild()
    {
        if (!Directory.Exists(file_path))
            return;

        DirectoryInfo direction = new DirectoryInfo(file_path);  
        FileInfo[] files = direction.GetFiles("*",SearchOption.AllDirectories);  

        for(int i=0;i<files.Length;i++){  
            if (files[i].Name.EndsWith(".xlsx")){  
                Dictionary<string, Dictionary<string, string >> excel_map;
                ExcelAccess.SelectMenuTable(file_path + files[i].Name);
                //Debug.Log( "Name:" + files[i].Name );  
            }  
            //Debug.Log( "FullName:" + files[i].FullName );  
            //Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
        }  

        //查询excel表中数据，赋值给asset文件
 
        string path= "Assets/Resources/booknames.asset";
 
        // AssetDatabase.CreateAsset(holder, path);
        // AssetDatabase.Refresh();
 
        Debug.Log("BuildAsset Success!");
    }

}
