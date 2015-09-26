using UnityEngine;
using System.Collections;

public class SnakeKiller : MonoBehaviour
{
    GameObject Snake;
    SnakeController SnakeScript;

    // not sure about this. But how else do you stop the game flow?
    void Start()
    {
        Snake = GameObject.FindWithTag("SnakeHead");
        SnakeScript = Snake.GetComponent<SnakeController>();
    }
    
    // since this will be snake bumping into stuff, we interact with her.
    void OnTriggerEnter(Collider other)
    {
        SnakeScript.alive = false;

        // death event
        // which should be the "death menu" popping up
        Debug.Log("BA-BOOOOM!");
    }
}
