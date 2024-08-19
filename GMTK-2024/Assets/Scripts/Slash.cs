using UnityEngine;

public class Slash : MonoBehaviour, IAttack
{
    public void Initialize(Transform relative, int damage, float velocity)
    {
        throw new System.NotImplementedException();
    }

    [SerializeField] int damage;
    [SerializeField] int sourceID;
    [SerializeField] SpriteRenderer _sprite;

    private Vector3 _maxLocalScale;

    private void Start()
    {
        Destroy(gameObject, 0.3f);
        damage = Mathf.RoundToInt(damage * PlayerController.GetDamageMultiplier());
        _maxLocalScale = transform.localScale * PlayerController.GetRelativeScaleFactor();
        transform.localScale = Vector2.zero;
    }

    HPEntity hitEntity;
    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            hitEntity = col.GetComponent<HPEntity>();
            if (hitEntity)
            {
                hitEntity.TakeDamage(damage, sourceID);
            }
        }
    }

    int _timer = 0;
    void FixedUpdate()
    {
        _timer++;
        if (_timer <= 10)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _maxLocalScale, 0.2f);
            transform.position += transform.up * 0.1f * (2 / PlayerController.GetRelativeScaleFactor());
        }
        else
        {
            _sprite.color = Color.Lerp(_sprite.color, Color.clear, 0.2f);
        }
    }

}
