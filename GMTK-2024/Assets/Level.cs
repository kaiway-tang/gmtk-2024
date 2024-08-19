using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform[] spawnPoints;

    public Transform[] keyNodes;
    public Transform[] gatherNodes;
    public Transform topBound, keyPos;

    public Transform[] doors;
    [SerializeField] Transform[] hazardStripes;
    [SerializeField] float stripeSpeed;
    [SerializeField] SpriteRenderer clearBarRend;
    [SerializeField] Color clearBarCol;

    public void CloseDoors()
    {
        doorTimer = 20;
    }
    public void LevelCleared()
    {
        for (int i = 0; i < hazardStripes.Length; i++)
        {
            Destroy(hazardStripes[i].gameObject);
        }
        levelClear = true;
    }

    private void Start()
    {
        clearBarCol = clearBarRend.color;
    }

    int doorTimer;
    bool levelClear;
    float clearBarTimer;
    [SerializeField] float strobeRate;
    private void FixedUpdate()
    {
        if (levelClear)
        {
            clearBarTimer += strobeRate;
            clearBarTimer %= 6.28f;

            clearBarCol.a = Mathf.Sin(clearBarTimer) * 0.25f + 0.5f;
            clearBarRend.color = clearBarCol;
        }

        if (doorTimer > 0)
        {
            doorTimer--;
            doors[0].position += Vector3.right * 0.15f;
            doors[1].position -= Vector3.right * 0.15f;
        }

        for (int i = 0; i < hazardStripes.Length; i++)
        {
            hazardStripes[i].position += Vector3.right * stripeSpeed;
            if (hazardStripes[i].localPosition.x > 45.5f)
            {
                hazardStripes[i].localPosition -= Vector3.right * 30.97f * 3;
            }
        }
    }
}
