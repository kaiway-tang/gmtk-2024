using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafingRun : MonoBehaviour
{
    [SerializeField] Transform laserLine;
    [SerializeField] float travel;
    [SerializeField] GameObject bullet;

    bool flyLeft;
    // Start is called before the first frame update
    void Start()
    {
        flyLeft = Random.Range(0,2) == 0;
        if (flyLeft) { laserLine.position += Vector3.right * travel * 20; }
        else { laserLine.position -= Vector3.right * travel * 20; }
    }

    [SerializeField] int time;
    void FixedUpdate()
    {
        time++;

        if (time < 41)
        {
            if (flyLeft) { laserLine.position -= Vector3.right * travel; }
            else { laserLine.position += Vector3.right * travel; }
        }

        if (time == 41)
        {
            Destroy(laserLine.gameObject);

            transform.up = Vector3.down;
            if (flyLeft) { transform.position += Vector3.right * travel * 20; }
            else { transform.position -= Vector3.right * travel * 20; }            
        }

        if (time > 70)
        {
            if (time % 2 == 0) { Instantiate(bullet, transform.position + Vector3.up * 20, transform.rotation); }            
            if (flyLeft) { transform.position -= Vector3.right * travel; }
            else { transform.position += Vector3.right * travel; }
        }
        if (time == 111)
        {
            Destroy(gameObject);
        }
    }
}
