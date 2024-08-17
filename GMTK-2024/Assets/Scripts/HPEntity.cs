using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPEntity : MonoBehaviour
{
    public int HP;

    [SerializeField] protected Transform trfm;
    [SerializeField] int objectID;

    [SerializeField] GameObject deathFX;

    protected void Start()
    {

    }

    protected void FixedUpdate()
    {
        
    }

    public void TakeDamage(int amount = 0, int sourceID = 0)
    {
        if (sourceID != 0 && sourceID == objectID) { return; }

        HP -= amount;

        if (HP <= 0)
        {
            Instantiate(deathFX, trfm.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
