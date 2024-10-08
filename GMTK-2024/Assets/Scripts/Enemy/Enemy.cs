using UnityEngine;
using UnityEngine.AI;

public interface ISmartEnemy
{
    public void SetTarget(Vector2 position);
    public int EvaluatePosition(Vector2 position);
}

public class Enemy : MobileEntity
{
    NavMeshAgent navMeshAgent;
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
        if (usePlayerVisible)
        {
            if (playerVisibleTimer > 0) { playerVisibleTimer--; }
            else
            {
                playerVisibleTimer = 5;
                lastPlayerVisible = !Physics2D.Linecast(trfm.position, PlayerController.self.transform.position, Tools.terrainMask);
            }
        }
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
                burnStacks -= 6;
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
        firePtcls.startSize = burnStacks / 22f + 0.5f;
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

    public void LerpFacePlayer(Transform pTrfm, float rate)
    {
        Tools.LerpRotation(pTrfm, PlayerController.self.transform.position, rate);
    }

    int playerVisibleTimer;
    bool usePlayerVisible, lastPlayerVisible;
    public bool PlayerVisible(bool oneOff = false)
    {
        if (!usePlayerVisible && !oneOff)
        {
            usePlayerVisible = true;
            return !Physics2D.Linecast(trfm.position, PlayerController.self.transform.position, Tools.terrainMask);
        }
        return lastPlayerVisible;
    }
}
