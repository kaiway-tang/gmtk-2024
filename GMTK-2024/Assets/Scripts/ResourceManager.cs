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
    [SerializeField] ResourceDrop resourceDropPrefab;
    [SerializeField] SpriteRenderer[] resourceSprites;

    private void Start()
    {
        materialQueue = new Queue<Resource>();
        numMats = new Dictionary<Resource, int>();
    }

    #region Inventory Management
    public void AddResource(Resource resource)
    {
        if (materialQueue.Count >= 5)
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
        OnUpdate();
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
            if (ct > 0 && resource == resourceType)
            {
                ct--;
                continue;
            }
            newQueue.Enqueue(resource);
        }
        if (numMats.ContainsKey(resourceType) && ct == 0)
        {
            numMats[resourceType] -= 3;
            materialQueue = newQueue;
            OnUpdate();
            return true;
        }
        return false;
    }
    #endregion Inventory Management

    #region Utils
    public void SpawnResource(Resource resource, Vector2 position)
    {
        if (resource == Resource.Invalid) return;

        ResourceDrop resourceDrop = GameObject.Instantiate(resourceDropPrefab, position, Quaternion.identity);
        resourceDrop.SetResource(resource, resourceSprites[(int)resource]);
    }
    public Sprite GetResourceSprite(Resource resource)
    {
        return resourceSprites[(int)resource].sprite;
    }
    public Color GetResourceColor(Resource resource)
    {
        return resourceSprites[(int)resource].color;
    }
    private void OnUpdate()
    {
        Debug.Log("inventory updated!");
        GameManager.DisplayManager.UpdateIcons(materialQueue.ToArray());
    }
    #endregion Utils
}
