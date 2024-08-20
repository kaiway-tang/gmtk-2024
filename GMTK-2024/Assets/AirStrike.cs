using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrike : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
    [SerializeField] GameObject explosion;
    int timer;
    
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        timer++;
        if (timer < 25)
        {
            rend.color += Color.black * 0.04f;
        }
        if (timer == 50)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
