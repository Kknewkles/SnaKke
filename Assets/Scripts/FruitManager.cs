using UnityEngine;
using System.Collections;

public class FruitManager : MonoBehaviour
{
	// Spawn a fruit on game start, spawn another on fruit being eaten.
    //private GameObject fruit;
    
    private int fruitCount = 0;
    // Snake should know how much fruits there are;
    //  and generate enough snake tails.
    public int maxFruits = 10;

    void Start()
    {
        //fruit = GameObject.FindWithTag("Fruit");
        Show();
	}
	
	void Show()
    {
        int rX, /*rY,*/rZ;
        // More hard-coded limits to remove
        rX = Random.Range(0, 19);
        //rY = Random.Range(0, 19);
        rZ = Random.Range(0, 19);

        // Check for snake coords before spawning the fruit;

        // reroll if snake occupies (rX, rY, rZ)

        // add a scale up animation for popping up?
        transform.position = new Vector3(rX, 0, rZ);
    }

    // This might die out completely once I hook everything up to the ObjectPool.
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
        //else
            //victory
    }
}
