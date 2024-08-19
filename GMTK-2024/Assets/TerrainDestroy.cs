using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDestroy : MonoBehaviour
{
    [SerializeField] GameObject baseObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            Destroy(baseObj);
        }
    }
}
