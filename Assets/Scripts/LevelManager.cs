using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class XMLEditor : EditorWindow
{
    LevelManager levelManager;
    
    [MenuItem("Window/XML button")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(XMLEditor));
    }

    void OnGUI()
    {
        if(GUILayout.Button("Write XML"))
        {
            levelManager.ObtainInfo();
        }
    }
}

public class LevelManager : MonoBehaviour
{
    static int maxWalls = 6;        // there will only ever be 6 walls.
    static int maxObstacles = 15;   // this should be enough.
        
    private static readonly string folder = Application.dataPath + "/Resources/Items/XML/";
    private static readonly string obstaclesFileName = "obstacles.xml";
    private static readonly string wallsFileName = "walls.xml";
  
    public void ObtainInfo()
    {
        Debug.Log("CONNECTION!!");
    }
}

