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
    public const int MAX_INVENTORY_SIZE = 5;
    [SerializeField] ResourceDrop resourceDropPrefab;
    [SerializeField] SpriteRenderer[] resourceSprites;

    private void Start()
    {
        materialQueue = new Queue<Resource>();
        numMats = new Dictionary<Resource, int>();
    }

    #region Inventory Management
    public void AddResource(Resource resource, Vector2 position)
    {
        if (materialQueue.Count >= MAX_INVENTORY_SIZE)
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
        OnAdd(resource, position);
    }

    public Resource CheckCraftable()
    {
        foreach (KeyValuePair<Resource, int> resCount in numMats)
        {
            if (resCount.Value >= 2)
            {
                return resCount.Key;
            }
        }
        return Resource.Invalid;
    }

    public bool HandleCraft(Resource resourceType)
    {
        Resource[] resources = materialQueue.ToArray();
        Queue<Resource> newQueue = new Queue<Resource>();
        int r1 = -1;
        int r2 = -1;
        int ct = 2;
        for (int i = 0; i < resources.Length; i++)
        {
            Resource resource = resources[i];
            if (ct > 0 && resource == resourceType)
            {
                ct--;
                if (r1 == -1)
                {
                    r1 = i;
                }
                else
                {
                    r2 = i;
                }
                continue;
            }
            newQueue.Enqueue(resource);
        }
        if (numMats.ContainsKey(resourceType) && ct == 0)
        {
            numMats[resourceType] -= 3;
            materialQueue = newQueue;
            OnCraft(r1, r2);
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
    private void OnAdd(Resource newResource, Vector2 position)
    {
        Debug.Log("inventory add!");
        GameManager.DisplayManager.AddResource(materialQueue.ToArray(), newResource, position);
    }
    private void OnCraft(int r1, int r2)
    {
        Debug.Log("inventory craft!");
        GameManager.DisplayManager.CraftMech(r1, r2, materialQueue.ToArray());
    }
    #endregion Utils
}
