using UnityEngine;

public class HomingRocketHitbox : MonoBehaviour
{
    [SerializeField] HomingRocket _rocket;
    [SerializeField] Collider2D _rocketHurtbox;

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (_rocketHurtbox != col.collider && col.gameObject.layer >= 7 && col.gameObject.layer <= 8)
        {
            _rocket.Explode();
        }
        //if (col.gameObject.layer == 7)
        //{
        //    PlayerController playerScript = col.GetComponent<PlayerController>();
        //    if (playerScript)
        //        _rocket.Explode();
        //}
        //if (col.gameObject.layer == 6)
        //{
        //    _rocket.Explode();
        //}
    }
}
