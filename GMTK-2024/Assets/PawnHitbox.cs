using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnHitbox : MonoBehaviour
{
    [SerializeField] HPEntity hpScript;

    PlayerController playerScript;
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            playerScript = col.GetComponent<PlayerController>();
            if (playerScript)
            {
                playerScript.TakeDamage(1, 0);
                hpScript.TakeDamage(999, 0);
            }
        }
    }
}
