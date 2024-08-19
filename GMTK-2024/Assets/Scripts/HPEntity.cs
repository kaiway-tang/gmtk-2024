using UnityEngine;

public class HPEntity : MonoBehaviour
{
    public int HP;

    [SerializeField] protected Transform trfm;
    [SerializeField] protected int objectID;

    public virtual int Tier => _tier;
    private int _tier;

    [SerializeField] protected GameObject damageFX, deathFX;

    protected void Start()
    {

    }

    protected void FixedUpdate()
    {

    }

    public virtual bool TakeDamage(int amount = 0, int sourceID = 0)
    {
        if (sourceID != 0 && sourceID == objectID) { return false; }

        HP -= amount;

        if (HP <= 0)
        {
            if (Tier < 1)
            {
                Instantiate(deathFX, trfm.position, Quaternion.identity);
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
            Instantiate(damageFX, trfm.position, Quaternion.identity);
        }

        return true;
    }
}
