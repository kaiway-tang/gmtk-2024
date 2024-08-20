using UnityEngine;

public class HPEntity : MonoBehaviour
{
    public int HP, maxHP;

    [SerializeField] protected Transform trfm;
    public int objectID;
    [SerializeField] int deathTrauma = 40;
    [SerializeField] HPBar hpBar;

    public virtual bool IsInvulnerable => _isInvulnerable;
    private bool _isInvulnerable = false;

    public int Tier;

    [SerializeField] protected GameObject damageFX, deathFX;

    bool usingHPBar;
    protected void Start()
    {
        if (maxHP == 0) { maxHP = HP; }
        if (hpBar) { usingHPBar = true; }
    }

    protected void FixedUpdate()
    {

    }

    public virtual bool TakeDamage(int amount = 0, int sourceID = 0, bool overrideOne = false)
    {
        if (sourceID != 0 && sourceID == objectID) { return false; }
        if (IsInvulnerable) { return false; }

        HP -= amount;

        if (HP <= 0)
        {
            if (Tier < 1)
            {
                Instantiate(deathFX, trfm.position, Quaternion.identity);
                CameraManager.SetTrauma(deathTrauma);
                Destroy(gameObject);
            }
            else
            {
                Tier--;
                HP = 1;
            }
        }
        else
        {
            if (usingHPBar) { hpBar.SetHPPercentage((float)HP / maxHP); }            
            Instantiate(damageFX, trfm.position, Quaternion.identity);
        }

        return true;
    }
}
