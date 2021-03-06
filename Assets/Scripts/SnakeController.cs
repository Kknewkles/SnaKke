﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour
{
    public static SnakeController instance;
    
    // all the public vars that don't have to be accessed from another class, serialize them
    public bool alive = true;

    // Is the head our instance? Or the primary 
    public GameObject SnakeHead;                        // le head.
    public GameObject SnakeTail;                        // le tails.
    public List<GameObject> Snake = new List<GameObject>();    // le snake.
    public GameObject FruitCompass;
    
    [HideInInspector] public bool spawnTail = false;
    [HideInInspector] public int spawnCounter = 0;
    
    [HideInInspector] public Vector3 tailSpawnCoord;
    [HideInInspector] public Vector3 spawnPoint;

    // this should be a dynamic array.
    // or at least a much bigger static one.
    [HideInInspector] public Vector3[] moveTo = new Vector3[100];     // array of next coords for tails; 10 max atm.
    
    // durations --- 
    public float initialDelay = 1f; // delay after launch    
    
    public float movDuration;
    float movElapsed = 0;

    public float rotDuration = 2;

    [HideInInspector] public Vector3 forV;   // vector for the head to form next coords for the head?
                    // maybe I should assign rotation to head in the same place.

    bool isMoving = false;  // no input is applied during any kind of head motion

    Vector3 horAxis = new Vector3(0, 1, 0);     // right of a classic basis
    Vector3 verAxis = new Vector3(1, 0, 0);     // up of a classic basis
    //Vector3[] axes = { new Vector3(0, 1, 0), new Vector3(-1, 0, 0) };

    int[] nullArray = { 0, 0 };
    
    int[] controlCheck = { 0, 0 };   // working variable for every check
    int[] saveControls = { 0, 0 };   // variable for storing non-zero input
    int[] applyInput = { 0, 0 };     // variable that applies to the snake

    void Awake()
    {
        instance = this;        
    }

    void Start()
    {
        // launch ---
        Snake_Spawn();

        StartCoroutine(SnakeCycle());
    }

    public void Snake_Spawn()
    {
        spawnPoint = new Vector3(5, 0, 5);

        // Spawn the head
        SnakeHead = (GameObject)Instantiate(SnakeHead, spawnPoint, Quaternion.identity);
        Snake.Add(SnakeHead);
        
    }

    public void Snake_Reset()
    {
        SnakeHead.transform.position = new Vector3(5, 0, 5);
        SnakeHead.transform.rotation = Quaternion.Euler(0, 0, 0);
        moveTo[0] = new Vector3(5, 0, 6);
        
        Time.timeScale = 1;
    }

    void Update()
    {
        // always poll
        InputManager.instance.Check(controlCheck);

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

        //Debug.Log(AudioListener.volume);
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
        // the tail should move towards the previous coords of last tail;
        // it should spawn at that coord - move of the last tail.
        //moveTo[Snake.Count] = Snake[Snake.Count - 1].transform.position;
        //Vector3 coords = moveTo[Snake.Count - 1];
        Snake.Add((GameObject)(Instantiate(SnakeTail, tailSpawnCoord, Quaternion.identity)));
    }
}