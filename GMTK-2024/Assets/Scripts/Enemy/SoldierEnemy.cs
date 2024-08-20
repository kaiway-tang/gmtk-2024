using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierEnemy : GroundEnemy
{
    [SerializeField] float jumpCoefficient = 0.2f;
    Rigidbody2D rb;
    Collider2D colliderBox;
    // Start is called before the first frame update
    protected override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        colliderBox = GetComponent<Collider2D>();
    }

    protected override void OnStateSwitch(EnemyState prevState, EnemyState newState)
    {
        Debug.Log("State switched!");
        if (prevState == EnemyState.Walking)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        base.OnStateSwitch(prevState, newState);
    }

    protected override void OnIdle() { }

    protected override void OnWalk()
    {
        int directionMod = nextNode.position.x < transform.position.x ? -1 : 1;
        rb.velocity = new Vector2(speed * directionMod, rb.velocity.y);
    }

    protected override void OnJump() { }

    protected override void OnNodeReached(PathNode currentNode, PathNode destinationNode)
    {
        if (destinationNode == null)  // No destination 
        {
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
        if (currentNode == destinationNode)
        {
            state = EnemyState.Idling;
        }
        PathNode newNode = currentNode.optimalPaths[destinationNode.slotId].Item1;
        if (currentNode.leftNode == newNode || currentNode.rightNode == newNode)  // Walk node
        {
            state = EnemyState.Walking;

        }
        else  // Jump node
        {
            state = EnemyState.Jumping;
            StartCoroutine(JumpTowards(newNode.position, 0.4f, 0.7f));
        }
        nextNode = newNode;
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

        float vf = 2 * (destination.x - transform.position.x) / t / (1 + jumpCoefficient);
        float vi = vf * jumpCoefficient;
        float vx = vi;

        rb.velocity = new Vector2(vx, vy);
        float startTime = Time.time;
        while (startTime + t > Time.time)
        {
            vx = Mathf.Lerp(vf, vi, (startTime + t - Time.time) / t);
            rb.velocity = new Vector2(vx, rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }

        colliderBox.isTrigger = false;
        // Reset the gravity adjustment for this jump
        rb.gravityScale = origGrav;
    }

}
