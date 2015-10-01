using UnityEngine;
using System.Collections;

public class Fruit_OnEaten : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
            // play sound
        FruitManager.instance.fruitsEaten++;

        SnakeController.instance.spawnTail = true;
        SnakeController.instance.spawnCounter = 1;

        FruitManager.instance.fruit.Recycle();
    }
    
  
}
