using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    public static float RotationalLerp(float start, float dest, float rate)
    {
        if (Mathf.Abs(dest - start) < 180)
        {
            return start + (dest - start) * rate;
        }
        else
        {
            if (dest > start)
            {
                return (start + (dest - start - 360) * rate);
            }
            else
            {
                return (start + (360 - start + dest) * rate) % 360;
            }

        }
    }

}
