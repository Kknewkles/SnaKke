using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class PlayerController : MonoBehaviour
{
    private bool alive = true;

    private GameObject SnakeHead;
    public GameObject SnakeTail;
    List<GameObject> Snake = new List<GameObject>();

    private FruitController Fruit;

    // angle to construct and assign to the snake's head
    /*
    private Quaternion LeAngle = Quaternion.Euler(0, 0, 0);
    private int LeAngleX = 0;
    private int LeAngleY = 0;
    private int LeAngleZ = 0;
    */

    Vector3[] moveTo = new Vector3[10];

    Quaternion rotateTo = Quaternion.Euler(0, 0, 0);

    float rotAccuracy = 3f;
    float movAccuracy = 0.01f;

    public float turnDelay = 1f;
    public float rotTime = 2;
    public float movTime = 2;
    float rotElapsed;
    float movElapsed;
    
    float angleInc;

    /*
    private float angleX;
    private float angleY;
    private float angleZ;
    */
    // =>
    Vector3 angle;

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
        
        Key = 0;

        /*
        angleX = 0;
        angleY = 0;
        angleZ = 0;
        */
        // =>
        angle = new Vector3(0, 0, 0);

        Vector3 forV = Snake[0].transform.forward;

        time = 0;
        StartCoroutine(SnakeControl());
    }
        
    IEnumerator SnakeControl()
    {
        while(alive)
        {
            // delay between turns
            yield return new WaitForSeconds(turnDelay);

            RotateHead();
            Crawl();

            // wait for longest of rotation or movement animations
            float motionDelay = 0;
            if (movTime > rotTime)
                motionDelay = movTime;
            else
                motionDelay = rotTime;
            movElapsed = 0;
            rotElapsed = 0;
            yield return new WaitForSeconds(motionDelay);            

            // crap to do in the turn's end
            if(spawnTail && (spawnCounter-- == 0))
            {
                AddTail();
                spawnTail = false;
            }
        }
    }

    void FixedUpdate()
    {
        CheckInput();
        SmoothRotate(rotateTo);
        SmoothCrawl();

        //Debug.Log("Key: " + Key + " AngleMod: " + angleY);
        //Debug.Log("rotateTo: " + rotateTo.eulerAngles);
        //Debug.Log(Time.time - time + " " + Time.deltaTime);
    }


    // This needs to become a lot smarter.
    // Maybe compare current transform.forward and transform.right to Vector3.constants and proceed from there?
    //  If tr.up is vector(0,-1,0), forward and right need to *-1. ...multiply them by the -1 taken from up?
    //  
    void RotateHead()
    {
        //Vector3 angle = new Vector3(0, 0, 0);
        
        // Axis processing HERE.
        
        /*
        if (angleX == 0 && angleY == 0 && angleZ == 0)
			forV = Snake[0].transform.forward;
        
        else if (angleY > 0)
            forV = Snake[0].transform.right;
        else if (angleY < 0)
        {
            forV = Snake[0].transform.right;
            forV *= -1;
        }
        */
        
        // =>
        // not sure about this one. 
        if (angle == new Vector3(0, 0, 0))
            forV = Snake[0].transform.forward;

        else if (angle[1] > 0)
            forV = Snake[0].transform.right;
        else if (angle[1] < 0)
        {
            forV = Snake[0].transform.right;
            forV *= -1;
        }
        
        
        /*
        rotateTo = Quaternion.Euler(CorrectAngle(Snake[0].transform.rotation.eulerAngles.x + angleX),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.y + angleY),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.z + angleZ));
        */
        // =>
        
        rotateTo = Quaternion.Euler(CorrectAngle(Snake[0].transform.rotation.eulerAngles.x + angle[0]),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.y + angle[1]),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.z + angle[2]));
        

        /*
        angleX = 0;
        angleY = 0;
        angleZ = 0;
        */
        // => 
        angle = new Vector3(0, 0, 0);
    }

    void SmoothRotate(Quaternion finish)
    {
        // Seems to be not needed
        //Quaternion from = Snake[0].transform.rotation;
        
        /*
        if (Mathf.Abs(Snake[0].transform.rotation.eulerAngles.y - finish.eulerAngles.y) > rotAccuracy)
            Snake[0].transform.rotation = Quaternion.Lerp(Snake[0].transform.rotation, finish, 0.1f);
        */
        
        if (rotElapsed < rotTime)
        {
            rotElapsed += Time.deltaTime;
            Snake[0].transform.rotation = Quaternion.Lerp(Snake[0].transform.rotation, finish, rotElapsed/rotTime);
        }

        // on acc. -> dur. this might still be useful.
        if ((Mathf.Abs(Snake[0].transform.rotation.eulerAngles.y - finish.eulerAngles.y) <= rotAccuracy) && (Snake[0].transform.rotation != finish))
        // =>
        if ((Mathf.Abs(Snake[0].transform.rotation.eulerAngles[1] - finish.eulerAngles[1]) <= rotAccuracy) && (Snake[0].transform.rotation != finish))
        {
            Snake[0].transform.rotation = finish;
            
            //angleY = 0;
            // =>
            angle = new Vector3(0, 0, 0);
            
            blockInput = false;

            rotElapsed = rotTime;
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

            if(movElapsed < movTime)
            {
                movElapsed += Time.deltaTime;
                Snake[i].transform.position = Vector3.Lerp(from, to, movElapsed/movTime);
            }

            // on acc. -> dur. this might still be useful.
            if (Mathf.Abs(diff.magnitude) <= movAccuracy)   // snap
            {
                Snake[i].transform.position = moveTo[i];

                movElapsed = movTime;
            }
        }
    }
    
    
        
    // Maybe I didn't account for the angle needing to be 360 and not 0 sometimes.
    int CorrectAngle(float angle)
    {
        if (Mathf.Abs(angle) < 5) angle = 0;
        if (Mathf.Abs(angle - 90) < 5) angle = 90;
        if (Mathf.Abs(angle - 180) < 5) angle = 180;
        if (Mathf.Abs(angle - 270) < 5) angle = 270;
        if (Mathf.Abs(angle - 360) < 5) angle = 0;

        return (int)angle;
    }


    // This needs to become a lot smarter.
    // It needs to determine which axes it needs to manipulate.

    // well, not that much smarter, but we'll need a second key.
    // This probably needs to become an int type, returning only value.
    void CheckInput()
    {
        // check which avis to modify
        
        if ((Key == 0) && (Input.GetAxis("Fire1") != 0) && (!blockInput))
        {
            Key = Input.GetAxis("Fire1");
        }

        if((Key != 0) && (Input.GetAxis("Fire1") == 0) && (!blockInput))
        {
            if (Key > 0)
            {
                //angleY = 90;
                // =>
                angle[1] = 90;
            }
            else if (Key < 0)
            { 
                //angleY = -90;
                // =>
                angle[1] = -90;
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
