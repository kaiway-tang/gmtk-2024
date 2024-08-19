using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierEnemy : GroundEnemy
{
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
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

    protected override void OnNodeReached(PathNode prevNode, PathNode newNode) 
    { 
        if (prevNode.leftNode == newNode || prevNode.rightNode == newNode)  // Walk node
        {
            state = EnemyState.Walking;
        } 
        else  // Jump node
        {

        }
    }

}
