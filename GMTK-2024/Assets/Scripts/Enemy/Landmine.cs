using System.Collections;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    //
    SpriteRenderer[] _sprites = new SpriteRenderer[3];
    [SerializeField] Collider2D _collider;
    [SerializeField] GameObject _explosion;
    [SerializeField] GameObject _sparks;

    void Start()
    {
        _sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7 && other.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            StartCoroutine(Explode());
            _collider.enabled = false;
        }
        if (other.gameObject.layer == 6)
        {
            Instantiate(_sparks, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void TriggerExplosion()
    {
        Instantiate(_explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator Explode()
    {
        int timer = 0;
        while (timer < 15)
        {
            foreach (var sprite in _sprites)
            {
                sprite.color = Color.Lerp(sprite.color, Color.white, 0.033f);
            }
            timer++;
            yield return new WaitForSeconds(0.02f);
        }
        TriggerExplosion();
    }
}
