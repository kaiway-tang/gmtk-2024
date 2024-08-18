using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPEntity : MonoBehaviour
{
    public int HP;

    [SerializeField] protected Transform trfm;
    [SerializeField] int objectID;

    public int tier;

    [SerializeField] GameObject damageFX, deathFX;

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
            if (tier < 1)
            {
                Instantiate(deathFX, trfm.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                tier--;
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
