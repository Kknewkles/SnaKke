using UnityEngine;
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

    private FruitController Fruit;

    private Vector3 moveVector = new Vector3(0, 0, 1);

    public float speed = 0.5f;

    List<GameObject> Snake = new List<GameObject>();
    // rotation and direction
    // 0 - forward, 1 - right, 2 - back, 3 - left
    
    private Quaternion[] rotYVectors = 
    {
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, 180, 0),
        Quaternion.Euler(0, 270, 0)
    };
    private Vector3[] dirVectors = 
    {
        Vector3.forward,
        Vector3.right,
        Vector3.back,
        Vector3.left
    };
    private int orientIndex = 0;

    
    void Start()
    {
        // get fruit controller
        GameObject fruitControllerObject = GameObject.FindWithTag("Fruit");
        Fruit = fruitControllerObject.GetComponent<FruitController>();
        
        // track the head
        SnakeHead = GameObject.FindWithTag("SnakeHead");
        Snake.Add(SnakeHead);
        
        SnakeHead.transform.position = new Vector3(5,0,5);
        //pos = SnakeHead.transform.position;
        
        orientIndex = 0;

        StartCoroutine(SnakeControl());
    }

    IEnumerator SnakeControl()
    {
        while(alive)
        {
            yield return new WaitForSeconds(speed);

            //Crawl();
            Creep();
        }
    }

    /*
    void Crawl()
    {        
        // --- SNAKE HEAD PROCESSING ---
        headPos = SnakeHead.transform.position;
        
        // posMod = modified position
        Vector3 posMod = headPos + moveVector;

        //  Come to think of it, this entire thing is only for tests and will be deleted later.

        // Replace hard limits with variables later
        // if modified position satisfies a condition, assign it
        if (((posMod.x >= 0) && (posMod.x <= 19)) &&
            ((posMod.z >= 0) && (posMod.z <= 19)))
        {
            Snake[0].transform.position = posMod;
        }

        
        // offset = difference between current and previous,
        //  which will serve as position modifier on next move
        Vector3 offset = moveVector;
        Vector3 mv = offset;
        // --- SNAKE TAIL PROCESSING ---
        for(int i = 1; i < Snake.Count; i++)
        {
            //Snake[i].transform.position = Snake[i-1].transform.position - offset;
            Snake[i].transform.position += mv;
            offset = Snake[i-1].transform.position - Snake[i].transform.position;
            mv = offset;
        }
    }
    */

    void Creep()
    {
        Vector3 vector, nextVector;
        vector = moveVector;
        
        if(Snake.Count == 1)
            Snake[0].transform.position += moveVector;

        if (Snake.Count > 1)
        {
            for (int i = 0; i < Snake.Count - 1; i++)
            {
                nextVector = Snake[i].transform.position - Snake[i + 1].transform.position;
                Snake[i].transform.position += vector;
                vector = nextVector;
            }
        }
        
    }

    void Update()
    {
        TrackInput();
        int count = Snake.Count;
        Debug.Log(count);
    }

    void TrackInput()
    {
        TrackKey("right", Vector3.right);
        TrackKey("d", Vector3.right);

        TrackKey("left", Vector3.left);
        TrackKey("a", Vector3.left);
        // save for later 3d
        /*
        TrackKey("up", Vector3.up);
        TrackKey("w", Vector3.up);

        TrackKey("down", Vector3.down);
        TrackKey("s", Vector3.down);
        */
    }
    
    //private float angleY = 0;
    //private Vector3 rotation = new Vector3(0, 0, 0);

    void TrackKey(string k, Vector3 pMod)
    {
        bool KeyDown = false;
        
        if (Input.GetKeyDown(k))
        {
            //if (pMod == Vector3.right)
            if(k == "right" || k == "d")
            {
                orientIndex++;
                if (orientIndex > 3) orientIndex = 0;
                
                // move to angles? Screw arrays? Or keep them in mind for 3d?
                //angleY += 90;        
            }
            if (k == "left" || k == "a")
            //if (pMod == Vector3.left)
            {
                orientIndex--;
                if (orientIndex < 0) orientIndex = 3;
                
                //angleY -= 90;
            }

            // apply rotation... or direction?
            SnakeHead.transform.rotation = rotYVectors[orientIndex];
            moveVector = dirVectors[orientIndex];

            //rotation = new Vector3(0, angleY, 0);
            //SnakeHead.transform.rotation = rotation;

            if (!KeyDown)
            {
                KeyDown = true;
            }
        }

        if (Input.GetKeyDown(k))
            if (KeyDown) KeyDown = false;
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
        // set up a tail segment
        
        Vector3 move = dirVectors[orientIndex];
        //Vector3 coords = SnakeHead.transform.position - move;
        
        
        Vector3 coord = Snake[0].transform.position;
        Debug.Log(coord.x + "," + coord.y + "," + coord.z);

        Vector3 coords = Snake[Snake.Count-1].transform.position - move;        

        // if you take one of existing/from objectpool, how do you distinguish and refer between them?
        SnakeTail = (GameObject)(Instantiate(SnakeTail, coords, Quaternion.identity));
        Snake.Add(SnakeTail);
    }
}
