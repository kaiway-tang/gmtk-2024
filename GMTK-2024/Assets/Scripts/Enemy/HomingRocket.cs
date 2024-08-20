using UnityEngine;

public class HomingRocket : MeleeEnemy
{
    //
    [SerializeField] float _maxSpeed;
    [SerializeField] GameObject _explosion;

    new private void FixedUpdate()
    {
        base.FixedUpdate();

        navigation.speed = Mathf.Min(navigation.speed + Time.deltaTime * 2f, _maxSpeed);

        // Fire explodes:
        if (burnTick > 0)
        {
            Explode();
        }
    }
    public void Explode()
    {
        Instantiate(_explosion, trfm.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
            Explode();
        }
        if (col.gameObject.layer == 7)
        {
            PlayerController playerScript = col.GetComponent<PlayerController>();
            if (playerScript)
                Explode();
        }
        if (col.gameObject.layer == 8 && navigation.speed >= 0.8f * _maxSpeed)
        {
            Explode();
        }
    }

}
