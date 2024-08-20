using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPositioner : MonoBehaviour
{
    List<Tuple<PathNode, float>> availableNodes;
    HashSet<PathNode> ownedNodes;
    Transform playerTrfm;
    [SerializeField] Transform playerOverride;
    [SerializeField] float scanRange = 5f;

    public static EnemyPositioner Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        availableNodes = new List<Tuple<PathNode, float>>();
        ownedNodes = new HashSet<PathNode>();
        if (playerOverride == null)
        {
            playerTrfm = GameManager.Instance.Player.transform;
        }
        else
        {
            playerTrfm = playerOverride;
        }
        if (playerTrfm != null)
        {
            StartCoroutine(ScanForAttackNodes());
        } else
        {
            Debug.LogError("Player not found, cannot perform positioning.");
        }
    }

    private static int CompareNodes(Tuple<PathNode, float> a, Tuple<PathNode, float> b)
    {
        float diff = a.Item2 - b.Item2;
        return diff < 0 ? 1 : -1;  // Sort descending
    }

    IEnumerator ScanForAttackNodes()
    {
        while (true)
        {
            LayerMask mask = LayerMask.GetMask("Pathfinding");
            RaycastHit2D[] hits = Physics2D.CircleCastAll(playerTrfm.position, scanRange, Vector2.up, 0.04f, mask);
            availableNodes.Clear();
            Debug.Log($"Num hits: {hits.Length}");
            // find ones able to hit player 
            foreach(var hit in hits)
            {
                Vector3 gunPos = hit.transform.position + 0.3f * Vector3.up;
                RaycastHit2D[] trajectory = Physics2D.RaycastAll(gunPos, (playerTrfm.position - gunPos).normalized, scanRange);

                PathNode node = hit.transform.GetComponent<PathfindingDebugDisplay>().pathNode;
                if (trajectory.Length > 1 && trajectory[1].transform == playerTrfm && !ownedNodes.Contains(node))
                {
                    // Debug.Log($"Valid node: {node.slotId}");
                    availableNodes.Add(new Tuple<PathNode, float>(node, Vector3.Distance(playerTrfm.position, node.position)));
                }
            }
            availableNodes.Sort(CompareNodes);
            yield return new WaitForSeconds(3f);
        }
    }

    public PathNode GetClosestNode(PathNode node)
    {
        PathNode closest = null;
        int shortestPath = int.MaxValue;
        foreach(Tuple<PathNode, float> avail in availableNodes)
        {
            if (avail.Item1.optimalPaths[node.slotId].Item2 < shortestPath)
            {
                closest = avail.Item1;
                shortestPath = avail.Item1.optimalPaths[node.slotId].Item2;
            }
        }

        return closest;
    }

    public PathNode CheckoutNode()
    {
        PathNode node = availableNodes[0].Item1;
        availableNodes.RemoveAt(0);
        ownedNodes.Add(node); 
        return node;
    }

    public void ReturnNode(PathNode node)
    {
        ownedNodes.Remove(node); 
    }
}
