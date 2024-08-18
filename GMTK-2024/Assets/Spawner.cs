using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int spawnRate;
    int spawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        spawnRate = 125;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (spawnTimer > 0)
        {
            spawnTimer--;
        }
        else
        {
            SpawnEnemy();
            if (spawnRate > 50)
            {
                spawnRate--;
            }            
            spawnTimer = spawnRate;
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemy, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
    }
}
