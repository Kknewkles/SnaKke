using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class PlayerController : MonoBehaviour
{
    private bool alive = true;

    private GameObject SnakeHead;
    public GameObject SnakeTail;
    List<GameObject> Snake = new List<GameObject>();
    
    Vector3[] moveTo = new Vector3[10];
    Quaternion rotateTo = Quaternion.Euler(0, 0, 0);

    Vector3 angle;
    

    float rotAccuracy = 3f;
    float movAccuracy = 0.01f;

    public float turnDelay = 0.5f;
    public float rotTime = 1;
    public float movTime = 1;
    float rotElapsed;
    float movElapsed;
    
    
    Vector3 forV;
    private float KeyAD, KeyWS;
    bool blockInput = false;

    private FruitController Fruit;
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

        // spawn conditions(AD)
        //SnakeHead.transform.position = new Vector3(5,0,5);
        // spawn conditions(WS)
        SnakeHead.transform.position = new Vector3(0, 5, 5);
        
        moveTo[0] = Snake[0].transform.position + Snake[0].transform.forward;
        
        
        KeyAD = 0;
        KeyWS = 0;

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
        CheckInputAD();
        CheckInputWS();
        SmoothRotate(rotateTo);
        SmoothCrawl();

        //Debug.Log("Input is locked: " + blockInput);
        //Debug.Log("Up: " + Snake[0].transform.up + " Right: " + Snake[0].transform.right);

        //Debug.Log("Key: " + Key + " AngleMod: " + angleY);
        Debug.Log("rotateTo: " + rotateTo.eulerAngles);
        //Debug.Log(Time.time - time + " " + Time.deltaTime);
    }


    // Maybe compare current transform.forward and transform.right to Vector3.constants and proceed from there?
    //  If tr.up is vector(0,-1,0), forward and right need to *-1. ...multiply them by the -1 taken from up?
    //  
    void RotateHead()
    {
        //Vector3 angle = new Vector3(0, 0, 0);
        
        // Axis processing HERE. Probably the most heavy-duty. Especially since forV's being calculated here.
        // up to this point angle should be constructed FLAWLESSLY.

        // DEFAULT
        if (angle == new Vector3(0, 0, 0))
        {
            forV = Snake[0].transform.forward;
        }

        // default AD
        // Ok, wait. Doesn't this need a bit of recognition itself? What is _angle_?... If it works by forward, then it's ok.
        else if (angle[1] > 0)
            forV = Snake[0].transform.right;
        else if (angle[1] < 0)
        {
            forV = Snake[0].transform.right;
            forV *= -1;
        }

        // default WS
        // rotations around either axis x or transform.right yield reverted angles...
        //  this might also need revert.
        else if (angle[0] > 0)
        {
            forV = Snake[0].transform.up;
            forV *= -1;
        }
        else if (angle[0] < 0)
        {
            forV = Snake[0].transform.up;            
        }
        
        // This is the root of double 180 on x rotate.
        // And possibly more. If our snake starts with absolutely clean angle, why not make it independent of everything but rotations?
        /*rotateTo = Quaternion.Euler(CorrectAngle(Snake[0].transform.rotation.eulerAngles.x + angle[0]),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.y + angle[1]),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.z + angle[2]));*/
        rotateTo = Quaternion.Euler(rotateTo.eulerAngles.x + angle[0],
                                    rotateTo.eulerAngles.y + angle[1],
                                    rotateTo.eulerAngles.z + angle[2]);
                
        angle = new Vector3(0, 0, 0);
    }

    // this seems to be abstracted well enough to be forgotten about for some time.
    // if I had better means to check accuracy, I'd might not have to have the axis business.
    // Probably it will only work correctly for AD rotation...
    void SmoothRotate(Quaternion finish)
    {
        int axis = 0;

        for (int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.up[i] != 0)
                axis = i;
        }

        if (rotElapsed < rotTime)
        {
            rotElapsed += Time.deltaTime;
            Snake[0].transform.rotation = Quaternion.Lerp(Snake[0].transform.rotation, finish, rotElapsed/rotTime);
        }

        // THIS IS WAY TOO FINICKY FOR 3D.
        // condition: if angle between current rotation and destined rotation is less than margin of accuracy, snap.
        if ((Mathf.Abs(Snake[0].transform.rotation.eulerAngles[axis] - finish.eulerAngles[axis]) <= rotAccuracy) && (Snake[0].transform.rotation != finish))
        //if(Vector3.Angle(Snake[0].transform.rotation.eulerAngles, finish.eulerAngles) <= 5)
        {
            
            Snake[0].transform.rotation = finish;
            
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
        
        for (int i = 1; i < Snake.Count + 1; i++)
        {
            moveTo[i] = Snake[i-1].transform.position;
        }
    }
    
    // lerp move to moveTo    
    void SmoothCrawl()
    {
        for(int i = 0; i < Snake.Count; i++)
        {
            Vector3 from = Snake[i].transform.position;
            Vector3 to   = moveTo[i];
            
            if(movElapsed < movTime)
            {
                movElapsed += Time.deltaTime;
                Snake[i].transform.position = Vector3.Lerp(from, to, movElapsed/movTime);
            }

            Vector3 diff = from - to;
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
    void CheckInputAD()
    {
        // Axis check!
        int axis = 0;
        for (int i = 0; i < 3; i++ )
        {
            if (Snake[0].transform.up[i] != 0)
                axis = i;
        }
        
        if ((KeyAD == 0) && (Input.GetAxis("Fire1") != 0) && (!blockInput))
        {
            KeyAD = Input.GetAxis("Fire1");
        }

        int sign = (int)Snake[0].transform.up[axis];
        // it's y, if transform.up y != 0.
        // Also, don't forget the sign multiple. By the value of the y, thankfully.
        if((KeyAD != 0) && (Input.GetAxis("Fire1") == 0) && (!blockInput))
        {
            if (KeyAD > 0)
            {
                angle[axis] = 90 * sign;
                // multiple how?? It's cranky!
            }
            else if (KeyAD < 0)
            { 
                angle[axis] = -90 * sign;
            }

            KeyAD = 0;
            blockInput = true;  // just where do you need to be, you fucking asshole.            
        }
    }

    void CheckInputWS()
    {
        // Axis check!
        /*
        int axis = 0;
        for (int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.right[i] != 0)
                axis = i;
        }*/

        if ((KeyWS == 0) && (Input.GetAxis("Fire2") != 0) && (!blockInput))
        {
            KeyWS = Input.GetAxis("Fire2");
        }

        //int sign = (int)Snake[0].transform.right[axis];
        // it's y, if transform.up y != 0.
        // Also, don't forget the sign multiple. By the value of the y, thankfully.
        if ((KeyWS != 0) && (Input.GetAxis("Fire2") == 0) && (!blockInput))
        {
            int axis = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Snake[0].transform.right[i] != 0)
                    axis = i;
            }
            int sign = (int)Snake[0].transform.right[axis];

            if (KeyWS > 0)
            {
                angle[axis] = -90 * sign;
                // multiple how?? It's cranky!
            }
            else if (KeyWS < 0)
            {
                angle[axis] = 90 * sign;
            }

            KeyWS = 0;
            blockInput = true;  // just where do you need to be, you fucking asshole.          
            Debug.Log("angle: " + angle);
        }
    }    

    // TO DO: move collider processing to walls, fruits, tails.
    // to do: tail should spawn where the fruit was.
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
