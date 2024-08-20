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
    Animator anim;
    bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
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
        anim.Play("SpotterBullet_Idle");
        StartCoroutine(SummonAirstrike());
    }

    IEnumerator SummonAirstrike()
    {
        for (int i = 0; i < strikeCount; i++)
        {
            Vector3 deviationVec = Random.insideUnitCircle * Random.Range(0,deviation);
            Instantiate(ammo, transform.position + deviationVec, Quaternion.identity);
            yield return new WaitForSeconds(fireInterval);
        }
        Destroy(gameObject);
    }
}
