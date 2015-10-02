using UnityEngine;
using System.Collections;

public class FruitManager : MonoBehaviour
{
    public static FruitManager instance;
    
    // Spawn a fruit on game start, spawn another on fruit being eaten.
    //private GameObject fruit;
    
    [HideInInspector] public int fruitsEaten = 0;
    [HideInInspector] public int maximumHarvest = 0;
    // Snake should know how much fruits there are and generate enough snake tails.
    	
    // again, this is snake, so we don't need to check who's the one bumpin'.
    // But we need to refer to snake to grow it a tail.
    // Or, if I had a better idea about HOW TO EFFIN CODE...
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PopupManager.instance.score.text = "Score: " + fruitsEaten;
    }

    void OnTriggerEnter(Collider other)
    {
        OnEaten();
    }

    void Update()
    {
        if(FruitInside(transform.position))
            transform.position = FruitCoordinates();
    }

	public void Show()
    {
        // Get level sizes from LevelManager
        Vector3 fruitCoords = FruitCoordinates();
        
        // CHECK COORDS
        //while(!goodPosition) / while(within a snake or within an obstacle)
        //  fruitCoords = FruitCoordinates();
        /*
        while(!FruitOutside(fruitCoords))
            fruitCoords = FruitCoordinates();
        */

        transform.position = fruitCoords;
    }

    bool FruitInside(Vector3 xyz)
    {
        for(int i = 0; i < SnakeController.instance.Snake.Count - 1; i++)
        {
            if(xyz == SnakeController.instance.Snake[i].transform.position)
                return true;
        }

        for(int i = 0; i < XMLToLevel.instance.obstacleList.obstacles.Count; i++)
        {
            if((Mathf.Abs(xyz.x - XMLToLevel.instance.obstacleList.obstacles[i].x)) <= ((XMLToLevel.instance.obstacleList.obstacles[i].scale_x) / 2) &&
               (Mathf.Abs(xyz.y - XMLToLevel.instance.obstacleList.obstacles[i].y)) <= ((XMLToLevel.instance.obstacleList.obstacles[i].scale_y) / 2) &&
               (Mathf.Abs(xyz.z - XMLToLevel.instance.obstacleList.obstacles[i].z)) <= ((XMLToLevel.instance.obstacleList.obstacles[i].scale_z) / 2))
                return true;
        }

        return false;
    }

    Vector3 FruitCoordinates()
    {
        float rX, rY, rZ;
        Vector3 result = new Vector3(0, 0, 0);

        rX = Random.Range(0, XMLToLevel.instance.levelLength_x - 1);
        rY = Random.Range(0, XMLToLevel.instance.levelLength_y - 1);
        rZ = Random.Range(0, XMLToLevel.instance.levelLength_z - 1);

        result = new Vector3(Mathf.Round(rX), Mathf.Round(rY), Mathf.Round(rZ));

        return result;
    }

    // This might die out completely once I hook everything up to the ObjectPool.
    // void Recycle()
    void Hide()
    {
        // This is weird. WELP
        transform.position = new Vector3(0, -2, 0);
    }

    public void OnEaten()
    {
        audio.Play();
        
        fruitsEaten++;
        //PopupManager.instance.
        PopupManager.instance.score.text = "Score: " + fruitsEaten;
        Hide();
        Show();

        SnakeController.instance.spawnTail = true;
        SnakeController.instance.spawnCounter = 1;

        // on fruit eaten record current coords of the snake to later spawn a tail at it
        // ...now that I think about it it SHOULDN'T spawn tails at the right spot. Somehow it does. WELP
        SnakeController.instance.tailSpawnCoord = SnakeController.instance.Snake[SnakeController.instance.Snake.Count - 1].transform.position -
                                                  SnakeController.instance.forV;
    }
}