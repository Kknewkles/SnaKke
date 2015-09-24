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

    GameObject inputManagerObject;
    InputManager inputManager;
    int[] controls = new int[2];
    
    Vector3[] moveTo = new Vector3[10];     // array of next coords for tails; 10 max atm.
    /*
    Quaternion rotateTo = Quaternion.Euler(0, 0, 0);    // not sure about this one.
    Vector3 angle = new Vector3(0, 0, 0);   // this might be better. Try constructing it yourself and then applying to the head.
    */

    [SerializeField] float initialDelay = 1f; // delay after launch    
    [SerializeField] float rotationDelay = 1.5f; // delay between cycles.
    
    public float rotationSpeed = 20;    // change this into duration later


    Vector3 forV;   // vector for the head to form next coords for the head?
                    // maybe I should assign rotation to head in the same place.

    bool isMoving = false;  // no input is applied during any kind of head motion

    Vector3 horAxis = new Vector3(0, 1, 0);     // right of a classic basis
    Vector3 verAxis = new Vector3(1, 0, 0);     // up of a classic basis

    int[] nullArray = { 0, 0 };
    public int[] controlCheck = { 0, 0 };


    void Start()
    {
        // preparations ---
        // get the head!
        SnakeHead = GameObject.FindWithTag("SnakeHead");
        Snake.Add(SnakeHead);

        // get the input! ---
        inputManagerObject = GameObject.FindWithTag("InputManager");
        inputManager = inputManagerObject.GetComponent<InputManager>();

        // spawn conditions
        SnakeHead.transform.position = new Vector3(5, 0, 5);        

        // initial movement - go _forward_.
        moveTo[0] = Snake[0].transform.position + Snake[0].transform.forward;
                
        // launch ---
        StartCoroutine(SnakeCycle());
    }
    
    // hehe, we might not even need the Update().
    IEnumerator SnakeCycle()
    {
        yield return new WaitForSeconds(initialDelay);
        while (alive)   // snake lives, snake crawls.
                        // TO DO: think about introducing a pause bool. if not for pause, at least for options.
        {
            // assign values from input; or assign final values.


            // if there is a rotation, rotate
            if (controlCheck != nullArray)
            {
                ProcessInput(controlCheck);
                controlCheck = new int[] {0,0};
            }

            // move, always
            yield return new WaitForSeconds(rotationDelay);


        }
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
    // Zody has one func for each axis, you better do too.
    void ProcessInput(int[] controls)
    {
        if (controls[0] != 0)
        {
            StartCoroutine(SnakeRotate(controls[0] * horAxis));
        }
        else if (controls[1] != 0)
        {
            StartCoroutine(SnakeRotate(controls[1] * -verAxis));
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
    

    IEnumerator SnakeRotate(Vector3 axis)
    {
        isMoving = true;    // false it when crawl ends

        // Time base it!
        float currAngle = 0;
        while(currAngle < 90)
        {
            float rotProgr = Time.deltaTime * rotationSpeed;  // leave it like this for now, but turn into time after.
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

    // stub
    IEnumerator SnakeCrawl()
    {
        if(true) { yield return 0; }
    }    
}
