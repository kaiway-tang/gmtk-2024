using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deparenter : MonoBehaviour
{
    [SerializeField] Transform[] trfms;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < trfms.Length; i++)
        {
            trfms[i].parent = null;
        }
    }
}
