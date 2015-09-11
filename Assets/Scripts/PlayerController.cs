using UnityEngine;
using System.Collections;

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

    private GameObject Snake;
    
    private Vector3 pos;
    private Vector3 moveVector = new Vector3(0, 0, 1);
    
    private Quaternion[] rotYVectors = 
    {
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, 180, 0),
        Quaternion.Euler(0, 270, 0)
    };
    private int rotYIndex = 0;

    void Start()
    {
        Snake = GameObject.FindWithTag("Snake");
        Snake.transform.position = new Vector3(5,0,5);
        pos = Snake.transform.position;

        rotYIndex = 0;

        StartCoroutine(SnakeControl());
    }

	void Update()
    {
        TrackInput();        
	}
    
    IEnumerator SnakeControl()
    {
        while (((pos.x > 0) && (pos.x < 19)) && ((pos.z > 0) && (pos.z < 19)))
        {
            yield return new WaitForSeconds(0.5f);

            Crawl();
        }
    }

    void Crawl()
    {
        pos += moveVector;
        
        Snake.transform.position = pos;
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

    void TrackKey(string k, Vector3 pMod)
    {
        bool KeyDown = false;
        
        if (Input.GetKeyDown(k))
        {
            if (pMod == Vector3.right)
            {
                rotYIndex++;
                if (rotYIndex > 3) rotYIndex = 0;
            }
            if (pMod == Vector3.left)
            {
                rotYIndex--;
                if (rotYIndex < 0) rotYIndex = 3;                
            }
            Snake.transform.rotation = rotYVectors[rotYIndex];
            
            Quaternion checkAngle = transform.rotation;
            if (checkAngle == Quaternion.Euler(0, 90, 0))
            {
                moveVector = new Vector3(1, 0, 0);
            }
            else if (checkAngle == Quaternion.Euler(0, 270, 0))
            {
                moveVector = new Vector3(-1, 0, 0);
            }
            else if (checkAngle == Quaternion.Euler(0, 0, 0))
            {
                moveVector = new Vector3(0, 0, 1);
            }
            else if (checkAngle == Quaternion.Euler(0, 180, 0))
            {
                moveVector = new Vector3(0, 0, -1);
            }
            
            
            if (!KeyDown)
            {
                KeyDown = true;
            }
        }

        if (Input.GetKeyDown(k))
            if (KeyDown) KeyDown = false;
    }
    // ---
}
