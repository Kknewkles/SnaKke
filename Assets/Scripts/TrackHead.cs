using UnityEngine;
using System.Collections;

public class TrackHead : MonoBehaviour
{
    public GameObject Snake;

    void Start()
    {
        transform.position = Snake.transform.position;
        transform.rotation = Snake.transform.rotation;
    }

    void Update()
    {
        transform.position = Snake.transform.position;
        transform.rotation = Snake.transform.rotation;
    }

    
}
