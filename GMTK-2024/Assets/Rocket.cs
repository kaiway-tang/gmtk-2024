using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float speed, acceleration, maxSpeed;
    Transform trfm;
    // Start is called before the first frame update
    void Start()
    {
        trfm = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (speed < maxSpeed) { speed += acceleration; }
        trfm.position += trfm.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8 || col.gameObject.layer == 7)
        {
            Instantiate(explosion, trfm.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
