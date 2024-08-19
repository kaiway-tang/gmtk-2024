using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int status;
    const int ENTRANCE = 0, SCATTERING = 1, IDLE = 2, GATHERING = 3, REST = 4;
    public Transform targetNode, gatherNode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (status == SCATTERING)
        {
            transform.position += (targetNode.position - transform.position).normalized * speed;
            if ((targetNode.position - transform.position).sqrMagnitude < 2)
            {
                status = IDLE;
            }
        }
        if (status == IDLE)
        {
            if ((PlayerController.self.transform.position - transform.position).sqrMagnitude < 1)
            {
                status = GATHERING;
                LevelManager.OnKeyCollect();
            }
        }
        if (status == GATHERING)
        {
            transform.position += (gatherNode.position - transform.position).normalized * speed;
            if ((gatherNode.position - transform.position).sqrMagnitude < 2)
            {
                transform.position = gatherNode.position;
                status = REST;
            }
        }
    }

    public void Scatter(Transform target, Transform gather)
    {
        targetNode = target;
        gatherNode = gather;
        status = SCATTERING;
    }
}
