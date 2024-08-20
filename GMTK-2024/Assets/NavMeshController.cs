using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    [SerializeField] Transform target;
    public NavMeshAgent navAgent;
    public Transform trfm, userTrfm;
    // Start is called before the first frame update

    public void Recover(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Disable()
    {
        navAgent.speed = 0;
        disabled = true;
        navAgent.enabled = false;
        trfm.position = userTrfm.position;
        trfm.parent = userTrfm;
    }
    public void Enable(float speed)
    {
        navAgent.speed = speed;
        disabled = false;
        navAgent.enabled = true;
        trfm.parent = null;
        trfm.position = userTrfm.position;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Start()
    {
        trfm = transform;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(target.position);
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
    }

    [SerializeField] bool disabled;
    void FixedUpdate()
    {
        if (!disabled)
        {
            navAgent.SetDestination(target.position);
        }
    }
}
