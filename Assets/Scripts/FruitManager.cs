using UnityEngine;
using System.Collections;

public class FruitManager : MonoBehaviour
{
	// Spawn a fruit on game start, spawn another on fruit being eaten.
    //private GameObject fruit;
    
    private int fruitCount = 0;
    // Snake should know how much fruits there are and generate enough snake tails.
    public int maxFruits = 10;

    GameObject Snake;
    SnakeController SnakeScript;

    GameObject optionsManager;
    OptionsManager optionsManagerScript;

    void Start()
    {
        Snake = GameObject.FindWithTag("SnakeHead");
        SnakeScript = Snake.GetComponent<SnakeController>();
        
        optionsManager = GameObject.FindWithTag("OptionsManager");
        optionsManagerScript = optionsManager.GetComponent<OptionsManager>();

        Show();
	}
	
    // again, this is snake, so we don't need to check who's the one bumpin'.
    // But we need to refer to snake to grow it a tail.
    // Or, if I had a better idea about HOW TO EFFIN CODE...
    void OnTriggerEnter(Collider other)
    {
        // Fruit's eaten, snake's growin'
        OnEaten();
        SnakeScript.spawnTail = true;
        SnakeScript.spawnCounter = 1;
    }

	void Show()
    {
        // Is all this stuff taken from LevelManager?        
        // More hard-coded limits to remove
        int rX, /*rY,*/rZ;
        
        rX = Random.Range(0, 19);
        //rY = Random.Range(0, 19);
        rZ = Random.Range(0, 19);

        // Check for snake coords before spawning the fruit;

        // reroll if snake occupies (rX, rY, rZ)

        // add a scale up animation for popping up?
        transform.position = new Vector3(rX, 0, rZ);
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
        if (fruitCount < maxFruits)
            Show();
        else
        {
            Debug.Log("Victory screen");
            optionsManagerScript.Pause();
            // level select, start screen, exit game
        }
        
    }
}
