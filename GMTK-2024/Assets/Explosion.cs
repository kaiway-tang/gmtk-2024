using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] CircleCollider2D cirCol;
    [SerializeField] int damage, activeTicks, sourceID;
    int timer;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5);
        damage = Mathf.RoundToInt(damage * PlayerController.GetDamageMultiplier());
        transform.localScale *= PlayerController.GetRelativeScaleFactor();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activeTicks > 0)
        {
            activeTicks--;
            if (activeTicks == 0)
            {
                cirCol.enabled = false;
            }
        }
    }

    HPEntity hitEntity;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            hitEntity = col.GetComponent<HPEntity>();
            if (hitEntity)
            {
                hitEntity.TakeDamage(damage, sourceID);
            }
        }
    }
}
