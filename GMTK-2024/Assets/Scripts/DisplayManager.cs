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
    private IEnumerator AddResource(int oldLength, int newLength, ResourceManager.Resource resource, Vector2 pos)
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
    private IEnumerator CraftMech(int r1, int r2)
    {

        // Update refs:
        _isAnimating = false;
        yield return null;
    }
    public void AddResource(ResourceManager.Resource[] inventory, ResourceManager.Resource newResource, Vector2 worldPos)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Action action = () =>
        {
            StartCoroutine(AddResource(_inventory.Length, inventory.Length, newResource, screenPos));
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
    public void CraftMech(int r1, int r2, ResourceManager.Resource[] inventory)
    {
        Debug.Log(r1 + " " + r2 + " ");
    }


    #endregion Hotbar
}
