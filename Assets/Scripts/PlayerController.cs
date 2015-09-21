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
    public float turnDelay = 0.5f;
    // rotate wait
    public float waitSmoothRot = 2f;
    
    // angle to construct and assign to the snake's head
    /*
    private Quaternion LeAngle = Quaternion.Euler(0, 0, 0);
    private int LeAngleX = 0;
    private int LeAngleY = 0;
    private int LeAngleZ = 0;
    */

    Vector3[] moveTo = new Vector3[10];

    Quaternion rotateTo = Quaternion.Euler(0, 0, 0);

    float lerpSpeed = 1;
    float rotAccuracy = 1f;
    float movAccuracy = 0.01f;

    float angleInc;

    private float angleX;
    private float angleY;
    private float angleZ;
    private float Key;

    Vector3 forV;
    bool blockInput = false;

    bool spawnTail = false;
    int spawnCounter = 0;

    float time;

    void Start()
    {
        // get fruit controller
        GameObject fruitControllerObject = GameObject.FindWithTag("Fruit");
        Fruit = fruitControllerObject.GetComponent<FruitController>();
        
        // track the head
        SnakeHead = GameObject.FindWithTag("SnakeHead");
        Snake.Add(SnakeHead);

        SnakeHead.transform.position = new Vector3(5,0,5);
        moveTo[0] = Snake[0].transform.position + Snake[0].transform.forward;
        //SnakeHead.transform.rotation = LeAngle;

        StartCoroutine(SnakeControl());

        Key = 0;
        
        angleX = 0;
        angleY = 0;
        angleZ = 0;

        angleInc = 0;
        Vector3 forV = Snake[0].transform.forward;

        time = Time.time;
    }
        
    IEnumerator SnakeControl()
    {
        while(alive)
        {
            // delay between turns
            yield return new WaitForSeconds(turnDelay);

            RotateHead();
            Crawl();

            // wait for smooth rotation            
            yield return new WaitForSeconds(waitSmoothRot);
            

            // crap to do in the turn's end
            

            if(spawnTail && (spawnCounter-- == 0))
            {
                AddTail();
                spawnTail = false;
            }

            /*if (AngleWithinMargin(angleY))
                blockInput = false;*/
        }
    }

    void FixedUpdate()
    {
        CheckInput();
        SmoothRotate(rotateTo);
        SmoothCrawl();
        //Debug.Log("Key: " + Key + " AngleMod: " + angleY);
        //Debug.Log("rotateTo: " + rotateTo.eulerAngles);
        Debug.Log(Time.time - time);
    }


    // This needs to become a lot smarter.
    // Maybe compare current transform.forward to Vector3.constants and proceed from there?
    void RotateHead()
    {
        // Axis processing HERE.

        if (angleX == 0 && angleY == 0 && angleZ == 0)
        {
            forV = Snake[0].transform.forward;
        }
        else if (angleY > 0)
            forV = Snake[0].transform.right;
        else if (angleY < 0)
        {
            forV = Snake[0].transform.right;
            forV *= -1;
        }

        rotateTo = Quaternion.Euler(Snake[0].transform.rotation.eulerAngles.x + angleX,
                                    Snake[0].transform.rotation.eulerAngles.y + angleY,
                                    Snake[0].transform.rotation.eulerAngles.z + angleZ);

        angleX = 0;
        angleY = 0;
        angleZ = 0;
    }

    void SmoothRotate(Quaternion finish)
    {
        Quaternion from = Quaternion.Euler(0, Snake[0].transform.rotation.eulerAngles.y, 0);
        Quaternion to = Quaternion.Euler(0, Snake[0].transform.rotation.eulerAngles.y + angleInc, 0);

        // if current is outside of margin of final...
        if (Mathf.Abs(Snake[0].transform.rotation.eulerAngles.y - finish.eulerAngles.y) > rotAccuracy)
            Snake[0].transform.rotation = Quaternion.Lerp(from, to, lerpSpeed);

        // why the fuck is tnis not being accessed.
        // if current is within extended margin of final... still not being accessed.
        // This is why - rotAccuracy determines difference of FLOATS, ot DEGREES. 0.1 float is too much for a degree difference.
        //  1f is WAY enough.
        if ((Mathf.Abs(Snake[0].transform.rotation.eulerAngles.y - finish.eulerAngles.y) <= 3 * rotAccuracy) &&
            (Snake[0].transform.rotation != finish))
        {
            Snake[0].transform.rotation = finish;
            angleY = 0;
            blockInput = false;
        }
    }


    // fill moveTo array
    // CAREFUL WITH THIS. FORWARD NEEDS TO BE CORRECT, ROTATIONS MAY FUDGE UP
    void Crawl()
    {
        moveTo[0] = Snake[0].transform.position + forV;
        
        // every tail follows the one before
        for (int i = 1; i < Snake.Count + 1; i++)
        {
            moveTo[i] = Snake[i-1].transform.position;
        }
    }
    
    // lerp move to moveTo    
    void SmoothCrawl()
    {
        // THIS ALREADY NEEDS TO KNOW WHICH AXIS WE'RE CRAWLIN' ON. MARVELLOUS.
        //  Actually, you can take a diff vector. That's it.
        //  Actually, dummy, be more careful with what the hell you diff!

        for(int i = 0; i < Snake.Count; i++)
        {
            Vector3 from = Snake[i].transform.position;
            Vector3 to   = moveTo[i];
            
            Vector3 diff = from - to;
            if(Mathf.Abs(diff.magnitude) > movAccuracy)     // move up to
            {
                Snake[i].transform.position = Vector3.Lerp(from, to, 0.1f);
            }

            // this works fine.
            if (Mathf.Abs(diff.magnitude) <= movAccuracy)   // snap
            {
                Snake[i].transform.position = moveTo[i];
            }
        }
    }
    
    
    
    
    // Maybe I didn't account for the angle needing to be 360 and not 0 sometimes.
    //  OH WOW, this isn't being used anywhere.
    int CorrectAngle(float angle)
    {
        if (Math.Abs(angle) < 5) angle = 0;
        if (Math.Abs(angle - 90) < 5) angle = 90;
        if (Math.Abs(angle - 180) < 5) angle = 180;
        if (Math.Abs(angle - 270) < 5) angle = 270;
        if (Math.Abs(angle - 360) < 5) angle = 0;

        return (int)angle;
    }

    bool AngleWithinMargin(float angle)
    {
        if ((Math.Abs(angle) < 5) ||
            (Math.Abs(angle - 90) < 5) ||
            (Math.Abs(angle - 180) < 5) ||
            (Math.Abs(angle - 270) < 5) ||
            (Math.Abs(angle - 360) < 5))
            return true;
        else
            return false;
    }
    


    // This needs to become a lot smarter.
    // It needs to determine which axes it needs to manipulate.
    void CheckInput()
    {
        if ((Input.GetAxis("Fire1") != 0) && (Key == 0) && (!blockInput))
        {
            Key = Input.GetAxis("Fire1");
        }

        if((Key != 0) && (Input.GetAxis("Fire1") == 0) && (!blockInput))
        {
            if (Key > 0)
            {
                angleY = 90;
                angleInc = 2;
            }
            else if (Key < 0)
            { 
                angleY = -90;
                angleInc = -2;
            }

            Key = 0;
            blockInput = true;  // just where do you need to be, you fucking asshole.            
        }
    }    

    // to do: move collider processing to walls, fruits, tails.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fruit"))
        {
            Fruit.OnEaten();
            spawnTail = true;
            spawnCounter = 1;
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
