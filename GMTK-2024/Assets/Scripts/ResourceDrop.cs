using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    // Fields
    [SerializeField] int LifetimeTicks;
    [SerializeField] int FlickerTicks;
    [SerializeField] ResourceManager.Resource _resource = ResourceManager.Resource.Invalid;

    // Layers
    public static int Layer => LayerMask.NameToLayer("ResourceDrop");

    // Inits
    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;

    #region Interface
    public void SetResource(ResourceManager.Resource resource, SpriteRenderer renderer)
    {
        _resource = resource;
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = renderer.sprite;
        _renderer.color = renderer.color;
    }
    public ResourceManager.Resource GetResource()
    {
        return _resource;
    }
    public void OnPickup()
    {
        Destroy(this.gameObject);
    }
    #endregion Interface

    #region Utils
    bool Standalone => (gameObject.layer == ResourceDrop.Layer);
    bool IsQuitting = false;
    private void Start()
    {
        // Standalone drop:
        if (Standalone)
        {
            // Random velocity:
            _rb = GetComponent<Rigidbody2D>();
            _rb.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(6f, 10f));
        }
        // Attached to enemy:

    }
    int _deathTimer = 0;
    private void FixedUpdate()
    {
        if (!Standalone) return;
        // Flicker animation near death:
        _deathTimer++;
        if (_deathTimer >= LifetimeTicks)
        {
            Destroy(gameObject);
        }
        else if (_deathTimer > LifetimeTicks - FlickerTicks)
        {
            if (_deathTimer % 6 == 0)
            {
                _renderer.enabled = !_renderer.enabled;
            }
        }
    }

    void OnApplicationQuit()
    {
        IsQuitting = true;
    }
    private void OnDestroy()
    {
        if (!Standalone && !IsQuitting && Random.Range(0,2) == 1)
        {
            // Drop resource:
            ResourceManager.Resource drop = (_resource == ResourceManager.Resource.Invalid ? ResourceManager.GetRandom() : _resource);
            GameManager.ResourceManager.SpawnResource(drop, transform.position);
        }
    }
    #endregion Utils
}
