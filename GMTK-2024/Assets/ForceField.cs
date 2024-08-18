using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : HPEntity
{
    [SerializeField] float speed, knockback;
    [SerializeField] float range;
    [SerializeField] Transform shieldTrfm;

    int status;
    const int TRAVELING = 0, DEPLOYING = 1, DEPLOYED = 2;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        range = Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();

        if (status == TRAVELING)
        {
            transform.position += transform.up * speed;
            range -= speed;
            if (range <= 0) { status = DEPLOYING; }
        }
        else if (status == DEPLOYING)
        {
            if (shieldTrfm.localScale.x < 1)
            {
                shieldTrfm.localScale += Vector3.right * 0.04f;
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
                hitEnemy.TakeDamage(0);
                hitEnemy.TakeKnockback(knockback);
                TakeDamage(1);
            }            
        }        
    }
}
