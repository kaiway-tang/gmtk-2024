using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] CircleCollider2D cirCol;
    [SerializeField] int damage, activeTicks, sourceID;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Color darkenCol, fadeCol;

    [SerializeField] bool autoScale;
    int timer;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5);
        if (autoScale)
        {
            damage = Mathf.RoundToInt(damage * PlayerController.GetDamageMultiplier());
            transform.localScale *= PlayerController.GetRelativeScaleFactor();
        }        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activeTicks > 0)
        {
            activeTicks--;
            rend.sortingOrder--;
            if (activeTicks == 0)
            {
                cirCol.enabled = false;
            }
        }

        rend.color -= darkenCol;
        rend.color -= fadeCol;
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
