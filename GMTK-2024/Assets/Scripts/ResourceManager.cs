using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public enum Resource
    {
        Invalid,
        Red,
        Blue,
        Yellow
    };

    Queue<Resource> materialQueue;
    Dictionary<Resource, int> numMats;

    public static ResourceManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        materialQueue = new Queue<Resource> ();
        numMats = new Dictionary<Resource, int> ();
    }

    public void AddResource(Resource resource)
    {
        if (materialQueue.Count > 5) 
        {
            numMats[materialQueue.Peek()]--;
            materialQueue.Dequeue();
        }
        materialQueue.Enqueue(resource);
        if (!numMats.ContainsKey(resource))
        {
            numMats[resource] = 0;
        }
        numMats[resource]++;
    }

    public Resource CheckCraftable() 
    {
        foreach (KeyValuePair<Resource, int> resCount in numMats) 
        { 
            if (resCount.Value >= 3)
            {
                return resCount.Key;
            }
        }
        return Resource.Invalid;
    }

    public bool HandleCraft(Resource resourceType)
    {
        Queue<Resource> newQueue = new Queue<Resource>();
        int ct = 3; 
        foreach (Resource resource in materialQueue) 
        {
            if (ct>0 && resource == resourceType) 
            {
                ct--;
                continue;
            }
            newQueue.Enqueue(resource);
        }
        materialQueue = newQueue;
        if (numMats.ContainsKey(resourceType) && ct == 0)
        {
            numMats[resourceType] -= 3;
            return true;
        }
        return false;
    }
}
