using UnityEngine;

public class Bullet : MonoBehaviour, IAttack
{
    public void Initialize(Transform relative, int damage, float velocity)
    {
        throw new System.NotImplementedException();
    }

    [SerializeField] int damage;
    [SerializeField] float velocity;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.up * velocity;
    }
}
