using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landminer : Skirmisher
{
    [SerializeField] GameObject landmine;
    new void Start()
    {
        base.Start();
    }

    [SerializeField] int cooldown;
    new void FixedUpdate()
    {
        base.FixedUpdate();

        if (fleeTimer > 0)
        {
            fleeTimer--;
            if (fleeTimer == 0)
            {
                minRange = 7;
                maxRange = 8;
            }
        }

        if (status == FLEEING)
        {
            if (cooldown > 0) { cooldown--; }
            else
            {
                cooldown = 50;
                Instantiate(landmine, trfm.position, Quaternion.identity);
            }
        }
    }

    int fleeTimer;
    public override bool TakeDamage(int amount = 0, int sourceID = 0, bool overrideOne = false)
    {
        bool result = base.TakeDamage(amount, sourceID, overrideOne);
        if (result)
        {
            fleeTimer = 250;
            status = HARD_FLEE;
            minRange = 99;
            maxRange = 99;
        }

        return result;
    }
}
