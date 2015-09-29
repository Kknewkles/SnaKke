using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class ObstacleData
{
    [XmlAttribute] public float x;
    [XmlAttribute] public float y;
    [XmlAttribute] public float z;
    [XmlAttribute] public float scale_x;
    [XmlAttribute] public float scale_y;
    [XmlAttribute] public float scale_z;
}

public class WallData
{
    [XmlAttribute] public float x;
    [XmlAttribute] public float y;
    [XmlAttribute] public float z;
    [XmlAttribute] public float rot_x;
    [XmlAttribute] public float rot_y;
    [XmlAttribute] public float rot_z;
    [XmlAttribute] public float scale_x;
    [XmlAttribute] public float scale_y;
    [XmlAttribute] public float scale_z;
}

public class CubeEditor : EditorWindow
{
    [MenuItem("Window/XML Button")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CubeEditor));
    }

    void OnGUI()
    {
        if(GUILayout.Button("Write XML")) ;
    }
}

public class LevelToXML : MonoBehaviour
{
    GameObject obstaclesParent;
    Transform[] obstaclesTransforms;
    ObstacleData obstacle;
    List<ObstacleData> obstacles = new List<ObstacleData>();

    GameObject wallsParent;
    Transform[] wallsTransforms;
    WallData wall;
    List<WallData> walls = new List<WallData>();

    private static readonly string pathFolderName = Application.dataPath + "/Resources/";
    private static readonly string pathObstaclesFile = "/Obstacles.xml";
    private static readonly string pathWallsFile = "/Walls.xml";

    int obstaclesCounter = -1;
    int wallsCounter = -1;
        
    void Awake()
    {
        obstaclesParent = GameObject.FindWithTag("Obstacles");
        obstaclesCounter = -1;
        //FindObstacles();

        wallsParent = GameObject.FindWithTag("Walls");
        wallsCounter = -1;
        //FindWalls();
    }

    public void FindObstacles()
    {
        obstaclesTransforms = obstaclesParent.GetComponentsInChildren<Transform>();
        foreach(Transform transform in obstaclesTransforms)
        {
            // rounding here isn't really needed, but if we don't want to spend time aligning everything, it's kind of helpful.
            if(obstaclesCounter > -1)
            {
                obstacle = new ObstacleData();
                obstacle.x       = (float)(Mathf.Round(transform.position.x));
                obstacle.y       = (float)(Mathf.Round(transform.position.y));
                obstacle.z       = (float)(Mathf.Round(transform.position.z));
                obstacle.scale_x = (float)(Mathf.Round(transform.localScale.x));
                obstacle.scale_y = (float)(Mathf.Round(transform.localScale.y));
                obstacle.scale_z = (float)(Mathf.Round(transform.localScale.z));

                obstacles.Add(obstacle);
            }
            obstaclesCounter++;
        }
        WriteObstaclesToXML(obstacles);
    }

    public void FindWalls()
    {
        wallsTransforms = wallsParent.GetComponentsInChildren<Transform>();
        Vector3 originShift = new Vector3(0, 0, 0);
        foreach(Transform transform in wallsTransforms)
        {
            // capture origin shift
            if(wallsCounter == -1)
            {
                originShift = transform.position;
            }
            
            // clarify walls coordinates: with origin shift, they are exact.
            // and round them up. We need precision. Even if by slightly barbaric means.
            if(wallsCounter > -1)
            {
                wall = new WallData();
                wall.x       = (float)(Mathf.Round(transform.position.x - originShift.x));
                wall.y       = (float)(Mathf.Round(transform.position.y - originShift.y));
                wall.z       = (float)(Mathf.Round(transform.position.z - originShift.z));
                wall.rot_x   = (float)(Mathf.Round(transform.rotation.eulerAngles.x));
                wall.rot_y   = (float)(Mathf.Round(transform.rotation.eulerAngles.y));
                wall.rot_z   = (float)(Mathf.Round(transform.rotation.eulerAngles.z));
                wall.scale_x = (float)(Mathf.Round(transform.localScale.x));
                wall.scale_y = (float)(Mathf.Round(transform.localScale.y));
                wall.scale_z = (float)(Mathf.Round(transform.localScale.z));

                walls.Add(wall);
            }
            wallsCounter++;
        }
        WriteWallsToXML(walls);
    }


    public void WriteObstaclesToXML(List<ObstacleData> list)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<ObstacleData>));
        XmlSerializerNamespaces nonamespaces = new XmlSerializerNamespaces();
        nonamespaces.Add("", "");
        
        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
        XmlWriter xmlWriter = XmlWriter.Create(pathFolderName + pathObstaclesFile, xmlWriterSettings);
        serializer.Serialize(xmlWriter, list, nonamespaces);
        xmlWriter.Close();
    }

    public void WriteWallsToXML(List<WallData> list)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<WallData>));
        XmlSerializerNamespaces nonamespaces = new XmlSerializerNamespaces();
        nonamespaces.Add("", "");

        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
        XmlWriter xmlWriter = XmlWriter.Create(pathFolderName + pathWallsFile, xmlWriterSettings);
        serializer.Serialize(xmlWriter, list, nonamespaces);
        xmlWriter.Close();
    }
}