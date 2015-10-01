using UnityEngine;
using System.Collections;

public class FruitManager : MonoBehaviour
{
    public static FruitManager instance;
    
    // Spawn a fruit on game start, spawn another on fruit being eaten.
    public GameObject fruit;
    
    [HideInInspector] public int fruitsEaten = 0;
    // Snake should know how much fruits there are and generate enough snake tails.
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Show();
	}
	
    // again, this is snake, so we don't need to check who's the one bumpin'.
    // But we need to refer to snake to grow it a tail.
    // Or, if I had a better idea about HOW TO EFFIN CODE...
    

	void Show()
    {
        // Get level sizes from LevelManager
        Vector3 fruitCoords = new Vector3(0, 0, 0);
        
        // pull out to separate function
        

        // reroll if snake occupies (rX, rY, rZ)

        // reroll is obstacle occupies

        // add a scale up animation for popping up?
        /*while(!ok)
            fruitCoords = FruitCoordinates();
        transform.positon = fruitCoords;*/

        // Spawn should be here?
    }

    Vector3 FruitCoordinates()
    {
        int rX, rY, rZ;
        
        rX = Random.Range(0, XMLToLevel.instance.levelLength_x);
        rY = Random.Range(0, XMLToLevel.instance.levelLength_y);
        rZ = Random.Range(0, XMLToLevel.instance.levelLength_z);

        return new Vector3(rX, rY, rZ);
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
        
        Hide();
        Show();
    }
}