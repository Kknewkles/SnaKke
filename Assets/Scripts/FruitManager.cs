using UnityEngine;
using System.Collections;

public class FruitManager : MonoBehaviour
{
    public static FruitManager instance;
    
    // Spawn a fruit on game start, spawn another on fruit being eaten.
    //private GameObject fruit;
    
    private int fruitCount = 0;
    // Snake should know how much fruits there are and generate enough snake tails.
    	
    // again, this is snake, so we don't need to check who's the one bumpin'.
    // But we need to refer to snake to grow it a tail.
    // Or, if I had a better idea about HOW TO EFFIN CODE...
    
    void Awake()
    {
        instance = this;
    }

    void OnTriggerEnter(Collider other)
    {
        // Fruit's eaten, snake's growin'
        OnEaten();

        SnakeController.instance.spawnTail = true;
        SnakeController.instance.spawnCounter = 1;
    }

	public void Show()
    {
        // Get level sizes from LevelManager
        Vector3 fruitCoords = FruitCoordinates();
                
        // CHECK COORDS
        //while(!goodPosition)
        //  fruitCoords = FruitCoordinates();

        // add a scale up animation for popping up?
        transform.position = fruitCoords;
    }

    Vector3 FruitCoordinates()
    {
        float rX, rY, rZ;

        rX = Random.Range(0, XMLToLevel.instance.levelLength_x - 1);
        rY = Random.Range(0, XMLToLevel.instance.levelLength_y - 1);
        rZ = Random.Range(0, XMLToLevel.instance.levelLength_z - 1);

        return new Vector3(Mathf.Round(rX), Mathf.Round(rY), Mathf.Round(rZ));
    }

    // This might die out completely once I hook everything up to the ObjectPool.
    // void Recycle()
    void Hide()
    {
        // Add scale to zero when you switch to smooth motion, as an animation of being eaten.
        transform.position = new Vector3(0, -2, 0);
    }

    public void OnEaten()
    {
        fruitCount++;
        Hide();
        Show();
    }
}