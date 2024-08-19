using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int status;
    const int ENTRANCE = 0, SCATTERING = 1, IDLE = 2, EXIT = 3;
    public Transform targetNode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (status == SCATTERING)
        {
            transform.position += (targetNode.position - transform.position).normalized;
            if ((targetNode.position - transform.position).sqrMagnitude < 2)
            {
                status = IDLE;
            }
        }
    }

    public void Scatter()
    {
        status = SCATTERING;
    }
}
