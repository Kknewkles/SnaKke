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


    Vector3 leAngle;    

    Vector3[] moveTo = new Vector3[10];

    Quaternion rotateTo = Quaternion.Euler(0, 0, 0);

    float rotAccuracy = 3f;
    float movAccuracy = 0.01f;

    public float turnDelay = 1f;
    public float rotTime = 2;
    public float movTime = 2;
    float rotElapsed;
    float movElapsed;
    
    private float angleX;
    private float angleY;
    private float angleZ;
    private float KeyAD, KeyWS;

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
        
        KeyAD = 0;
        KeyWS = 0;
        
        angleX = 0;
        angleY = 0;
        angleZ = 0;
        
        leAngle = new Vector3(0, 0, 0);
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

            // main cycle
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
        //Debug.Log("Key: " + Key + " AD: " + Input.GetAxis("Fire1") + " WS: " + Input.GetAxis("Fire2") + " leAngle: " + leAngle);
        //Debug.Log(" leAngle: " + leAngle);
        //Debug.Log(" forV: " + forV);

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
    //   With improved input this is almost deprecated.

    // Or wait, this not only applies rotation, but also next forward? D'oh!
    // Take care of this after input.
    void RotateHead()
    {
        

        // Axis processing HERE.
        /*
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
        */

        // clean this up into "non-zero, the value is the sign" etc.
        if (leAngle == new Vector3(0, 0, 0))
            forV = Snake[0].transform.forward;  // (0, 0, 1)
        
        else if (leAngle == new Vector3(0, 1, 0))    // around Y - left/right
            forV = Snake[0].transform.right;
        else if (leAngle == new Vector3(0, -1, 0))
        {
            forV = Snake[0].transform.right;
            forV *= -1;
        }
        else if (leAngle == new Vector3(0, 0, 1))    // around Z - up/down
            forV = Snake[0].transform.up;
        else if (leAngle == new Vector3(0, 0, -1))
        {
            forV = Snake[0].transform.up;
            forV *= -1;
        }
        


        /* is this enough to account for our up being pointed downward?
        if (transform.forward == Vector3.down)
            forV *= -1;
        */

        // Now this processes the future angle.
        /*
        rotateTo = Quaternion.Euler(CorrectAngle(Snake[0].transform.rotation.eulerAngles.x + angleX),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.y + angleY),
                                    CorrectAngle(Snake[0].transform.rotation.eulerAngles.z + angleZ));

        angleX = 0;
        angleY = 0;
        angleZ = 0;
        */

        // Remember to apply CorrectAngle later if needed
        // Either leAngle shouldn't be always reset, or it should be applied ONLY if there was input.
        //  Try resetting it only on input?
        rotateTo = Quaternion.Euler(leAngle);
        
        //leAngle = new Vector3(0, 0, 0);
    }

    // Also forgotten to fix this. Y is welded here.
    // Although, it's only on the margin check... might not be the problem.
    // Actually, if check's never accessed, leAngle isn't being reset.
    void SmoothRotate(Quaternion finish)
    {
        if (rotElapsed < rotTime)
        {
            rotElapsed += Time.deltaTime;
            Snake[0].transform.rotation = Quaternion.Lerp(Snake[0].transform.rotation, finish, rotElapsed/rotTime);
        }

        // Check angle difference with float Vector3.Angle(from, to);
        if ((Mathf.Abs(Snake[0].transform.rotation.eulerAngles.y - finish.eulerAngles.y) <= rotAccuracy) &&
            (Snake[0].transform.rotation != finish))
        {
            Snake[0].transform.rotation = finish;
            //leAngle = new Vector3(0, 0, 0);
            //Debug.Log("HALLO");
            blockInput = false;

            rotElapsed = rotTime;
        }
    }



    // CAREFUL WITH THIS. FORWARD NEEDS TO BE CORRECT, ROTATIONS MAY FUDGE UP
    void Crawl()
    {
        moveTo[0] = Snake[0].transform.position + forV;
        Debug.Log("leAngle: " + leAngle + " forV: " + forV);

        for (int i = 1; i < Snake.Count + 1; i++)
        {
            moveTo[i] = Snake[i-1].transform.position;
        }
    }
    
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
    // Introduce RotateRight/Left, LookUp/Down? Pass it le Key?

    // OOOH boy am I nervous about this one. Looks good though.
    //  Dunn output anything doe!
    
    // Yeah, slick looks don't quite cut it. :'(
    //  Now it seems to work.
    void CheckInput()
    {
        /*
        if((Key == 0) && (!blockInput) && ((Input.GetAxis("Fire1") != 0) || (Input.GetAxis("Fire2") != 0)))
        {
            if(Input.GetAxis("Fire1") != 0)
                leAngle = RotateLeft_Right(Key);
            else if(Input.GetAxis("Fire2") != 0)    // just if or else if?
                leAngle = LookUp_Down(Key);
            
            Key = 0;
            blockInput = true;
        }
        */

        if ((KeyAD == 0) && (!blockInput) && (Input.GetAxis("Fire1") != 0))
        {
            KeyAD = Input.GetAxis("Fire1");
        }

        if ((KeyAD != 0) && (!blockInput) && (Input.GetAxis("Fire1") == 0))
        {
            //leAngle = RotateLeft_Right(KeyAD);
            RotateLeft_Right(KeyAD);
            KeyAD = 0;
            blockInput = true;
        }

        if ((KeyWS == 0) && (!blockInput) && (Input.GetAxis("Fire2") != 0))
        {
            KeyWS = Input.GetAxis("Fire2");
        }

        if ((KeyWS != 0) && (!blockInput) && (Input.GetAxis("Fire2") == 0))
        {
            //leAngle = LookUp_Down(KeyWS);
            LookUp_Down(KeyWS);
            KeyWS = 0;
            blockInput = true;
        }

    }

    // A-D.
    void RotateLeft_Right(float Key)
    //Vector3 RotateLeft_Right(float Key)
    {
        leAngle = new Vector3(0, 0, 0);
        for(int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.up[i] != 0)
            {
                leAngle[i] += Snake[0].transform.up[i] * 90 * Key;
            }
        }

    }

    // W-S.
    // Seems to alter wrong axis??
    void LookUp_Down(float Key)
    //Vector3 LookUp_Down(float Key)
    {
        leAngle = new Vector3(0, 0, 0);
        for (int i = 0; i < 3; i++)
        {
            if (Snake[0].transform.right[i] != 0)
            {
                leAngle[i] += Snake[0].transform.right[i] * 90 * Key * -1;  // WS seems to be inverted. I will think later about why the hell.
            }
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
