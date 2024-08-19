using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; 

public class PathNode
{
    public int slotId; 
    public GameObject colliderRef;
    public Vector3 position;
    public int platformId;
    public PathNode leftNode;
    public PathNode rightNode;
    public Dictionary<int, List<PathNode>> edges;
    public bool leftEdge;
    public Dictionary<int, Tuple<PathNode, int>> optimalPaths;
}

class Platform
{
    public int platformId;
}

public class EnemyPathfinder : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] GameObject markerPrefab;

    Dictionary<int, List<PathNode>> platformLinks;
    Dictionary<int, PathNode> nodeDict;
    
  
    List<PathNode>[,] nodesMap;
    List<PathNode> edges;
    PathNode[,] nodes;

    // Computed paths
    Dictionary<Tuple<int, int>, List<PathNode>> optimalPaths;

    public static EnemyPathfinder Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        platformLinks = new Dictionary<int, List<PathNode>>();
        nodeDict = new Dictionary<int, PathNode>();
        optimalPaths = new Dictionary<Tuple<int, int>, List<PathNode>>();
        edges = new List<PathNode>();
        nodes = new PathNode[tilemap.cellBounds.max.x - tilemap.cellBounds.min.x, tilemap.cellBounds.max.y - tilemap.cellBounds.min.y];
        nodesMap = new List<PathNode>[40, 40];  // 40 x 10 = 400 long & wide spatial hash map. If we go past +- 200 there will be issues :wokege:
        IdentifyPositions();
        PrecomputePaths();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IdentifyPositions()
    {
        int curSlot = 0;
        for (int z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++)
        {
            int xmin = tilemap.cellBounds.min.x;
            int ymin = tilemap.cellBounds.min.y;
            int xmax = tilemap.cellBounds.max.x;
            int ymax = tilemap.cellBounds.max.y;
            int xrange = xmax - xmin;
            int yrange = ymax - ymin;
            // int array default 0, empty space should be -1
            int[,] memo = new int[xrange, yrange];
            int curIsland = 1;
            for (int x = 0; x < xrange; x++)
            {
                for (int y = 0; y < yrange; y++)
                {
                    if (memo[x, y] != 0)
                    {
                        continue;
                    }
                    // Get tile, check contents
                    Vector3Int gridCoords = new Vector3Int(x + xmin, y + ymin, z);
                    TileBase tile = tilemap.GetTile(gridCoords);
                    // If empty, mark and continue. Otherwise, BFS
                    if (tile == null)
                    {
                        memo[x, y] = -1;
                        continue;
                    } 
                    else
                    {
                        Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
                        stack.Push(new Tuple<int,int>(x, y));
                        while (stack.Count > 0)
                        {
                            Tuple<int, int> pair = stack.Pop();
                            if (memo[pair.Item1, pair.Item2] != 0)  // Already visited
                            {
                                // Debug.Log($"Visited: {pair.Item1}, {pair.Item2}");
                                continue;
                            }
                            if (tilemap.GetTile(new Vector3Int(pair.Item1 + xmin, pair.Item2 + ymin, z)) == null)
                            {
                                memo[pair.Item1, pair.Item2] = -1;
                                // Debug.Log($"Empty: {pair.Item1}, {pair.Item2}");
                                continue;
                            }
                            memo[pair.Item1, pair.Item2] = curIsland;  // Mark the cell 

                            if (/*pair.Item1 > xmin &&*/ tilemap.GetTile(new Vector3Int(pair.Item1 + xmin - 1, pair.Item2 + ymin, z)) != null)
                            {
                                stack.Push(new Tuple<int, int>(pair.Item1 - 1, pair.Item2));
                            }
                            if (/*pair.Item1 < xmax - 1 &&*/ tilemap.GetTile(new Vector3Int(pair.Item1 + xmin + 1, pair.Item2 + ymin, z)) != null)
                            {
                                stack.Push(new Tuple<int, int>(pair.Item1 + 1, pair.Item2));
                            }
                            if (/*pair.Item2 > ymin &&*/ tilemap.GetTile(new Vector3Int(pair.Item1 + xmin, pair.Item2 + ymin - 1, z)) != null)
                            {
                                stack.Push(new Tuple<int, int>(pair.Item1, pair.Item2 - 1));
                            }
                            if (/*pair.Item2 < ymax - 1 &&*/ tilemap.GetTile(new Vector3Int(pair.Item1 + xmin, pair.Item2 + ymin + 1, z)) != null)
                            {
                                stack.Push(new Tuple<int, int>(pair.Item1, pair.Item2 + 1));
                            }

                        }
                        curIsland++; 
                    }
                }
            }

            Debug.Log(curIsland);

            /*
             * With the memo table, we run the same ground check and assign the nodes to its corresponding island ID (platformId) 
             */

            
            for (int x = 0; x < xrange; x++)
            {
                for (int y = 0; y < yrange; y++)
                {
                    Vector3Int gridCoords = new Vector3Int(x + xmin, y + ymin, z);
                    // TileBase tile = tilemap.GetTile(gridCoords);
                    if (memo[x, y] != -1)
                    {
                        continue;
                    }
                    else
                    {
                        //Debug.Log($"x: {x}, y: {y}, z: {z}");
                        //Debug.Log($"World pos: {tilemap.layoutGrid.CellToWorld(gridCoords)}");

                        if (y > 0 && memo[x, y-1] != -1)  // is valid position (block underneath not empty) 
                        {  
                           // Cell position based on bottom left corner, we need center
                            Vector3 spawnPos = tilemap.layoutGrid.CellToWorld(gridCoords) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
                            // Make the node
                            PathNode node = new PathNode();
                            node.slotId = curSlot;
                            GameObject colobj = Instantiate(markerPrefab, spawnPos, Quaternion.identity);
                            colobj.name = curSlot.ToString();
                            colobj.GetComponent<PathfindingDebugDisplay>().SetDebugId(memo[x, y - 1]);
                            colobj.GetComponent<PathfindingDebugDisplay>().pathNode = node;
                            node.colliderRef = colobj;
                            node.position = spawnPos;
                            node.platformId = memo[x, y - 1];
                            node.edges = new Dictionary<int, List<PathNode>>();
                            node.optimalPaths = new Dictionary<int, Tuple<PathNode, int>>();
                            bool isLeftEdge = x > 0 && memo[x - 1, y - 1] == -1;
                            bool isRightEdge = x < xrange - 1 && memo[x + 1, y - 1] == -1;
                            if (isLeftEdge) 
                            {
                                node.leftEdge = true;
                                edges.Add(node);
                            }
                            if (isRightEdge) 
                            {
                                node.leftEdge = false;
                                edges.Add(node);
                            }

                            int xind = (int)Mathf.Floor(spawnPos.x / 10) + 20;
                            int yind = (int)Mathf.Floor(spawnPos.y / 10) + 20;
                            if (nodesMap[xind, yind] == null)
                            {
                                nodesMap[xind, yind] = new List<PathNode>();
                            }
                            nodesMap[xind, yind].Add(node);
                            nodes[x, y] = node;
                            nodeDict[curSlot] = node;
                            curSlot++; 
                        }
                    }
                }
            }
            Debug.Log($"Number of left edges: {edges.Count}");
            // Second pass: identify all valid links
            for (int x = 1; x < xrange; x++)
            {
                for (int y = 0; y < yrange; y++)
                {
                    if (nodes[x, y] != null && nodes[x-1, y] != null)
                    {
                        nodes[x, y].leftNode = nodes[x - 1, y];
                        nodes[x - 1, y].rightNode = nodes[x, y];
                    }
                }
            }
            foreach (PathNode edge in edges)
            {
                // circle cast for all other points in a range
                LayerMask mask = LayerMask.GetMask("Pathfinding");
                RaycastHit2D[] targets = Physics2D.CircleCastAll(new Vector2(edge.position.x, edge.position.y), 6.5f, Vector2.up, 0.1f, mask);
                int leftModifier = edge.leftEdge ? 1 : -1;
                foreach (RaycastHit2D targ in targets)
                {
                    if (targ.point.x * leftModifier > edge.position.x * leftModifier)  // check only the left half 
                    {
                        continue;
                    } 
                    // Debug.Log(targ.point);
                    // raycast from edge to all other nodes, note all successful hits as edge links 
                    RaycastHit2D[] hits = Physics2D.RaycastAll(targ.transform.position, (new Vector2(edge.position.x - leftModifier * tilemap.cellSize.x / 2, edge.position.y) - targ.point).normalized);
                  
                    if (hits.Length > 1 && hits[1].transform == edge.colliderRef.transform)
                    {
                        // save the link as a valid link from the current edge 
                        PathNode link = targ.transform.GetComponent<PathfindingDebugDisplay>().pathNode;
                        if (!edge.edges.ContainsKey(link.platformId))
                        {
                            edge.edges[link.platformId] = new List<PathNode>(); 
                        }
                        edge.edges[link.platformId].Add(link);

                        // Do so bidirectionally
                        if (!link.edges.ContainsKey(edge.platformId))
                        {
                            link.edges[edge.platformId] = new List<PathNode>();
                        }
                        link.edges[edge.platformId].Add(edge);

                    }
                }

            }

        }
    }

    void PrecomputePaths()
    {
        foreach(PathNode node in nodeDict.Values)
        {
            Queue<Tuple<int, int, int>> queue = new Queue<Tuple<int, int, int>>();  // current, parent, path length
            HashSet<int> visited = new HashSet<int>();
            queue.Enqueue(new Tuple<int, int, int>(node.slotId, node.slotId, 0));

            while (queue.Count > 0)
            {
                // Fetch node
                var elem = queue.Dequeue();
                PathNode cur = nodeDict[elem.Item1];
                // Check it's not visited already otherwise skip
                if (visited.Contains(cur.slotId))
                {
                    continue;
                }
                // Store source and previous pointer pair in dict
                cur.optimalPaths[node.slotId] = new Tuple<PathNode, int>(nodeDict[elem.Item2], elem.Item3);
                // enqueue all connected edges
                if (cur.leftNode != null)
                {
                    queue.Enqueue(new Tuple<int, int, int>(cur.leftNode.slotId, cur.slotId, elem.Item3 + 1));
                }
                if (cur.rightNode != null)
                {
                    queue.Enqueue(new Tuple<int, int, int>(cur.rightNode.slotId, cur.slotId, elem.Item3 + 1));
                }
                foreach (List<PathNode> edges in cur.edges.Values)
                {
                    foreach(PathNode edge in edges)
                    {
                        queue.Enqueue(new Tuple<int, int, int>(edge.slotId, cur.slotId, elem.Item3 + 1));
                    }
                }
                // Update visited set 
                visited.Add(cur.slotId);
            }
        }
    }

    public PathNode GetNodeFromId(int id)
    {
        return nodeDict[id];
    }

    public void OnDrawGizmos()
    {
        if (edges == null) return;
        foreach (PathNode edge in edges)
        {
            if (edge.edges == null) continue;
            foreach (var link in edge.edges)
            {
                // Debug.Log(link.Key);
                foreach(PathNode node in link.Value)
                {
                    // Debug.Log(node.position);
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(edge.position, node.position);
                }
            }
        }

        //for (int x = 0; x < nodes.Length; x++)
        //{
        //    for (int y = 0; y < nodes.LongLength / nodes.Length; y++)
        //    {
        //        if (nodes[x, y] != null && nodes[x, y].leftNode != null)
        //        {
        //            Gizmos.color = Color.cyan;
        //            Gizmos.DrawLine(nodes[x,y].position, nodes[x,y].leftNode.position);
        //        }
        //    }
        //}

        foreach (PathNode pn in nodes)
        {
            if (pn != null && pn.leftNode != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(pn.position, pn.leftNode.position);
            }
        }
        
    }

    
}
