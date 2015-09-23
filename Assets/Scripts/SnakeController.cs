using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour
{
    // all the public vars that don't have to be accessed from another class, serialize them
    bool alive = true;

    GameObject inputManagerObject;
    InputManager inputManager;
    float inputHor = 0;
    float inputVer = 0;
    int[] controls = new int[2];

    GameObject SnakeHead;                               // le head.
    public GameObject SnakeTail;                        // le tails.
    List<GameObject> Snake = new List<GameObject>();    // le snake.

    Vector3[] moveTo = new Vector3[10];     // array of next coords for tails; 10 max atm.
    Quaternion rotateTo = Quaternion.Euler(0, 0, 0);    // not sure about this one.
    Vector3 angle;  // this might be better. Try constructing it yourself and then applying to the head.

    float initialDelay = 3f;                // delay after launch    
    [SerializeField] float cycleDelay = 1f; // delay between cycles.


    Vector3 forV;   // vector for the head to form next coords for the head?
                    // maybe I should assign rotation to head in the same place.

    bool noInput;   // if there's no input atm, apply.

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

        // initial angle is default
        angle = new Vector3(0, 0, 0);

        noInput = true;

        // launch ---
        StartCoroutine(SnakeCycle());
    }

    // do everything with coroutines this time? Motion, rotation - everything?
    IEnumerator SnakeCycle()
    {
        yield return new WaitForSeconds(2);
        while (alive)   // snake lives, snake crawls.
                        // TO DO: think about introducing a pause bool.
        {
            // assign values from input; or assign final values.


            // if there is a rotation, rotate

            // move, always
            // actually, hold off movement some. Shoulda tested rotations on a stationary ball.
            yield return new WaitForSeconds(1);
            noInput = true;
        }
    }
    
    // leave only input processing to updates.
    void FixedUpdate()
    {
        ProcessInput(inputManager.Check());
        // correct the angles that like to get screwed up here and there.
    }

    // This is it.
    void ProcessInput(int[] controls)
    {
        if (controls[0] == 0 && controls[1] == 0) ;
        else if (noInput)
        {
            // hor check
            if (controls[0] < 0)
                TurnLeft();
            if (controls[0] > 0)
                TurnRight();

            // ver check. Don't forget about invert.
            if (controls[1] < 0)
                LookUp();
            if (controls[1] > 0)
                LookDown();

            noInput = false;
        }
    }

    Vector3 CorrectAngle(Vector3 angle)
    {
        int margin = 10;
        
        for(int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(angle[i]) < margin)
                angle[i] = 0;
            if (Mathf.Abs(angle[i] - 90) < margin)
                angle[i] = 90;
            if (Mathf.Abs(angle[i] - 180) < margin)
                angle[i] = 180;
            if (Mathf.Abs(angle[i] - 270) < margin)
                angle[i] = 270;
            if (Mathf.Abs(angle[i] - 360) < margin)
                angle[i] = 0;
        }

        return angle;
    }

    int CorrectAngle(float angle)
    {
        int margin = 5;
        
        if (Mathf.Abs(angle) < margin) angle = 0;
        if (Mathf.Abs(angle - 90) < margin) angle = 90;
        if (Mathf.Abs(angle - 180) < margin) angle = 180;
        if (Mathf.Abs(angle - 270) < margin) angle = 270;
        if (Mathf.Abs(angle - 360) < margin) angle = 0;

        return (int)angle;
    }

    // // --- CAUTION --- 

    // determine relative horizontal axis, rotate.
    // hor. rotations - axis is transform.up 
    //  Now logically, application of the angle should be done somewhere else.
    void TurnLeft()
    {
        int axis = 0;
        int sign = 1;
        for(int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.up[i] != 0)
            {
                axis = i;
                sign = (int)Snake[0].transform.up[i];
            }
        }

        //angle[axis] = Snake[0].transform.rotation.eulerAngles[axis];
        angle[axis] -= 90 * sign;
        //angle[axis] = CorrectAngle(angle[axis]);
        Snake[0].transform.rotation = Quaternion.Euler(CorrectAngle(angle));
    }
    void TurnRight()
    {
        int axis = 0;
        int sign = 1;
        for (int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.up[i] != 0)
            {
                axis = i;
                sign = (int)Snake[0].transform.up[i];
            }
        }

        //angle[axis] = Snake[0].transform.rotation.eulerAngles[axis];
        angle[axis] += 90 * sign;
        //angle[axis] = CorrectAngle(angle[axis]);
        Snake[0].transform.rotation = Quaternion.Euler(CorrectAngle(angle));
    }

    // vertical axis
    // rotation axis - transform.right
    void LookUp()
    {
        int axis = 0;
        int sign = 1;
        for (int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.right[i] != 0)
            {
                axis = i;
                sign = (int)Snake[0].transform.right[i];
            }
        }

        //angle[axis] = Snake[0].transform.rotation.eulerAngles[axis];
        angle[axis] += 90 * sign;
        //angle[axis] = CorrectAngle(angle[axis]);
        Snake[0].transform.rotation = Quaternion.Euler(CorrectAngle(angle));
    }
    void LookDown()
    {
        int axis = 0;
        int sign = 1;
        for (int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.right[i] != 0)
            {
                axis = i;
                sign = (int)Snake[0].transform.right[i];
            }
        }

        //angle[axis] = Snake[0].transform.rotation.eulerAngles[axis];
        angle[axis] -= 90 * sign;
        //angle[axis] = CorrectAngle(angle[axis]);
        Snake[0].transform.rotation = Quaternion.Euler(CorrectAngle(angle));
    }
    
    // \\ --- CAUTION ---

    // stub
    IEnumerator SnakeRotate()
    {
        if (true) { yield return 0; }
    }

    // stub
    IEnumerator SnakeCrawl()
    {
        if(true) { yield return 0; }
    }    
}
