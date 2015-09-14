using UnityEngine;
using System.Collections;

public class Snake_EyesMover : MonoBehaviour
{
    private GameObject SnakeHead;
    private Vector3 offset;

    void Start()
    {
        // Switch to tracking List[0]
        SnakeHead = GameObject.FindWithTag("SnakeHead");
    }

    void Update()
    {
        transform.rotation = SnakeHead.transform.rotation;
        transform.position = SnakeHead.transform.position;
    }
}
