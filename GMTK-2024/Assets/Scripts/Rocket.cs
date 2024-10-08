using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float speed, acceleration, maxSpeed;
    Transform trfm;

    bool started;
    // Start is called before the first frame update
    void Start()
    {
        if (started) { return; }
        trfm = transform;
        started = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (speed < maxSpeed) { speed += acceleration; }
        trfm.position += trfm.up * speed;
    }

    HPEntity hpEntity;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!started) { Start(); }

        if (col.gameObject.layer == 8)
        {
            Instantiate(explosion, trfm.position, Quaternion.identity);
            Destroy(gameObject);
        }
        if (col.gameObject.layer == 7)
        {
            hpEntity = col.GetComponent<HPEntity>();
            if (hpEntity && hpEntity.objectID != 1)
            {
                Instantiate(explosion, trfm.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
