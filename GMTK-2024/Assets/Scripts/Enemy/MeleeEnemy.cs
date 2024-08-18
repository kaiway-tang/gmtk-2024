using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : Enemy
{
    [SerializeField] Transform target;

    NavMeshAgent navigation;

    Vector3 prevPos = Vector3.zero;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        navigation = GetComponent<NavMeshAgent>();
        navigation.updateRotation = false;
        navigation.updateUpAxis = false;
        prevPos = transform.position;
        target = PlayerController.self.transform;
    }

    // Update is called once per frame
    void Update()
    {
        navigation.SetDestination(target.position);
        transform.up = (transform.position - prevPos).normalized;
        prevPos = transform.position;
    }
}
