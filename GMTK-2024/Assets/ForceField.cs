using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : HPEntity
{
    [SerializeField] float speed, knockback;
    [SerializeField] float range;
    [SerializeField] Transform shieldTrfm;
    [SerializeField] int contactDamage;

    int status;
    const int TRAVELING = 0, DEPLOYING = 1, DEPLOYED = 2;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        range = Vector3.Distance(PlayerController.self.mousePos, transform.position);

        contactDamage = Mathf.RoundToInt(contactDamage * PlayerController.GetDamageMultiplier());
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();

        if (status == TRAVELING)
        {
            transform.position += transform.up * speed;
            range -= speed;
            if (range < speed) { status = DEPLOYING; }
        }
        else if (status == DEPLOYING)
        {
            if (shieldTrfm.localScale.x < 1)
            {
                shieldTrfm.localScale += Vector3.right * 0.08f;
            }
            else
            {
                shieldTrfm.localScale = Vector3.one;
                status = DEPLOYED;
            }
        }
    }

    Enemy hitEnemy;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            hitEnemy = col.GetComponent<Enemy>();
            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(contactDamage);
                hitEnemy.Slow(100);
                TakeDamage(1);
            }            
        }        
    }
}
