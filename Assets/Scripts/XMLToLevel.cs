using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class ArrayOfWallData
{
    [XmlElement("WallData")]    // this is freaking paramount.
    public List<WallData> walls = new List<WallData>();
}

public class ArrayOfObstacleData
{
    [XmlElement("ObstacleData")]
    public List<ObstacleData> obstacles = new List<ObstacleData>();
}

public class XMLToLevel : MonoBehaviour
{
    public static XMLToLevel instance;
    
    ArrayOfWallData wallList = new ArrayOfWallData();
    WallData wall = new WallData();
    public GameObject wallPrefab;
    public GameObject wallsEmptyObject;

    ArrayOfObstacleData obstacleList = new ArrayOfObstacleData();
    ObstacleData obstacle = new ObstacleData();
    public GameObject obstaclePrefab;
    public GameObject obstaclesEmptyObject;
        
    /*[HideInInspector]*/ public int levelNumber;
    /*[HideInInspector]*/ public float levelLength_x = 0;
    /*[HideInInspector]*/ public float levelLength_y = 0;
    /*[HideInInspector]*/ public float levelLength_z = 0;

    private static readonly string pathFolderName = Application.dataPath + "/Resources/";
    private static readonly string pathObstaclesFile = "/Obstacles";
    private static readonly string pathWallsFile = "/Walls";
    private static readonly string fileExtention = ".xml";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        wallsEmptyObject = new GameObject("Walls");
        wallsEmptyObject.tag = "Walls";
        wallsEmptyObject.transform.parent = transform;
        wallsEmptyObject.transform.position = new Vector3(-0.5f, -0.5f, -0.5f);
 
        obstaclesEmptyObject = new GameObject("Obstacles");
        obstaclesEmptyObject.tag = "Obstacles";
        obstaclesEmptyObject.transform.parent = transform;
    }

    public void LoadLevel(int number)
    {
        XMLToLevel.instance.levelNumber = number;

        XMLToLevel.instance.ReadWallsFromXML();
        XMLToLevel.instance.ReadObstaclesFromXML();
    }

    // deserialize: get a bunch of object info
    public void ReadWallsFromXML()
    {
        XmlSerializer deserializer = new XmlSerializer(typeof(ArrayOfWallData));
        TextReader xmlReader = new StreamReader(pathFolderName + pathWallsFile + levelNumber + fileExtention);
        wallList = (ArrayOfWallData)(deserializer.Deserialize(xmlReader));
        xmlReader.Close();

        for(int i = 0; i < wallList.walls.Count; i++)
        {
            float x = (Mathf.Round(wallList.walls[i].x));
            float y = wallList.walls[i].y;
            float z = wallList.walls[i].z;
            Vector3 position = new Vector3(x, y, z);

            float rot_x = wallList.walls[i].rot_x;
            float rot_y = wallList.walls[i].rot_y;
            float rot_z = wallList.walls[i].rot_z;
            Quaternion rotation = Quaternion.Euler(rot_x, rot_y, rot_z);

            float scale_x = wallList.walls[i].scale_x;
            float scale_y = wallList.walls[i].scale_y;
            float scale_z = wallList.walls[i].scale_z;
            Vector3 scale = new Vector3(scale_x, scale_y, scale_z);

            GameObject wall = Instantiate(wallPrefab, position, rotation) as GameObject;


            wall.transform.localScale = scale;

            // for planes Y is always defunct.
            wall.renderer.material.SetTextureScale("_MainTex", new Vector2(10 * scale.x, 10 * scale.z));
            wall.renderer.material.SetTextureScale("_BumpMap", new Vector2(10 * scale.x, 10 * scale.z));

            // parent to empty
            wall.transform.parent = wallsEmptyObject.transform;     // THIS DUNN WORK

            // shift
            wall.transform.localPosition = position;
        }
        
        levelLength_x = (wallList.walls[0].scale_x * 10);
        levelLength_z = (wallList.walls[0].scale_z * 10);
        levelLength_y = (wallList.walls[1].scale_x * 10);

    }

    public void ReadObstaclesFromXML()
    {
        XmlSerializer deserializer = new XmlSerializer(typeof(ArrayOfObstacleData));
        TextReader xmlReader = new StreamReader(pathFolderName + pathObstaclesFile + levelNumber + fileExtention);
        obstacleList = (ArrayOfObstacleData)(deserializer.Deserialize(xmlReader));
        xmlReader.Close();

        for(int j = 0; j < obstacleList.obstacles.Count; j++)
        {
            float x = obstacleList.obstacles[j].x;
            float y = obstacleList.obstacles[j].y;
            float z = obstacleList.obstacles[j].z;
            Vector3 position = new Vector3(x, y, z);

            float scale_x = obstacleList.obstacles[j].scale_x;
            float scale_y = obstacleList.obstacles[j].scale_y;
            float scale_z = obstacleList.obstacles[j].scale_z;
            Vector3 scale = new Vector3(scale_x, scale_y, scale_z);

            // Pull this out to a separate function
            //  Instantiate or ObjectPool.Spawn
            // ---
            GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity) as GameObject;
            // parent to empty
            obstacle.transform.parent = obstaclesEmptyObject.transform;

            obstacle.transform.localScale = scale;

            Vector2[] DoMeUVs =
            {
                /* my try
                // x, y
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 1), 
                
                // x, z
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1),
                
                // y, z
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1)
                */

                /* Zody to the rescue
                new Vector2(x, 0), new Vector2(0, 0), new Vector2(x, y), new Vector2(0, y),//
                new Vector2(x, 0), new Vector2(0, 0), new Vector2(x, 0), new Vector2(0, 0), 
                
                // x, z
                new Vector2(x, z), new Vector2(0, z), new Vector2(x, y), new Vector2(0, y),
                new Vector2(x, 0), new Vector2(0, z), new Vector2(0, 0), new Vector2(x, z),//
                
                // z, y
                new Vector2(z, 0), new Vector2(0, y), new Vector2(0, 0), new Vector2(z, y),//
                new Vector2(0, 0), new Vector2(z, y), new Vector2(z, 0), new Vector2(0, y)//
                */
                // x, y
                new Vector2(scale_x, 0), new Vector2(0, 0), new Vector2(scale_x, scale_y),  new Vector2(0, scale_y),//
                new Vector2(scale_x, 0), new Vector2(0, 0), new Vector2(scale_x, 0),        new Vector2(0, 0),

                // x, z
                new Vector2(scale_x, scale_z),  new Vector2(0, scale_z), new Vector2(scale_x, scale_y), new Vector2(0, scale_y),
                new Vector2(scale_x, 0),        new Vector2(0, scale_z), new Vector2(0, 0),             new Vector2(scale_x, scale_z),//
                
                // z, y
                new Vector2(scale_z, 0),    new Vector2(0, scale_y),        new Vector2(0, 0),          new Vector2(scale_z, scale_y),//
                new Vector2(0, 0),          new Vector2(scale_z, scale_y),  new Vector2(scale_z, 0),    new Vector2(0, scale_y)//
            };

            Mesh mesh = obstacle.GetComponent<MeshFilter>().mesh;
            mesh.uv = DoMeUVs;
            // ---
        }
    }

}