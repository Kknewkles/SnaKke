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
    ArrayOfWallData wallList = new ArrayOfWallData();
    WallData wall = new WallData();
    public GameObject wallPrefab;
    GameObject wallsEmptyObject;

    ArrayOfObstacleData obstacleList = new ArrayOfObstacleData();
    ObstacleData obstacle = new ObstacleData();
    public GameObject obstaclePrefab;
    GameObject obstaclesEmptyObject;

    private static readonly string pathFolderName = Application.dataPath + "/Resources/";
    private static readonly string pathObstaclesFile = "/Obstacles.xml";
    private static readonly string pathWallsFile = "/Walls.xml";

    void Start()
    {
        wallsEmptyObject = new GameObject("Walls");
        wallsEmptyObject.transform.parent = transform;
        wallsEmptyObject.transform.position = new Vector3(-0.5f, -0.5f, -0.5f);
 
        obstaclesEmptyObject = new GameObject("Obstacles");
        obstaclesEmptyObject.transform.parent = transform;

        ReadWallsFromXML();

        ReadObstaclesFromXML();
    }

    // deserialize: get a bunch of object info
    public void ReadWallsFromXML()
    {
        XmlSerializer deserializer = new XmlSerializer(typeof(ArrayOfWallData));
        TextReader xmlReader = new StreamReader(pathFolderName + pathWallsFile);
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
    }

    public void ReadObstaclesFromXML()
    {
        XmlSerializer deserializer = new XmlSerializer(typeof(ArrayOfObstacleData));
        TextReader xmlReader = new StreamReader(pathFolderName + pathObstaclesFile);
        obstacleList = (ArrayOfObstacleData)(deserializer.Deserialize(xmlReader));
        xmlReader.Close();

        for(int i = 0; i < obstacleList.obstacles.Count; i++)
        {
            float x = obstacleList.obstacles[i].x;
            float y = obstacleList.obstacles[i].y;
            float z = obstacleList.obstacles[i].z;
            Vector3 position = new Vector3(x, y, z);

            float scale_x = obstacleList.obstacles[i].scale_x;
            float scale_y = obstacleList.obstacles[i].scale_y;
            float scale_z = obstacleList.obstacles[i].scale_z;
            Vector3 scale = new Vector3(scale_x, scale_y, scale_z);

            GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity) as GameObject;
            // parent to empty
            obstacle.transform.parent = obstaclesEmptyObject.transform;

            obstacle.transform.localScale = scale;

            // instead of this we need uv maps.
            //obstacle.renderer.material.SetTextureScale("_MainTex", new Vector2(scale.x, scale.y));
            //obstacle.renderer.material.SetTextureScale("_BumpMap", new Vector2(scale.x, scale.y));

            //obstacle.renderer.material.SetTextureScale("_MainTex", new Vector2(1, 1));
            //obstacle.renderer.material.SetTextureScale("_BumpMap", new Vector2(1, 1));

            // uv
            // WOW. Printed out arrays of vertexes and uvs, then filled them in that order. WORKED. Almost. One polygon is mysteriously broken.
            // The whole thing is fucking convoluted.
            /*
            Vector2[] DoMeUVs =
            {
                // x, y
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 1), 
                
                // x, z
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1),
                
                // y, z
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1)
            };

            Mesh mesh = obstacle.GetComponent<MeshFilter>().mesh;
            mesh.uv = DoMeUVs;
            */
        }
    }

    /*
     * z=1, back 
        0   (1, 0,  )  (0, 0)       0   (1, 0,  ) (0, 0)
        1   (0, 0,  )  (1, 0)       1   (0, 0,  ) (1, 0)
        2   (1, 1,  )  (0, 1)       2   (1, 1,  ) (0, 1)
        3   (0, 1,  )  (1, 1)       3   (0, 1,  ) (1, 1)
       z=0, front                    
        4   (1, 1,  )  (0, 1)       4   (1, 1,  ) (0, 1) 
        5   (0, 1,  )  (1, 1)       5   (0, 1,  ) (1, 1) 
        6   (1, 0,  )  (0, 1)       6   (1, 0,  ) (0, 1) 
        7   (0, 0,  )  (1, 1)       7   (0, 0,  ) (1, 1) 
       y=1, top                      
        8   (1,  , 1)  (0, 0)       8   (1,  , 1) (0, 0) 
        9   (0,  , 1)  (1, 0)       9   (0,  , 1) (1, 0) 
        10  (1,  , 0)  (0, 0)       10  (1,  , 0) (0, 0) 
        11  (0,  , 0)  (1, 0)       11  (0,  , 0) (1, 0) 
       y=0, bottom                   
        12  (1,  , 0)  (0, 0)       12  (1,  , 0) (0, 0) 
        13  (0,  , 1)  (1, 1)       13  (0,  , 1) (1, 1) 
        14  (0,  , 0)  (1, 0)       14  (0,  , 0) (1, 0) 
        15  (1,  , 1)  (0, 1)       15  (1,  , 1) (0, 1) 
       x=0, left                     
        16  ( , 0, 1)  (0, 0)       16  ( , 0, 1) (0, 0) 
        17  ( , 1, 0)  (1, 1)       17  ( , 1, 0) (1, 1) 
        18  ( , 0, 0)  (1, 0)       18  ( , 0, 0) (1, 0) 
        19  ( , 1, 1)  (0, 1)       19  ( , 1, 1) (0, 1) 
       x=1, right       
        20  ( , 0, 0)  (0, 0)       20  ( , 0, 0) (0, 0)
        21  ( , 1, 1)  (1, 1)       21  ( , 1, 1) (1, 1)
        22  ( , 0, 1)  (1, 0)       22  ( , 0, 1) (1, 0)
        23  ( , 1, 0)  (0, 1)       23  ( , 1, 0) (0, 1)
    */

    /* DEFAULT  / works with 0 and 1
                new Vector2(0, 0), new Vector2(4, 0), new Vector2(0, 7), new Vector2(4, 7),
                new Vector2(0, 7), new Vector2(4, 7), new Vector2(0, 7), new Vector2(4, 7), 
                // y = c
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1),
                // x = c
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1)
                */
}