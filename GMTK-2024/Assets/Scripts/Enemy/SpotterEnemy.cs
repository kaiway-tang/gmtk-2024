using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotterEnemy : GroundEnemy
{
    [SerializeField] float jumpCoefficient = 0.2f;
    [SerializeField] float acceptableRange = 0.5f;
    [SerializeField] float timeUntilRecovery = 2f;
    [SerializeField] float fireDelay = 3f;
    [SerializeField] float gunOffset = 0.8f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunRef;
    [SerializeField] Transform spriteRef;
    [SerializeField] Transform playerOverride;

    float timeCounter = 0f;
    float lastArrivalTime = 0f;
    float fireCounter = 0f; 

    Rigidbody2D rb;
    [SerializeField] Collider2D colliderBox;
    Transform playerTrfm;
    // Start is called before the first frame update
    protected override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        // colliderBox = GetComponent<Collider2D>();
        if (playerOverride == null)
        {
            playerTrfm = GameManager.Instance.Player.transform;
        } else
        {
            playerTrfm = playerOverride;
        }
        
    }

    protected override void OnStateSwitch(EnemyState prevState, EnemyState newState)
    {
        // Debug.Log($"State switched! Now: {newState}");
        if (prevState == EnemyState.Walking)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        } else if (prevState == EnemyState.Idling && newState != EnemyState.Idling)
        {
            StopAllCoroutines();
        } else if (prevState == EnemyState.Jumping)
        {
            rb.velocity = Vector2.zero;
            colliderBox.isTrigger = false;
        }
        if (newState == EnemyState.Idling && prevState != EnemyState.Idling)
        {
            // StartCoroutine(ShootOnSight());
            timeCounter = 0f;
            fireCounter = 0f;
        }
        if (newState == EnemyState.Recovering)
        {
            rb.velocity = Vector2.zero;
        }
        base.OnStateSwitch(prevState, newState);
    }

    protected override void OnIdle() 
    {
        if (timeCounter > 3f)
        {
            QueryNewNode();
            timeCounter = 0;
            return;
        }
        // Face player 
        float xscale = playerTrfm.position.x < transform.position.x ? 1 : -1;
        if (playerTrfm.position.x != transform.position.x)
            spriteRef.localScale = new Vector3(xscale, 1, 1);

        // Raycast towards player
        Vector3 gunPos = transform.position + Vector3.up * gunOffset;
        LayerMask mask = ~LayerMask.GetMask("Pathfinding");  // All but the Pathfinding layer 
        gunRef.up = Vector3.Lerp(gunRef.up, (playerTrfm.position - gunPos).normalized, 0.4f);
        if (gunRef.rotation.eulerAngles.z > 180)
        {
            gunRef.localScale = new Vector3(-1, 1, 1);  // Not the best idea but works
        } else
        {
            gunRef.localScale = new Vector3(1, 1, 1);
        }

        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
            return;
        }

        // RaycastHit2D hit = Physics2D.CircleCast(gunPos, 0.05f, (playerTrfm.position - gunPos).normalized, Mathf.Infinity, mask);
        RaycastHit2D hit = Physics2D.Linecast(gunPos, playerTrfm.position, Tools.terrainMask);
        if (hit.transform != null)
            Debug.Log($"Aiming: {hit.transform.name}");
        // bool oldCheck = Vector2.Distance(hit.point, new Vector2(playerTrfm.position.x, playerTrfm.position.y)) <= acceptableRange;
        if (hit.transform == null)  // Shot valid
        {
            
            Debug.Log("Firing");
            GameObject shot = Instantiate(bullet, gunRef.position + gunRef.transform.up * 0.5f, Quaternion.identity);
            shot.transform.up = (playerTrfm.position - shot.transform.position).normalized;
            timeCounter = 0;
            fireCounter = fireDelay;
        }
        else
        {
            timeCounter += Time.deltaTime;
        }
        
    }

    protected override void OnWalk()
    {
        int directionMod = nextNode.position.x < transform.position.x ? -1 : 1;
        rb.velocity = new Vector2(speed * directionMod, rb.velocity.y);
        transform.localScale = new Vector3(directionMod, transform.localScale.y, transform.localScale.z);
        lastArrivalTime += Time.deltaTime;
        if (lastArrivalTime > timeUntilRecovery)
        {
            state = EnemyState.Recovering;
        }
    }

    protected override void OnJump() 
    {
        lastArrivalTime += Time.deltaTime;
        if (lastArrivalTime > timeUntilRecovery)
        {
            state = EnemyState.Recovering;
        }
    }


    protected override void OnNodeReached(PathNode currentNode, PathNode destinationNode)
    {
        if (destinationNode == null)  // No destination 
        {
            QueryNewNode();
            // patrol on current platform
            if (currentNode.leftNode != null)
            {
                state = EnemyState.Walking;
                nextNode = currentNode.leftNode;
            }
            else if (currentNode.rightNode != null)
            {
                state = EnemyState.Walking;
                nextNode = currentNode.rightNode;
            }
            return;
        }
        PathNode newNode = currentNode.optimalPaths[destinationNode.slotId].Item1;
        if (currentNode == destinationNode)
        {
            state = EnemyState.Idling;
        } else if (currentNode.leftNode == newNode || currentNode.rightNode == newNode)  // Walk node
        {
            state = EnemyState.Walking;

        }
        else  // Jump node
        {
            state = EnemyState.Jumping;
            StopAllCoroutines();
            StartCoroutine(JumpTowards(newNode.position, 0.4f, 0.7f));
        }
        nextNode = newNode;
        lastArrivalTime = 0f;
    }

    void QueryNewNode()
    {
        if (targetNode != null)
        {
            EnemyPositioner.Instance.ReturnNode(targetNode);
        }
        if (nearestNode != null)
        {
            PathNode newTarg = EnemyPositioner.Instance.CheckoutNode();
            if (newTarg != null)
            {
                Debug.Log($"Repositioning to: {newTarg.slotId}");
                SetTarget(newTarg.position);
            }
        }
    }

    
    IEnumerator JumpTowards(Vector3 destination, float heightOffset, float time)
    {
        rb.velocity = Vector2.zero;

        colliderBox.isTrigger = true;  // Phase through colliders

        float ct = time / 10;

        // Find acceleration
        // float rt = Mathf.Max(0.2f, time - ct);
        float rt = time - ct;
        float maxHeight = Mathf.Max(transform.position.y, destination.y) + heightOffset;

        // Find vertical velocity needed to reach the max height (destination.y + heightOffset)
        float g = Physics2D.gravity.magnitude * rb.gravityScale;  // magnitude of acceleration due to gravity
        float vy = Mathf.Sqrt(2 * g * (maxHeight - transform.position.y));
        float t1 = vy / g;

        // Then find horizontal velocity needed to reach the end 
        float t2 = Mathf.Sqrt(2 * (maxHeight - destination.y) / g);

        // Figure out the necessary gravity adjustment to reduce the time to the desired jump duration
        float t = t1 + t2;
        float origGrav = rb.gravityScale;
        rb.gravityScale = t / rt * origGrav;

        // Recalculate everything according to new gravity
        g = Physics2D.gravity.magnitude * rb.gravityScale;
        vy = Mathf.Sqrt(2 * g * (maxHeight - transform.position.y));
        t1 = vy / g;
        t2 = Mathf.Sqrt(2 * (maxHeight - destination.y) / g);
        t = t1 + t2;

        float vavg = (destination.x - transform.position.x) / t;
        // Rise...
        float x1 = vavg * t1;
        float vm = 2 * x1 / t1 / (1 + jumpCoefficient);
        float vi = vm * jumpCoefficient;
        // ...and fall
        float x2 = vavg * t2;
        float vf = 2 * x2 / t2 - vm;

        var vx = vi;
        rb.velocity = new Vector2(vx, vy);
        float startTime = Time.time;
        while (startTime + t1 > Time.time)
        {
            vx = Mathf.Lerp(vm, vi, (startTime + t1 - Time.time) / t1);
            rb.velocity = new Vector2(vx, rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        while (startTime + t > Time.time)
        {
            vx = Mathf.Lerp(vf, vm, (startTime + t - Time.time) / t2);
            rb.velocity = new Vector2(vx, rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        /*
        while (Vector3.Distance(destination, transform.position) > Time.fixedDeltaTime * 5f)
        {
            rb.velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * 9f);
            yield return new WaitForFixedUpdate(); 
        }
        */
        transform.position = destination;

        //float vf = 2 * (destination.x - transform.position.x) / t / (1 + jumpCoefficient);
        //float vi = vf * jumpCoefficient;
        //float vx = vi;

        //rb.velocity = new Vector2(vx, vy);
        //float startTime = Time.time;
        //while (startTime + t > Time.time)
        //{
        //    vx = Mathf.Lerp(vf, vi, (startTime + t - Time.time) / t);
        //    rb.velocity = new Vector2(vx, rb.velocity.y);
        //    yield return new WaitForFixedUpdate();
        //}

        colliderBox.isTrigger = false;
        // Reset the gravity adjustment for this jump
        rb.gravityScale = origGrav;
    }
}
