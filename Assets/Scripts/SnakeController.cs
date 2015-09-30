using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour
{
    // all the public vars that don't have to be accessed from another class, serialize them
    public bool alive = true;

    GameObject SnakeHead;                               // le head.
    public GameObject SnakeTail;                        // le tails.
    List<GameObject> Snake = new List<GameObject>();    // le snake.

    
    [HideInInspector] public bool spawnTail = false;
    public int spawnCounter = 0;

    GameObject inputManagerObject;
    InputManager inputManager;
        
    public Vector3[] moveTo = new Vector3[10];     // array of next coords for tails; 10 max atm.
    
    // durations --- 
    public float initialDelay = 1f; // delay after launch    
    
    public float movDuration;
    float movElapsed = 0;

    public float rotDuration = 2;    // change this into duration later

    Vector3 forV;   // vector for the head to form next coords for the head?
                    // maybe I should assign rotation to head in the same place.

    bool isMoving = false;  // no input is applied during any kind of head motion

    Vector3 horAxis = new Vector3(0, 1, 0);     // right of a classic basis
    Vector3 verAxis = new Vector3(1, 0, 0);     // up of a classic basis
    //Vector3[] axes = { new Vector3(0, 1, 0), new Vector3(-1, 0, 0) };

    int[] nullArray = { 0, 0 };
    
    int[] controlCheck = { 0, 0 };   // working variable for every check
    int[] saveControls = { 0, 0 };   // variable for storing non-zero input
    int[] applyInput = { 0, 0 };     // variable that applies to the snake


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
        Vector3 spawnPoint = new Vector3(5, 0, 5);
        SnakeHead.transform.position = spawnPoint;
                        
        // launch ---
        StartCoroutine(SnakeCycle());
    }

    void Update()
    {
        // always poll
        inputManager.Check(controlCheck);

        // save if not zero
        if(controlCheck[0] != 0 || controlCheck[1] != 0)
        {
            saveControls[0] = controlCheck[0];
            saveControls[1] = controlCheck[1];
        }
        
        if(!isMoving)
            return;
        else
        {
            // apply if not moving
            if(saveControls[0] != 0 || saveControls[1] != 0)
            {
                applyInput[0] = saveControls[0];
                applyInput[1] = saveControls[1];
            }
        }

    }

    // This is... eh. Probably ok.
    void ProcessInput(int[] controls)
    {
        // hor axis
        int value = 0;
        if (controls[0] != 0)
        {
            value = controls[0];
            StartCoroutine(SnakeRotate(value * horAxis));
        }
        // ver axis
        else if (controls[1] != 0)
        {
            value = controls[1];
            StartCoroutine(SnakeRotate(value * -verAxis));
        }
    }


    // hehe, we might not even need the Update().
    IEnumerator SnakeCycle()
    {
        yield return new WaitForSeconds(initialDelay);
        while (alive)   // snake lives, snake crawls.
                        // TO DO: think about introducing a pause bool. if not for pause, at least for options.
        {
            if(applyInput[0] != 0 || applyInput[1] != 0)
            {
                ProcessInput(applyInput);
                
                yield return new WaitForSeconds(rotDuration);
            }

            forV = Snake[0].transform.forward;
            moveTo[0] = Snake[0].transform.position + forV;

            StartCoroutine(SnakeCrawl());
            yield return new WaitForSeconds(movDuration);

            // spawn conditions
            if (spawnTail && (--spawnCounter == 0))
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

        // WHILE NOT ROTATED
        float currAngle = 0;
        while(currAngle < 90)
        {
            float incAngle = 90 * Time.deltaTime / rotDuration; // seems a bit hacky, but it's actually fine.
            if (incAngle + currAngle > 90)
            {
                incAngle = 90 - currAngle;
            }
            Snake[0].transform.Rotate(axis, incAngle);  // rotate around axis by incAngle

            currAngle += incAngle;

            yield return null;
        }
        AngleCorrection();

        isMoving = false;

        // reset inputs
        applyInput[0] = 0;
        applyInput[1] = 0;
        saveControls[0] = 0;
        saveControls[1] = 0;
    }

    IEnumerator SnakeCrawl()
    {
        isMoving = true;

        forV = Snake[0].transform.forward;
        moveTo[0] = Snake[0].transform.position + forV;
        
        for (int i = 1; i < Snake.Count; i++)
        {
            moveTo[i] = Snake[i - 1].transform.position;
        }
        
        movElapsed = 0;

        while(movElapsed < movDuration)
        {
            for(int i = 0; i < Snake.Count; i++)
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

        isMoving = false;
    }



    void AngleCorrection()
    {
        Vector3 angle = transform.rotation.eulerAngles;
        angle.x = Mathf.Round(angle.x / 90) * 90;
        angle.y = Mathf.Round(angle.y / 90) * 90;
        angle.z = Mathf.Round(angle.z / 90) * 90;
        transform.rotation = Quaternion.Euler(angle);
    }

    // This will need to become a bit smarter for the ObjectPool.
    public void AddTail()
    {
        // is it + or - move vector?...
        // does the last tail shift by head's move or previous's move?
        // maybe we could pass the forth vector with the call. Should be correct.

        Vector3 move = Snake[0].transform.forward;
        Vector3 coords = Snake[Snake.Count - 1].transform.position - move;

        // if you take one of existing/from objectpool, how do you distinguish and refer between them?
        SnakeTail = (GameObject)(Instantiate(SnakeTail, coords, Quaternion.identity));
        Snake.Add(SnakeTail);

        // with ObjectPool snake will have to track tails only up to snake's length,
        //  which will have to become _a separate variable_ from Snake.Count.
    }
    
    // all this to dedicated OptionsManager
    // ---
    
    // ---
}