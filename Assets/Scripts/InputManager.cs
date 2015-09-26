using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    int Hor = 0;
    int Ver = 0;
    float rawHorAxis = 0;
    float rawVerAxis = 0;

    int[] controls = new int[2] {0, 0};
    
    public int[] Check(int[] controls)
    {
        Hor = 0;
        Ver = 0;
        rawHorAxis = 0;
        rawVerAxis = 0;

        rawHorAxis = Input.GetAxis("Horizontal");
        rawVerAxis = Input.GetAxis("Vertical");

        if(rawHorAxis != 0)
        {
            if (rawHorAxis > 0)
                Hor = 1;
            if (rawHorAxis < 0)
                Hor = -1;
        }
        else if(rawVerAxis != 0)
        {
            if (rawVerAxis > 0)
                Ver = 1;
            if (rawVerAxis < 0)
                Ver = -1;
        }
        
        // fill and pass the array
        controls[0] = Hor;
        controls[1] = Ver;
        return controls;
    }
}
