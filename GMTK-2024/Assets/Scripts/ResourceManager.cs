using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public enum Resource
    {
        Invalid,
        Red, // slasher
        Blue, // gunner
        Yellow // controller
    };

    Queue<Resource> materialQueue;
    Dictionary<Resource, int> numMats;
    public const int MAX_INVENTORY_SIZE = 5;
    public const int CRAFT_REQUIREMENT = 2;
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
        foreach (Resource resource in materialQueue)
        {
            if (numMats.ContainsKey(resource) && numMats[resource] >= CRAFT_REQUIREMENT)
            {
                return resource;
            }
        }
        return Resource.Invalid;
    }

    public bool HandleCraft(Resource resourceType)
    {
        if (resourceType == Resource.Invalid)
        {
            return false;
        }
        Resource[] resources = materialQueue.ToArray();
        Queue<Resource> newQueue = new Queue<Resource>();
        int r1 = -1;
        int r2 = -1;
        int ct = CRAFT_REQUIREMENT;
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
            numMats[resourceType] -= CRAFT_REQUIREMENT;
            OnCraft(r1, r2, materialQueue.ToArray(), newQueue.ToArray());
            materialQueue = newQueue;
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
    public static Resource GetRandom()
    {
        int random = UnityEngine.Random.Range(1, Enum.GetValues(typeof(ResourceManager.Resource)).Length);
        return (Resource)random;
    }
    private void OnAdd(Resource newResource, Vector2 position)
    {
        GameManager.DisplayManager.AddResource(materialQueue.ToArray(), newResource, position);
    }
    private void OnCraft(int r1, int r2, Resource[] beforeCraft, Resource[] afterCraft)
    {
        GameManager.DisplayManager.CraftMech(r1, r2, beforeCraft, afterCraft);
    }
    #endregion Utils
}
