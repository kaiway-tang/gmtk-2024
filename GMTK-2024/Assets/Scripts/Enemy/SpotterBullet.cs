using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpotterBullet : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] bool predictPositioning;
    [SerializeField] int strikeCount = 1;
    [SerializeField] float fireInterval = 0.5f;
    [SerializeField] float deviation = 0.2f;
    [SerializeField] GameObject ammo;
    Rigidbody2D rb;
    bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!collided)
            rb.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collided = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        StartCoroutine(SummonAirstrike());
    }

    IEnumerator SummonAirstrike()
    {
        for (int i = 0; i < strikeCount; i++)
        {
            Vector3 deviationVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * deviation;
            Instantiate(ammo, transform.position + deviationVec, Quaternion.identity);
            yield return new WaitForSeconds(fireInterval);
        }
        Destroy(gameObject);
    }
}
