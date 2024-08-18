using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface ISmartEnemy
{
    public void SetTarget(Vector2 position); 
    public int EvaluatePosition(Vector2 position);  
}

public class Enemy : HPEntity
{
    NavMeshAgent navMeshAgent;
    Rigidbody2D rb;
    protected int stunned;

    protected new void Start()
    {
        base.Start();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        if (stunned > 0)
        {
            stunned--;
            if (stunned < 1)
            {
                if (navMeshAgent) { navMeshAgent.enabled = true; }
            }
        }
    }

    protected bool IsStunned()
    {
        return stunned > 0;
    }

    [SerializeField] float stunKnockbackFactor;
    public void TakeKnockback(float power)
    {
        if (navMeshAgent) { navMeshAgent.enabled = false; }
        stunned = Mathf.RoundToInt(power * stunKnockbackFactor);
        rb.velocity = trfm.forward * -power;
    }
}
