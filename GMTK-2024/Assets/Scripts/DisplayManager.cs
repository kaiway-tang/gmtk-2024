using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayManager : MonoBehaviour
{
    //
    [SerializeField] private Image _crosshair;
    [SerializeField] private RectTransform _hotbarResources;
    private Image[] _resourceIcons;

    void Start()
    {
        _resourceIcons = _hotbarResources.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // move cursor to mouse position:
        _crosshair.transform.position = Input.mousePosition;
    }

    #region Hotbar
    private enum AnimationType
    {
        CRAFT,
        ADD,
    }
    // Queue animations for the hotbar:
    private Queue<IEnumerator> _pendingAnimations = new Queue<IEnumerator>();
    private Coroutine _activeAnimation;
    private void FixedUpdate()
    {
        if (_activeAnimation == null && _pendingAnimations.Count > 0)
        {
            _activeAnimation = StartCoroutine(_pendingAnimations.Peek());
        }
    }
    private void QueueAnimation(AnimationType type)
    {
        if (_activeAnimation == null)
        {
            _activeAnimation = StartCoroutine(HotbarAdd());
        }
        else
        {
            _pendingAnimations.Enqueue(HotbarAdd());
        }
    }
    private IEnumerator AnimateHotbar(AnimationType type)
    {
        int timer = 0;
        while (timer < 50)
        {
            if (type == AnimationType.ADD)
            {
                // Add animation:
                Debug.Log("adding");
                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                // Craft animation:
                Debug.Log("crafting");
                yield return new WaitForSeconds(0.02f);
            }
            timer++;
        }
        Debug.Log("DONE");
        _activeAnimation = null;
    }
    private IEnumerator HotbarAdd()
    {
        int timer = 0;
        while (timer < 50)
        {
            Debug.Log("adding");
            yield return new WaitForSeconds(0.02f);
            timer++;
        }
        Debug.Log("DONE");
        _activeAnimation = null;
    }

    public void UpdateIcons(ResourceManager.Resource[] inventory)
    {
        //for (int i = 0; i < _resourceIcons.Length; i++)
        //{
        //    if (i < inventory.Length)
        //    {
        //        _resourceIcons[i].sprite = GameManager.ResourceManager.GetResourceSprite(inventory[i]);
        //        _resourceIcons[i].color = GameManager.ResourceManager.GetResourceColor(inventory[i]);
        //        _resourceIcons[i].enabled = true;
        //    }
        //    else
        //    {
        //        _resourceIcons[i].enabled = false;
        //    }
        //}
        if (_resourceIcons.Length >= inventory.Length)
        {
            // Add:
            QueueAnimation(AnimationType.ADD);
        }
        else
        {
            // Remove:
            QueueAnimation(AnimationType.CRAFT);
        }
    }


    #endregion Hotbar
}
