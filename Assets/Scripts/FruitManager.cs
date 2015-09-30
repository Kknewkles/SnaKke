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

    GameObject popupManager;
    PopupManager popupManagerScript;

    void Start()
    {
        
        Snake = GameObject.FindWithTag("SnakeHead");
        SnakeScript = Snake.GetComponent<SnakeController>();
        
        popupManager = GameObject.FindWithTag("PopupManager");
        popupManagerScript = popupManager.GetComponent<PopupManager>();

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
        // Get level sizes from LevelManager
        int rX, rY, rZ;
        
        rX = Random.Range(0, 10);
        rY = Random.Range(0, 10);
        rZ = Random.Range(0, 10);

        // Check for snake coords before spawning the fruit;

        // And how the hell do you check obstacles?...
        // have a list of obstacles, I guess
        // Ooh! Read from XML! Or list.

        // reroll if snake occupies (rX, rY, rZ)

        // add a scale up animation for popping up?
        transform.position = new Vector3(rX, rY, rZ);
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
            // call into popupmanager's LevelComplete function?
            popupManagerScript.Pause();
            // level select, start screen, exit game
        }
        
    }
}
