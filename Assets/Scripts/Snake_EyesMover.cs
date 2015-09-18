using UnityEngine;
using System.Collections;

public class Snake_EyesMover : MonoBehaviour
{
    private GameObject SnakeHead;
    
    void Start()
    {
        SnakeHead = GameObject.FindWithTag("SnakeHead");
        transform.parent = SnakeHead.transform;
    }
}
