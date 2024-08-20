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

    public static int keysCompleted, levelsCompleted;
    [SerializeField] NavMeshSurface navSurface;

    [SerializeField] int timeInLevel;

    public Transform[] mappingNodes;
    [SerializeField] bool levelRunning;
    void Start()
    {
        SPAWNING_Start();

        self = GetComponent<LevelManager>();
        currentLevel = startLevel;
        navSurface.BuildNavMeshAsync();
        baseBombTime = Mathf.RoundToInt(baseBombTime / 0.9f);

        Invoke("InitStart", 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SPAWNING_FixedUpdate();
        BOMBARD_FixedUpdate();

        if (ejectNextTimer > 0)
        {
            ejectNextTimer--;
            if (ejectNextTimer == 60)
            {
                Destroy(currentLevel.gameObject);
            }
            if (ejectNextTimer == 30)
            {
                InstantiateRandomLevel(PlayerController.self.transform.position + Vector3.up * 15);                
            }
            if (ejectNextTimer == 10) { currentLevel.CloseDoors(); }
            if (ejectNextTimer == 0)
            {
                StartLevel(currentLevel);                
            }

            if (ejectNextTimer < 30)
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
        else
        {
            timeInLevel++;
        }
    }


    #region SPAWNING

    [SerializeField] GameObject enemy;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int spawnRate, levelSpawnRate, spawnTimer;
    // Start is called before the first frame update
    void SPAWNING_Start()
    {
        
    }

    [SerializeField] int enemyQueCount, targetQue, levelTargetQue;
    // Update is called once per frame
    void SPAWNING_FixedUpdate()
    {
        if (ejectNextTimer > 0 || !levelRunning) { return; }
        if (spawnTimer > 0)
        {
            spawnTimer--;
        }
        else
        {
            enemyQueCount++;
            if (enemyQueCount >= targetQue)
            {
                SetInvalidSpawn();
                for (int i = 0; i < enemyQueCount; i++)
                {
                    SpawnEnemy();
                }
                enemyQueCount -= targetQue;
                targetQue = Random.Range(Mathf.RoundToInt(levelTargetQue * 0.8f), Mathf.RoundToInt(levelTargetQue * 1.2f));
            }

            if (spawnRate > 30)
            {
                spawnRate--;
            }
            spawnTimer = spawnRate;
        }
    }

    int invalidSpawnPointID;
    void SetInvalidSpawn()
    {
        float min = 99999999;
        float sqrMag;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            sqrMag = (spawnPoints[i].position - PlayerController.self.transform.position).sqrMagnitude;
            if (sqrMag < min)
            {
                min = sqrMag;
                invalidSpawnPointID = i;
            }
        }
    }

    int spawnPointID;
    void SpawnEnemy()
    {
        do { spawnPointID = Random.Range(0, spawnPoints.Length); }
        while (spawnPointID == invalidSpawnPointID);
        Instantiate(enemy, spawnPoints[spawnPointID].position, Quaternion.identity);
    }
    #endregion

    #region LEVEL_TRANSITION

    public static void OnKeyCollect()
    {
        keysCompleted++;
        if (keysCompleted >= self.keys.Length)
        {
            self.currentLevel.LevelCleared();
        }
    }

    public static void OnEject()
    {
        if (keysCompleted > 2 && PlayerController.self.transform.position.y > self.currentLevel.topBound.position.y - 4)
        {
            EjectFromLevel();
            keysCompleted = 0;
            levelsCompleted++;
            self.timeInLevel = 0;

            self.levelRunning = false;
            self.levelTargetQue += 2;
        }
    }

    public static void EjectFromLevel()
    {
        self.ejectNextTimer = 100;        
    }

    void InitStart()
    {
        StartLevel(startLevel);
    }

    Level currentLevel;
    public void InstantiateRandomLevel(Vector3 pos)
    {
        currentLevel = Instantiate(levels[Random.Range(0,levels.Length)], pos, Quaternion.identity).GetComponent<Level>();
        navSurface.BuildNavMeshAsync();

        spawnPoints = currentLevel.spawnPoints;
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].transform.position = currentLevel.keyPos.position;
            if (i < levelsCompleted)
            {
                keys[i].UseKeyBox();
            }
        }
    }

    
    HashSet<int> keyPosIDs = new HashSet<int>();
    void StartLevel(Level level)
    {
        self.levelRunning = true;

        spawnRate = levelSpawnRate;
        levelSpawnRate = Mathf.RoundToInt(levelSpawnRate * 0.9f);
        enemyQueCount = levelTargetQue - 2;
        targetQue = levelTargetQue;

        bombardTimer = 0;
        baseBombTime = Mathf.RoundToInt(baseBombTime * 0.9f);
        bombTime = baseBombTime;

        keyPosIDs.Clear();
        keyNodes = level.keyNodes;
        int selectedNode = -1;

        for (int i = 0; i < keys.Length; i++)
        {
            do { selectedNode = Random.Range(0, keyNodes.Length); }
            while (keyPosIDs.Contains(selectedNode));
            keys[i].Scatter(keyNodes[selectedNode], level.gatherNodes[0]);
            keyPosIDs.Add(selectedNode);
        }
    }

    #endregion

    #region BOMBARDING

    [SerializeField] GameObject airStrike;
    [SerializeField] int baseBombTime, bombTime;
    [SerializeField] int bombardTimer;
    void BOMBARD_FixedUpdate()
    {
        if (!levelRunning) { return; }
        bombardTimer++;
        if (bombardTimer >= bombTime)
        {            
            bombTime = bombTime / 2;
            if (bombTime < 50) { bombTime = 50; }
            bombardTimer = 0;
            Instantiate(airStrike, PlayerController.self.posTracker.PredictedPosition(1), Quaternion.identity);
        }
    }

    #endregion
}
