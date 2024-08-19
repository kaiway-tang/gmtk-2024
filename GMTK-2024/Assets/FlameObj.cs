using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameObj : MonoBehaviour
{
    [SerializeField] int intensity;
    [SerializeField] float scale, time, speed;
    // Start is called before the first frame update
    void Start()
    {
        intensity = Mathf.RoundToInt(intensity * PlayerController.GetDamageMultiplier());
        Destroy(gameObject, time);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale += Vector3.one * scale;
        transform.position += transform.up * speed;
    }

    Enemy hitEnemy;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            hitEnemy = col.GetComponent<Enemy>();
            if (hitEnemy)
            {
                hitEnemy.Burn(intensity);
            }
        }
    }
}
