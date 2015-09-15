using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    /*       
    x - left-right
    right	Vector3(1, 0, 0).
    left	Vector3(-1, 0, 0).
    
    y - up-down
    up	    Vector3(0, 1, 0).
    down	Vector3(0, -1, 0).

    z - back-forward
    forward	Vector3(0, 0, 1).
    back	Vector3(0, 0, -1).

    zero	Vector3(0, 0, 0).
    one	    Vector3(1, 1, 1).
    */

    private bool alive = true;

    private GameObject SnakeHead;
    public GameObject SnakeTail;
    List<GameObject> Snake = new List<GameObject>();

    private FruitController Fruit;

    public float speed = 0.5f;
    
    private Quaternion LeAngle = Quaternion.Euler(0, 0, 0);
    private int LeAngleX = 0;
    private int LeAngleY = 0;
    private int LeAngleZ = 0;


    private int orientIndex = 0;

    private bool KeyDown = false;
    
    void Start()
    {
        // get fruit controller
        GameObject fruitControllerObject = GameObject.FindWithTag("Fruit");
        Fruit = fruitControllerObject.GetComponent<FruitController>();
        
        // track the head
        SnakeHead = GameObject.FindWithTag("SnakeHead");
        Snake.Add(SnakeHead);

        SnakeHead.transform.position = new Vector3(5,0,5);
        SnakeHead.transform.rotation = LeAngle;

        orientIndex = 0;

        StartCoroutine(SnakeControl());
    }
    
    
    IEnumerator SnakeControl()
    {
        while(alive)
        {
            yield return new WaitForSeconds(speed);
            
            // assign the value here
            RotateHead();
            Creep();
        }
    }
    
    void RotateHead()
    {
        /*Quaternion Angle;
        Angle = Quaternion.Euler(LeAngleX, LeAngleY, LeAngleZ);
        SnakeHead.transform.rotation = Angle;*/
    }

    void Creep()
    {
        Vector3 coords, prevCoords;

        prevCoords = Snake[0].transform.position;
        Snake[0].transform.position += Snake[0].transform.forward;
        
        for (int i = 1; i < Snake.Count; i++)
        {
            coords = prevCoords;
            prevCoords = Snake[i].transform.position;
            Snake[i].transform.position = coords;
        }
    }

    void Update()
    {
        CheckInput();
    }
    
    int CorrectAngle(float angle)
    {
        if (Math.Abs(angle) < 5) angle = 0;
        if (Math.Abs(angle - 90) < 5) angle = 90;
        if (Math.Abs(angle - 180) < 5) angle = 180;
        if (Math.Abs(angle - 270) < 5) angle = 270;
        if (Math.Abs(angle - 360) < 5) angle = 0;

        return (int)angle;
    }
    
    void CheckInput()
    {
        float Key = 0;

        Key = Input.GetAxis("Fire1");

        if (Key != 0)
        {
            float angleY = CorrectAngle(LeAngle.eulerAngles.y);

            if (Key > 0)
                angleY += 90;
            if (Key < 0)
                angleY -= 90;

            LeAngle = Quaternion.Euler(0, angleY, 0);
            Debug.Log("LeAngle " + LeAngle.eulerAngles);

            SnakeHead.transform.rotation = LeAngle;
        }
    }

    // ---

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fruit"))
        {
            Fruit.OnEaten();
            AddTail();
        }
        //if (other.CompareTag("Snake")) ;
            // Destroy
            // 
        //if (other.CompareTag("Obstacle")) ;
            // Destroy
    }

    public void AddTail()
    {
        Vector3 move = Snake[0].transform.forward;
        Vector3 coords = Snake[Snake.Count - 1].transform.position - move;

        // if you take one of existing/from objectpool, how do you distinguish and refer between them?
        SnakeTail = (GameObject)(Instantiate(SnakeTail, coords, Quaternion.identity));
        Snake.Add(SnakeTail);
    }
}
