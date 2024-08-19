using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Key[] keys;
    [SerializeField] Transform[] keyNodes;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }


    void StartLevel(Level level)
    {
        int[] keyNodeIDs = { -1, -1, -1 };
        keyNodes = level.keyNodes;

        keyNodeIDs[0] = Random.Range(0, keyNodes.Length);
        keys[0].targetNode = keyNodes[keyNodeIDs[0]];

        do { keyNodeIDs[1] = Random.Range(0, keyNodes.Length); }
        while (keyNodeIDs[1] == keyNodeIDs[0]);
        keys[1].targetNode = keyNodes[keyNodeIDs[1]];

        do { keyNodeIDs[2] = Random.Range(0, keyNodes.Length); }
        while (keyNodeIDs[2] == keyNodeIDs[0] || keyNodeIDs[2] == keyNodeIDs[1]);
        keys[2].targetNode = keyNodes[keyNodeIDs[2]];
    }
}
