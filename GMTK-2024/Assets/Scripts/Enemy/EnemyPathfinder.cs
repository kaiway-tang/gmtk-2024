using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; 

class PathNode
{
    public GameObject colliderRef;
    public Vector3 position;
    public int platformId;
    public List<PathNode> edges;
}

class Platform
{
    public int platformId;
}

public class EnemyPathfinder : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] GameObject markerPrefab;

    // List<List<List<PathNode>>> nodes; 
    List<PathNode>[,] nodes;

    // Start is called before the first frame update
    void Start()
    {
        //nodes = new List<List<List<PathNode>>>();
        nodes = new List<PathNode>[20, 20];  // 20 x 20 = 400x400 spatial hash map. If we go past +- 200 there will be issues :wokege:
        IdentifyPositions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IdentifyPositions()
    {
        for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                for (int z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++)
                {
                    Vector3Int gridCoords = new Vector3Int(x, y, z);
                    TileBase tile = tilemap.GetTile(gridCoords);
                    if (tile != null) 
                    {
                        Debug.Log(tile.name);
                    } else
                    {
                        //Debug.Log($"x: {x}, y: {y}, z: {z}");
                        //Debug.Log($"World pos: {tilemap.layoutGrid.CellToWorld(gridCoords)}");

                        if (y > tilemap.cellBounds.min.y && tilemap.GetTile(new Vector3Int(x, y-1, z)) != null) {  // is position 
                            GameObject colobj = Instantiate(markerPrefab, tilemap.layoutGrid.CellToWorld(gridCoords), Quaternion.identity);
                            PathNode node = new PathNode();
                            node.colliderRef = colobj;
                            node.position = colobj.transform.position;
                            int xind = (int) Mathf.Floor(colobj.transform.position.x / 20) + 10;
                            int yind = (int) Mathf.Floor(colobj.transform.position.y / 20) + 10;
                            if (nodes[xind, yind] == null)
                            {
                                nodes[xind,yind] = new List<PathNode>();
                            }
                            nodes[xind, yind].Add(node); 
                        }
                    }
                    
                }
            }
        }
    }
}
