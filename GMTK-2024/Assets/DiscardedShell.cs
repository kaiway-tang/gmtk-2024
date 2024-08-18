using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardedShell : MonoBehaviour
{
    [SerializeField] int type;
    [SerializeField] GameObject[] explosion;
    const int GUNNER = 0, REAPER = 1, CONTROLLER = 2;
    int timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * 5);
        timer++;

        if (type == GUNNER)
        {
            if (timer == 30)
            {
                Instantiate(explosion[PlayerController.self.tier], transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else if (type == REAPER)
        {

        }
        else if (type == CONTROLLER)
        {

        }
    }
}
