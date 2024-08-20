using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAirStrike : MonoBehaviour
{
    [SerializeField] Transform square;
    [SerializeField] GameObject strafingRun;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] float squareYScaleRate;
    Vector3 vect3;
    int timer;
    void FixedUpdate()
    {
        vect3 = square.localScale;
        vect3.x -= 0.06f;
        vect3.y = vect3.y * squareYScaleRate;
        square.localScale = vect3;

        timer++;
        if (timer == 11)
        {
            Instantiate(strafingRun, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
