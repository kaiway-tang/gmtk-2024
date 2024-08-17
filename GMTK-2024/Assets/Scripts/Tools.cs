using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    static Transform emptyTrfm;
    static Vector3 vect3;

    private void Start()
    {
        emptyTrfm = transform;
    }

    public static void LerpRotation(Transform trfm, float targetz, float rate)
    {
        vect3 = trfm.eulerAngles;
        vect3.z = RotationalLerp(vect3.z, targetz, rate);
        //vect3.z += (targetz - vect3.z) * rate;
        trfm.eulerAngles = vect3;
    }

    public static void LerpRotation(Transform trfm, Vector3 targetPos, float rate, float offset = 0)
    {
        emptyTrfm.position = trfm.position;
        emptyTrfm.right = targetPos - trfm.position;
        LerpRotation(trfm, emptyTrfm.eulerAngles.z + offset, rate);
    }

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
