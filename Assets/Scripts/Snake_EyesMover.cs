using UnityEngine;
using System.Collections;

public class Snake_EyesMover : MonoBehaviour
{
    private GameObject Snake;
    private Vector3 offset;

    void Start()
    {
        Snake = GameObject.FindWithTag("Snake");
    }

    void Update()
    {
        transform.rotation = Snake.transform.rotation;
    }
}
