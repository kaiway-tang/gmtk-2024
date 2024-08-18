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
    protected int stunned, slowed;
    [SerializeField] protected float speed, baseSpeed;

    protected new void Start()
    {
        base.Start();        
        rb = GetComponent<Rigidbody2D>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent)
        {
            navMeshAgent.speed = baseSpeed;
        }
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
        if (slowed > 0)
        {
            slowed--;
            if (slowed < 1)
            {
                if (navMeshAgent) { navMeshAgent.speed = baseSpeed; }
                speed = baseSpeed;
            }
        }
    }

    protected bool IsStunned()
    {
        return stunned > 0;
    }

    public void SetSpeed(float percentage)
    {
        speed = baseSpeed * percentage;
        if (navMeshAgent)
        {
            navMeshAgent.speed = speed;
        }
    }

    public void Slow(int ticks)
    {
        slowed = ticks;
        SetSpeed(0.1f);
    }

    public void Stun(int ticks)
    {
        if (ticks > stunned)
        {
            stunned = ticks;
            if (navMeshAgent) { navMeshAgent.enabled = false; }
        }
    }

    float stunKnockbackFactor;
    public void TakeKnockback(float power)
    {
        if (navMeshAgent) { navMeshAgent.enabled = false; }
        stunned = Mathf.RoundToInt(power * stunKnockbackFactor);
        rb.velocity = trfm.forward * -power;
    }
}
