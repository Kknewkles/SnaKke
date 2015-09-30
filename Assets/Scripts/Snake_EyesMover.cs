using UnityEngine;
using System.Collections;

public class Snake_EyesMover : MonoBehaviour
{
    private GameObject SnakeHead;
    
    void Update()
    {
        transform.position = SnakeController.instance.SnakeHead.transform.position;
        transform.rotation = SnakeController.instance.SnakeHead.transform.rotation;
    }
}
