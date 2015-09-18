using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    private bool alive = true;

    private GameObject SnakeHead;
    public GameObject SnakeTail;
    List<GameObject> Snake = new List<GameObject>();

    private FruitController Fruit;

    // delay
    public float speed = 0.5f;
    // rotate wait
    float waitSmoothRot = 3f;
    // crawl wait
    float waitCrawl = 3f;
    
    // angle to construct and assign to the snake's head
    /*
    private Quaternion LeAngle = Quaternion.Euler(0, 0, 0);
    private int LeAngleX = 0;
    private int LeAngleY = 0;
    private int LeAngleZ = 0;
    */


    Quaternion rotateTo = Quaternion.Euler(0, 0, 0);

    float lerpSpeed = 1;
    float accuracy = 0.1f;

    float angleInc;
    float angle;

    private float angleY;
    private float Key;


    void Start()
    {
        // get fruit controller
        GameObject fruitControllerObject = GameObject.FindWithTag("Fruit");
        Fruit = fruitControllerObject.GetComponent<FruitController>();
        
        // track the head
        SnakeHead = GameObject.FindWithTag("SnakeHead");
        Snake.Add(SnakeHead);

        SnakeHead.transform.position = new Vector3(5,0,5);
        //SnakeHead.transform.rotation = LeAngle;

        StartCoroutine(SnakeControl());

        Key = 0;
        angleY = 0;

        angleInc = 0;
    }
    
    
    IEnumerator SnakeControl()
    {
        while(alive)
        {
            yield return new WaitForSeconds(speed);
                        
            //RotateHead();   // deliver angle change for
            LerpRotateHead();
            // SmoothRotate();
            // wait for smooth rotation
            
            yield return new WaitForSeconds(waitSmoothRot);
            //angleInc = 0;
            //angleY = 0;
            // yield return new WaitForSeconds(blah);
            Crawl();
            // SmoothCrawl();
        }
    }
    

    // THE PROBLEM HAILS FROM HERE.
    void RotateHead()
    {
        Quaternion angle = Quaternion.Euler(0, CorrectAngle(Snake[0].transform.rotation.eulerAngles.y + angleY), 0);
        Snake[0].transform.rotation = angle;
        angleY = 0;
        
        //angle = Snake[0].transform.rotation.y;
        //Quaternion angle = Quaternion.Euler(0, CorrectAngle(Snake[0].transform.rotation.eulerAngles.y + angleY), 0);
    }

    void LerpRotateHead()
    {
        rotateTo = Quaternion.Euler(Snake[0].transform.rotation.eulerAngles.x,
                                    Snake[0].transform.rotation.eulerAngles.y + angleY,
                                    Snake[0].transform.rotation.eulerAngles.z);
    }

    void Crawl()
    {
        Vector3 coords, prevCoords;

        prevCoords = Snake[0].transform.position;
        Vector3 forV = Snake[0].transform.forward;
        Snake[0].transform.position += forV;

        

        for (int i = 1; i < Snake.Count; i++)
        {
            // ADD LERP MOVEMENT
            coords = prevCoords;
            prevCoords = Snake[i].transform.position;
            Snake[i].transform.position = coords;
        }
    }
    
    void FixedUpdate()
    {
        CheckInput();
        
        SmoothRotate(rotateTo);
        
        //DebugInfos();
    }
    
    void DebugInfos()
    {
        //Debug.Log("angleY:" + angleY + " rotateTo:" + rotateTo.eulerAngles);
    }

    void SmoothRotate(Quaternion finish)
    {
        Quaternion from = Quaternion.Euler(0, Snake[0].transform.rotation.eulerAngles.y, 0);
        Quaternion to = Quaternion.Euler(0, Snake[0].transform.rotation.eulerAngles.y + angleInc, 0);

        if (Mathf.Abs(Snake[0].transform.rotation.eulerAngles.y - finish.eulerAngles.y) > accuracy)
            Snake[0].transform.rotation = Quaternion.Lerp(from, to, lerpSpeed);

        if ((Mathf.Abs(Snake[0].transform.rotation.eulerAngles.y - finish.eulerAngles.y) <= accuracy) &&
            (Snake[0].transform.rotation != finish))
        {
            Snake[0].transform.rotation = finish;
            angleY = 0;
        }
    }
    
    // Maybe I didn't account for the angle needing to be 360 and not 0 sometimes.
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
        if ((Input.GetAxis("Fire1") != 0) && (Key == 0))
            Key = Input.GetAxis("Fire1");

        if((Key != 0) && (Input.GetAxis("Fire1") == 0))
        {
            if (Key > 0)
            {
                angleY = 90;
                angleInc = 1;
            }
            else if (Key < 0)
            { 
                angleY = -90;
                angleInc = -1;
            }
            else if (Key == 0)
            { 
                angleY = 0;
            }

            Key = 0;
        }
    }


    // to do: move collider processing to walls, fruits, tails.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fruit"))
        {
            Fruit.OnEaten();
            AddTail();
        }
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
