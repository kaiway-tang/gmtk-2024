using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is poorly named
public class PathfindingDebugDisplay : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] List<Color> debugColors;
    public PathNode pathNode { get; set; }
    public int debugId = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetDebugId(int id)
    {
        debugId = id;
        if (debugColors.Count > 0)
        {
            // Debug.Log(debugColors[debugId % debugColors.Count]);
            sr.color = debugColors[debugId % debugColors.Count];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
