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

public class LevelToXML : MonoBehaviour
{
    public static LevelToXML instance;
    
    GameObject obstaclesParent;
    Transform[] obstaclesTransforms;
    ObstacleData obstacle;
    List<ObstacleData> obstacles = new List<ObstacleData>();

    GameObject wallsParent;
    Transform[] wallsTransforms;
    WallData wall;
    List<WallData> walls = new List<WallData>();

    [HideInInspector] public int levelNumber;

    private static readonly string pathFolderName = Application.dataPath + "/Resources/";
    private static readonly string pathObstaclesFile = "/Obstacles";
    private static readonly string pathWallsFile = "/Walls";
    private static readonly string fileExtention = ".xml";

    int obstaclesCounter = -1;
    int wallsCounter = -1;
        
    void Awake()
    {
        instance = this;
        
    }

    public void FindObstacles()
    {
        obstaclesParent = GameObject.FindWithTag("Obstacles");
        obstaclesCounter = -1;
        
        obstaclesTransforms = obstaclesParent.GetComponentsInChildren<Transform>();
        foreach(Transform transform in obstaclesTransforms)
        {
            if(obstaclesCounter > -1)
            {
                obstacle = new ObstacleData();
                obstacle.x       = transform.position.x;
                obstacle.y       = transform.position.y;
                obstacle.z       = transform.position.z;
                obstacle.scale_x = transform.localScale.x;
                obstacle.scale_y = transform.localScale.y;
                obstacle.scale_z = transform.localScale.z;

                obstacles.Add(obstacle);
            }
            obstaclesCounter++;
        }
        WriteObstaclesToXML(obstacles);
    }

    public void FindWalls()
    {
        wallsParent = GameObject.FindWithTag("Walls");
        wallsCounter = -1;
        
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
        XmlWriter xmlWriter = XmlWriter.Create(pathFolderName + pathObstaclesFile + levelNumber + fileExtention, xmlWriterSettings);
        serializer.Serialize(xmlWriter, list, nonamespaces);
        xmlWriter.Close();
    }

    public void WriteWallsToXML(List<WallData> list)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<WallData>));
        XmlSerializerNamespaces nonamespaces = new XmlSerializerNamespaces();
        nonamespaces.Add("", "");

        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
        XmlWriter xmlWriter = XmlWriter.Create(pathFolderName + pathWallsFile + levelNumber + fileExtention, xmlWriterSettings);
        serializer.Serialize(xmlWriter, list, nonamespaces);
        xmlWriter.Close();
    }
}