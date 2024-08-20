using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : HPEntity, ISmartEnemy
{
    public enum EnemyState
    {
        Walking,
        Jumping,
        Idling,
        Recovering
    }
    [SerializeField] int targetNodeId;
    [SerializeField] int startingNodeId;
    [SerializeField] protected float speed = 5f;
    public int nextId = -1;

    protected EnemyState state
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateSwitch(_state, value);
        }
    }
    [SerializeField] EnemyState _state = EnemyState.Idling;
    protected PathNode nearestNode;
    protected PathNode nextNode;
    protected PathNode targetNode;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();

        nextId = startingNodeId;
        state = EnemyState.Idling;
        if (targetNodeId > 0)
        {
            SetTarget(EnemyPathfinder.Instance.GetNodeFromId(targetNodeId).position);
        }
        Initialize();
    }

    protected virtual void Initialize() { }

    // Update is called once per frame
    void Update()
    {
        // If jumping, have special behavior
        switch (_state)
        {
            case EnemyState.Idling:
                OnIdle();
                break;
            case EnemyState.Walking:
                OnWalk();
                break;
            case EnemyState.Jumping:
                OnJump();
                break;
        }

        if (targetNode != null && nextNode == null && nearestNode != null)
        {
            nextNode = nearestNode;

        }
        if (nextNode != null && Vector2.SqrMagnitude(nextNode.position - transform.position) < Time.deltaTime)  // Close enough to target
        {
            nearestNode = nextNode;
            // nextNode = nextNode.optimalPaths[targetNode.slotId].Item1;
            OnNodeReached(nearestNode, targetNode);
        }
    }

    protected virtual void OnStateSwitch(EnemyState prevState, EnemyState newState)
    {
        _state = newState;
    }

    protected virtual void OnIdle() { }

    protected virtual void OnWalk() { }

    protected virtual void OnJump() { }

    protected virtual void OnNodeReached(PathNode currentNode, PathNode destinationNode) { }

    public void SetTarget(Vector2 target)
    {
        PathNode node = EnemyPathfinder.Instance.GetClosestNode(target);
        if (node == null)
        {
            Debug.LogError("No nodes near the location specified!");
            return;
        }
        targetNode = node;
        OnNodeReached(nearestNode, targetNode);
        //if (nearestNode != null)
        //{
        //    nextNode = nearestNode;
        //    state = EnemyState.Walking; // assume nearest
        //} 
    }

    public int EvaluatePosition(Vector2 position)
    {
        return 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger collided");
        PathfindingDebugDisplay waypoint = collision.transform.GetComponent<PathfindingDebugDisplay>();
        if (waypoint != null)
        {
            Debug.Log("Nearest updated");
            nearestNode = waypoint.pathNode;
            if (state == EnemyState.Recovering)
            {
                OnNodeReached(nearestNode, targetNode);
            }
        }

    }

}
