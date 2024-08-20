using UnityEngine;

public class Skirmisher : Enemy
{
    [SerializeField] protected float minRange, maxRange, fleeSpeed, approachSpeed, navSpeed;

    [SerializeField] int status;
    const int APPROACHING = 0, FLEEING = 1, REST = 2;
    [SerializeField] NavMeshController navController;

    Vector3 prevPos;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        status = FLEEING;
        bestRayHit.distance = 0;
        navController.trfm.parent = null;
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();
        sqrDist = (transform.position - PlayerController.self.transform.position).sqrMagnitude;

        if (!IsStunned())
        {
            if (status == APPROACHING)
            {
                trfm.right = trfm.position - prevPos;
                prevPos = trfm.position;

                rb.velocity += (Vector2)(navController.trfm.position - trfm.position).normalized * approachSpeed;
                if ((navController.trfm.position - trfm.position).sqrMagnitude > 2) { navController.Recover(trfm.position); }

                if (WithinDist(maxRange) && PlayerVisible())
                {
                    status = REST;
                    navController.Disable();
                }
            }
            else if (!WithinDist(maxRange) || !PlayerVisible())
            {
                status = APPROACHING;
                navController.Recover(trfm.position);
                navController.Enable(navSpeed);
            }

            if (status == FLEEING)
            {
                LerpFacePlayer(trfm, 0.1f);

                if (fleeCastTimer > 0) { fleeCastTimer--; }
                else
                {
                    fleeCastTimer = 15;
                    fleeDir = GetFleeDir();
                }

                rb.velocity += (Vector2)fleeDir * fleeSpeed;

                if (!WithinDist(minRange) || !PlayerVisible())
                {
                    status = REST;
                }
            }
            else if (WithinDist(minRange) && PlayerVisible())
            {
                status = FLEEING;
            }
        }
    }

    [SerializeField] Transform emptyTrfm;
    Vector3 playerDir, fleeDir;
    RaycastHit2D rayHit, bestRayHit;
    float castScore, maxCastScore;
    int fleeCastTimer;

    Vector3 GetFleeDir()
    {
        playerDir = PlayerController.self.transform.position - trfm.position;

        emptyTrfm.up = -playerDir;
        emptyTrfm.Rotate(Vector3.forward * -135);
        maxCastScore = 0;

        for (int i = 0; i < 7; i++)
        {
            rayHit = Physics2D.Raycast(emptyTrfm.position, emptyTrfm.up, 15, Tools.terrainMask);

            castScore = rayHit.distance * (Vector3.Dot(-playerDir.normalized, emptyTrfm.up) + 2);
            if (castScore > maxCastScore)
            {
                maxCastScore = castScore;
                bestRayHit = rayHit;
            }

            emptyTrfm.Rotate(Vector3.forward * 45);
        }

        bestRayHit.distance--;

        return (bestRayHit.point - (Vector2)trfm.position).normalized;
    }

    float sqrDist = 0;
    bool WithinDist(float dist)
    {
        return sqrDist < dist * dist;
    }

    private void OnDestroy()
    {
        Destroy(navController.gameObject);
    }
}
