using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour
{
    // all the public vars that don't have to be accessed from another class, serialize them
    bool alive = true;

    GameObject SnakeHead;                               // le head.
    public GameObject SnakeTail;                        // le tails.
    List<GameObject> Snake = new List<GameObject>();    // le snake.

    private FruitController Fruit;
    bool spawnTail = false;
    int spawnCounter = 0;

    GameObject inputManagerObject;
    InputManager inputManager;
    int[] controls = new int[2];
    
    public Vector3[] moveTo = new Vector3[10];     // array of next coords for tails; 10 max atm.
    
    // durations --- 
    [SerializeField] float initialDelay = 1f; // delay after launch    
    [SerializeField] float rotationDelay = 1.5f; // delay between cycles.
    
    public float rotationSpeed = 20;    // change this into duration later
    
    public float movDuration;
    float movElapsed = 0;
    float movAccuracy = 0.01f;


    Vector3 forV;   // vector for the head to form next coords for the head?
                    // maybe I should assign rotation to head in the same place.

    bool isMoving = false;  // no input is applied during any kind of head motion

    Vector3 horAxis = new Vector3(0, 1, 0);     // right of a classic basis
    Vector3 verAxis = new Vector3(1, 0, 0);     // up of a classic basis
    //Vector3[] axes = { new Vector3(0, 1, 0), new Vector3(-1, 0, 0) };

    int[] nullArray = { 0, 0 };
    public int[] controlCheck = { 0, 0 };


    void Start()
    {
        // preparations ---
        // get the head!
        SnakeHead = GameObject.FindWithTag("SnakeHead");
        Snake.Add(SnakeHead);

        // get the fruit!
        GameObject fruitControllerObject = GameObject.FindWithTag("Fruit");
        Fruit = fruitControllerObject.GetComponent<FruitController>();
        
        // get the input! ---
        inputManagerObject = GameObject.FindWithTag("InputManager");
        inputManager = inputManagerObject.GetComponent<InputManager>();

        // spawn conditions
        SnakeHead.transform.position = new Vector3(5, 0, 5);        

        // initial movement - go _forward_.
        //moveTo[0] = Snake[0].transform.position + Snake[0].transform.forward;
                
        // launch ---
        StartCoroutine(SnakeCycle());
    }

    void Update()
    {
        if (isMoving)
        {
            return;
        }
        inputManager.Check(controlCheck);
    }

    // This is... eh. Probably ok.
    void ProcessInput(int[] controls)
    {
        // hor axis
        if (controls[0] != 0)
        {
            StartCoroutine(SnakeRotate(controls[0] * horAxis));
        }
        // ver axis
        else if (controls[1] != 0)
        {
            StartCoroutine(SnakeRotate(controls[1] * -verAxis));
        }
    }


    // hehe, we might not even need the Update().
    IEnumerator SnakeCycle()
    {
        yield return new WaitForSeconds(initialDelay);
        while (alive)   // snake lives, snake crawls.
                        // TO DO: think about introducing a pause bool. if not for pause, at least for options.
        {
            // if there is a rotation, rotate
            if (controlCheck != nullArray)
            {
                ProcessInput(controlCheck);
                //StartCoroutine(SnakeRotate(controls[axis] * axisArray[axis]));

                // reset input
                controlCheck = new int[] {0,0};
            }
            yield return new WaitForSeconds(rotationDelay);

            // move, always
            forV = Snake[0].transform.forward;
            moveTo[0] = Snake[0].transform.position + forV;

            StartCoroutine(SnakeCrawl());
            yield return new WaitForSeconds(movDuration);

            // spawn conditions
            if (spawnTail && (spawnCounter-- == 0))
            {
                AddTail();
                spawnTail = false;
            }
        }
    }
        
    IEnumerator SnakeRotate(Vector3 axis)
    {
        isMoving = true;    // false it when crawl ends
                            // when you do correct input sequence.

        // Time base this!
        float currAngle = 0;
        while(currAngle < 90)
        {
            float rotProgr = Time.deltaTime * rotationSpeed;
            if (rotProgr + currAngle > 90)
            {
                rotProgr = 90 - currAngle;
            }
            Snake[0].transform.Rotate(axis, rotProgr);

            currAngle += rotProgr;

            yield return null;
        }
        AngleCorrection();

        isMoving = false;
    }

    IEnumerator SnakeCrawl()
    {
        forV = Snake[0].transform.forward;
        moveTo[0] = Snake[0].transform.position + forV;
        
        for (int i = 1; i < Snake.Count + 1; i++)
        {
            moveTo[i] = Snake[i-1].transform.position;
        }
        
        movElapsed = 0;

        while(movElapsed < movDuration)
        {
            for (int i = 0; i < Snake.Count; i++)
            {
                Vector3 start = Snake[i].transform.position;
                Vector3 target = moveTo[i];
                Vector3 range = target - start;

                movElapsed = Mathf.MoveTowards(movElapsed, movDuration, Time.deltaTime);
                Snake[i].transform.position = start + range * (movElapsed / movDuration);
            }
            yield return null;
        }

        for (int i = 0; i < Snake.Count; i++)
        {
            Snake[i].transform.position = moveTo[i];
        }
    }


    void AngleCorrection()
    {
        Vector3 angle = transform.rotation.eulerAngles;
        angle.x = Mathf.Round(angle.x / 90) * 90;
        angle.y = Mathf.Round(angle.y / 90) * 90;
        angle.z = Mathf.Round(angle.z / 90) * 90;
        transform.rotation = Quaternion.Euler(angle);
    }

    // collider move time soon
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