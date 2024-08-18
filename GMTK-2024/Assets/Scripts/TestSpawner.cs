using System;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    int _timer;
    void FixedUpdate()
    {
        _timer++;

        if (_timer % 60 == 0)
        {
            int random = UnityEngine.Random.Range(1, Enum.GetValues(typeof(ResourceManager.Resource)).Length);
            if (GameManager.ResourceManager != null)
                GameManager.ResourceManager.SpawnResource((ResourceManager.Resource)random, transform.position);
        }
    }
}
