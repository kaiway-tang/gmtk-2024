using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaiwayTest : MonoBehaviour
{
    [SerializeField] float dotProd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dotProd = Vector3.Dot(Vector3.up, transform.up);
    }
}
