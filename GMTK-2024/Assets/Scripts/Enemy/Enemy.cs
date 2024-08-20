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
    [SerializeField] ParticleSystem firePtcls;

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
        if (burnTick > 0) { burnTick--; }
        else
        {
            if (burnStacks > 0)
            {
                TakeDamage(burnStacks);
                burnStacks -= 4;
                SetPtclSize();
                if (burnStacks <= 0)
                {
                    burnStacks = 0;
                    firePtcls.Stop();
                }
                burnTick = 25;
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

    [SerializeField] protected int burnStacks, burnTick;
    public void Burn(int intensity)
    {
        if (burnStacks == 0) { firePtcls.Play(); }
        if (burnStacks < intensity * 10)
        {
            burnStacks += intensity;
        }
        SetPtclSize();
    }

    void SetPtclSize()
    {
        firePtcls.startSize = burnStacks / 15f + 0.5f;
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
