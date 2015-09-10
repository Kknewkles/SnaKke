using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    /*       
    x
    right	Vector3(1, 0, 0).
    left	Vector3(-1, 0, 0).
    
    y
    up	    Vector3(0, 1, 0).
    down	Vector3(0, -1, 0).

    z
    forward	Vector3(0, 0, 1).
    back	Vector3(0, 0, -1).

    zero	Vector3(0, 0, 0).
    one	    Vector3(1, 1, 1).
    */

    private bool KeyDown = false;

    //private string key;
    //private Vector3 posMod;

    //public Vector3 pos;
    void Start()
    {
        transform.position = new Vector3 (5,0,5);
    }

	void Update()
    {
        TrackInput();
	}
    
    void TrackInput()
    {
        TrackKey("right", Vector3.right);
        TrackKey("d", Vector3.right);

        TrackKey("left", Vector3.left);
        TrackKey("a", Vector3.left);

        TrackKey("up", Vector3.forward);
        TrackKey("w", Vector3.forward);

        TrackKey("down", Vector3.back);
        TrackKey("s", Vector3.back);
    }

    void TrackKey(string k, Vector3 pMod)
    {
        if (Input.GetKeyDown(k))
        {
            if (!KeyDown)
            {
                Vector3 pos = transform.position;
                // IF MOTION CONDITION
                if( ((pos.x + pMod.x >= 0) && (pos.x + pMod.x <= 19)) && 
                    ((pos.z + pMod.z >= 0) && (pos.z + pMod.z <= 19)) /* &&
                     y */ )
                    pos += pMod;
                transform.position = pos;
                KeyDown = true;
            }
        }

        if (Input.GetKeyDown(k))
            if (KeyDown) KeyDown = false;
    }
}
