using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour, ISmartEnemy
{
    [SerializeField] int targetNodeId;
    [SerializeField] int startingNodeId;
    [SerializeField] float speed = 5f;
    Vector3 targetPos = Vector3.zero;
    public int nextId = -1;

    // Start is called before the first frame update
    void Start()
    {
        nextId = startingNodeId;
        targetPos = EnemyPathfinder.Instance.GetNodeFromId(nextId).position;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
        transform.position = nextPos;
    }

    public void SetTarget(Vector2 target)
    {

    }

    public int EvaluatePosition(Vector2 position)
    {
        return 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PathNode waypoint = collision.transform.GetComponent<PathfindingDebugDisplay>().pathNode;
        if (nextId == -1 || waypoint.slotId == nextId)
        {
            PathNode nextWaypoint = waypoint.optimalPaths[targetNodeId].Item1;
            nextId = nextWaypoint.slotId;
            targetPos = nextWaypoint.position;
        }

    }
}
