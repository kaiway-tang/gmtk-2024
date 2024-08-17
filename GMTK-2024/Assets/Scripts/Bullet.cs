using UnityEngine;

public class Bullet : MonoBehaviour, IAttack
{
    public void Initialize(Transform relative, int damage, float velocity)
    {
        throw new System.NotImplementedException();
    }

    [SerializeField] int damage;
    [SerializeField] float velocity;
    [SerializeField] int sourceID;

    // Start is called before the first frame update
    protected void Start()
    {
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        transform.position += transform.up * velocity;
    }

    HPEntity hitEntity;
    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            Destroy(gameObject);
        }

        if (col.gameObject.layer == 7)
        {
            hitEntity = col.GetComponent<HPEntity>();
            if (hitEntity)
            {
                hitEntity.TakeDamage(damage, sourceID);
            }
            Destroy(gameObject);
        }
    }
}
