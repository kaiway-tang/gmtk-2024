using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayManager : MonoBehaviour
{
    //
    [SerializeField] private Image _crosshair;
    [SerializeField] private RectTransform _hotbar;
    [SerializeField] private Image _resourcePrefab;
    private RectTransform[] _resourceAnchors;
    private RectTransform[] _resources = new RectTransform[ResourceManager.MAX_INVENTORY_SIZE];

    void Start()
    {
        _resourceAnchors = _hotbar.GetComponentsInChildren<RectTransform>();
        _resourceAnchors = _resourceAnchors[1..]; // cuts off the parent rect transform.
    }

    // Update is called once per frame
    void Update()
    {
        // move cursor to mouse position:
        _crosshair.transform.position = Input.mousePosition;
    }

    #region Hotbar
    private Queue<Action> _pendingAnimations = new Queue<Action>();
    private bool _isAnimating = false;
    private ResourceManager.Resource[] _inventory = new ResourceManager.Resource[0];
    private static WaitForSeconds _delay = new WaitForSeconds(0.02f);
    private void FixedUpdate()
    {
        if (!_isAnimating && _pendingAnimations.Count > 0)
        {
            _isAnimating = true;
            _pendingAnimations.Dequeue().Invoke();
        }
    }
    private IEnumerator AddResourceAnimation(int oldLength, int newLength, ResourceManager.Resource resource, Vector2 pos)
    {
        // Create resource:
        Image newResource = Instantiate(_resourcePrefab, pos, Quaternion.identity, _hotbar);
        newResource.sprite = GameManager.ResourceManager.GetResourceSprite(resource);
        newResource.color = GameManager.ResourceManager.GetResourceColor(resource);
        newResource.enabled = true;
        int index = newLength - 1; // 0 -> 4
        int timer = 0;
        while (timer < 20)
        {
            // New Resource => Hotbar Slot
            newResource.rectTransform.anchoredPosition = Vector2.Lerp(newResource.rectTransform.anchoredPosition, _resourceAnchors[index].anchoredPosition, 0.2f);
            if (oldLength == newLength)
            {
                // Old Resources => Shift Left:
                for (int i = 1; i < oldLength; i++)
                {
                    _resources[i].anchoredPosition = Vector2.Lerp(_resources[i].anchoredPosition, _resourceAnchors[i - 1].anchoredPosition, 0.2f);
                }
                // Oldest Resource => Fall Down:
                _resources[0].anchoredPosition = Vector2.Lerp(_resources[0].anchoredPosition, _resources[0].anchoredPosition + new Vector2(0, -50), 0.2f);
            }
            timer++;
            yield return _delay;
        }
        // Update refs:
        if (newLength == oldLength)
        {
            // Destroy Oldest, Update _resources:
            Destroy(_resources[0].gameObject);
            for (int i = 0; i < newLength - 1; i++)
            {
                _resources[i] = _resources[i + 1];
            }
        }
        _resources[index] = newResource.rectTransform;
        _isAnimating = false;
    }
    private IEnumerator CraftMechAnimation(int r1, int r2, ResourceManager.Resource[] inventory)
    {
        RectTransform rect1 = _resources[r1];
        RectTransform rect2 = _resources[r2];
        r2 = r2 - 1; // correct for the shift after r1.
        int timer = 0;
        // Shift first:
        for (int i = 0; i < _inventory.Length; i++)
        {
            if (i == r1)
            {
                for (int j = i; j < _inventory.Length - 1; j++)
                {
                    _resources[j] = _resources[j + 1];
                }
            }
        }
        // Shift second:
        for (int i = 0; i < _inventory.Length; i++)
        {
            if (i == r2)
            {
                for (int j = i; j < _inventory.Length - 1; j++)
                {
                    _resources[j] = _resources[j + 1];
                }
            }
        }
        _resources[_inventory.Length - 1] = null;
        _resources[_inventory.Length - 2] = null;
        while (timer < 20)
        {
            // Crafted Resource => Player:
            Vector2 playerPos = Camera.main.WorldToScreenPoint(GameManager.Instance.Player.transform.position);
            rect1.position = Vector2.Lerp(rect1.position, playerPos, 0.2f);
            rect2.position = Vector2.Lerp(rect2.position, playerPos, 0.2f);
            rect1.localScale = Vector3.Lerp(rect1.localScale, Vector3.zero, 0.1f);
            rect2.localScale = Vector3.Lerp(rect2.localScale, Vector3.zero, 0.1f);


            // Other Resources => Shift left:
            for (int i = 0; i < _inventory.Length; i++)
            {
                if (_resources[i] == null) continue;
                _resources[i].anchoredPosition = Vector2.Lerp(_resources[i].anchoredPosition, _resourceAnchors[i].anchoredPosition, 0.2f);
            }
            timer++;
            yield return _delay;
        }
        // Update refs:
        Destroy(rect1.gameObject);
        Destroy(rect2.gameObject);
        _isAnimating = false;
        yield return null;

        //for (int i = 0; i < _inventory.Length; i++)
        //{
        //    if (_resources[i] != null)
        //        Destroy(_resources[i].gameObject);
        //}
        //for (int i = 0; i < _inventory.Length; i++)
        //{
        //    Image newResource = Instantiate(_resourcePrefab, _resourceAnchors[i].transform.position, Quaternion.identity, _hotbar);
        //    newResource.sprite = GameManager.ResourceManager.GetResourceSprite(inventory[i]);
        //    newResource.color = GameManager.ResourceManager.GetResourceColor(inventory[i]);
        //    newResource.enabled = true;
        //    _resources[i] = newResource.rectTransform;
        //}
        //yield return null;
        //_isAnimating = false;
    }
    public void AddResource(ResourceManager.Resource[] inventory, ResourceManager.Resource newResource, Vector2 worldPos)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Action action = () =>
        {
            StartCoroutine(AddResourceAnimation(_inventory.Length, inventory.Length, newResource, screenPos));
            _inventory = inventory;
        };
        if (_isAnimating)
        {
            _pendingAnimations.Enqueue(action);
        }
        else
        {
            _isAnimating = true; // set back to false inside animation; can't do inside action due to scoping issues
            action.Invoke();
        }
    }
    public void CraftMech(int r1, int r2, ResourceManager.Resource[] beforeCraft, ResourceManager.Resource[] afterCraft)
    {
        Action action = () =>
        {
            StartCoroutine(CraftMechAnimation(r1, r2, beforeCraft));
            _inventory = afterCraft;
        };
        if (_isAnimating)
        {
            _pendingAnimations.Enqueue(action);
        }
        else
        {
            _isAnimating = true; // set back to false inside animation; can't do inside action due to scoping issues
            action.Invoke();
        }
    }


    #endregion Hotbar
}
