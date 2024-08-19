using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Key[] keys;
    [SerializeField] Transform[] keyNodes;
    [SerializeField] Level startLevel;
    [SerializeField] GameObject[] levels;
    // Start is called before the first frame update
    [SerializeField] int ejectNextTimer;
    public static LevelManager self;

    public static int keysCompleted;
    [SerializeField] NavMeshSurface navSurface;

    void Start()
    {
        SPAWNING_Start();

        self = GetComponent<LevelManager>();
        currentLevel = startLevel;
        Invoke("Init", 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SPAWNING_FixedUpdate();

        if (ejectNextTimer > 0)
        {
            ejectNextTimer--;
            if (ejectNextTimer == 60)
            {
                Destroy(currentLevel.gameObject);
            }
            if (ejectNextTimer == 20)
            {
                InstantiateRandomLevel(PlayerController.self.transform.position + Vector3.up * 15);
                navSurface.BuildNavMeshAsync();
                spawnPoints = currentLevel.spawnPoints;
            }
            if (ejectNextTimer == 0)
            {
                ScatterKeys(currentLevel);
            }

            if (ejectNextTimer < 20)
            {
                PlayerController.self.SetXVelocity(0);
            }
            else
            {
                if (PlayerController.self.rb.velocity.y < 24)
                {
                    PlayerController.self.SetYVelocity(24);
                }
            }
        }
    }


    #region SPAWNING

    [SerializeField] GameObject enemy;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int spawnRate;
    int spawnTimer;
    // Start is called before the first frame update
    void SPAWNING_Start()
    {
        spawnRate = 125;
    }

    // Update is called once per frame
    void SPAWNING_FixedUpdate()
    {
        if (ejectNextTimer > 0) { return; }
        if (spawnTimer > 0)
        {
            spawnTimer--;
        }
        else
        {
            SpawnEnemy();
            if (spawnRate > 30)
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
    #endregion

    #region LEVEL_TRANSITION

    public static void OnEject()
    {
        if (keysCompleted > 2 && PlayerController.self.transform.position.y > self.currentLevel.topBound.position.y - 4)
        {
            EjectFromLevel();
            keysCompleted = 0;
        }
    }

    public static void EjectFromLevel()
    {
        self.ejectNextTimer = 100;
    }

    void Init()
    {
        ScatterKeys(startLevel);
    }

    Level currentLevel;
    public void InstantiateRandomLevel(Vector3 pos)
    {
        currentLevel = Instantiate(levels[Random.Range(0,levels.Length)], pos, Quaternion.identity).GetComponent<Level>();
    }

    void ScatterKeys(Level level)
    {
        int[] keyNodeIDs = { -1, -1, -1 };
        keyNodes = level.keyNodes;

        keyNodeIDs[0] = Random.Range(0, keyNodes.Length);
        keys[0].Scatter(keyNodes[keyNodeIDs[0]], level.gatherNodes[0]);

        do { keyNodeIDs[1] = Random.Range(0, keyNodes.Length); }
        while (keyNodeIDs[1] == keyNodeIDs[0]);
        keys[1].Scatter(keyNodes[keyNodeIDs[1]], level.gatherNodes[1]);

        do { keyNodeIDs[2] = Random.Range(0, keyNodes.Length); }
        while (keyNodeIDs[2] == keyNodeIDs[0] || keyNodeIDs[2] == keyNodeIDs[1]);
        keys[2].Scatter(keyNodes[keyNodeIDs[2]], level.gatherNodes[2]);
    }

    #endregion
}
