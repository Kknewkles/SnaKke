using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    //public int[] Check(float hor, float ver)
    int intHor = 0;
    int intVer = 0;
    float hor = 0;
    float ver = 0;

    int[] controls = new int[2] {0, 0};
    
    void Update()
    {
        controls[0] = 0;
        controls[1] = 0;
    }

    public int[] Check()
    {
        intHor = 0;
        intVer = 0;
        hor = 0;
        ver = 0;
        controls[0] = 0;
        controls[1] = 0;

        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        // hor, ver
        if(hor == 0 && ver == 0)
        {
            intHor = 0;
            intVer = 0;
        }
        else if(hor != 0)
        {
            if (hor > 0)
                intHor = 1;
            if (hor < 0)
                intHor = -1;
        }
        else if(ver != 0)
        {
            if (ver > 0)
                intVer = 1;
            if (ver < 0)
                intVer = -1;
        }
        
        // fill and pass the array
        controls[0] = intHor;
        controls[1] = intVer;
        return controls;
    }
}
