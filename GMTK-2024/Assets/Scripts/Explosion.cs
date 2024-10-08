using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] CircleCollider2D cirCol;
    [SerializeField] int damage, activeTicks, sourceID;
    [SerializeField] int playerDamage;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Color darkenCol, fadeCol;

    [SerializeField] bool autoScale;
    int timer;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5);
        if (autoScale)
        {
            damage = Mathf.RoundToInt(damage * PlayerController.GetDamageMultiplier());
            transform.localScale *= PlayerController.GetRelativeScaleFactor();
        }
        CameraManager.SetTrauma(30 + Mathf.RoundToInt(damage * 0.02f));

        float scale = transform.localScale.x;
        float pitch = Sigmoid(scale);
        GameManager.SoundManager.PlaySound(SoundType.EXPLOSION, pitch, pitch, 1, transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activeTicks > 0)
        {
            activeTicks--;
            rend.sortingOrder--;
            if (activeTicks == 0)
            {
                cirCol.enabled = false;
            }
        }

        rend.color -= darkenCol;
        rend.color -= fadeCol;
    }

    HPEntity hitEntity;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            hitEntity = col.GetComponent<HPEntity>();
            if (hitEntity)
            {
                if (playerDamage != 0 && col.TryGetComponent<PlayerController>(out PlayerController player))
                {
                    player.TakeDamage(playerDamage);
                }
                else
                {
                    hitEntity.TakeDamage(damage, sourceID);
                }
            }
        }
    }


    private float Sigmoid(float x)
    {
        // Calculate the exponential part
        float exponent = -(x - 2);
        // Calculate the denominator
        float denominator = 1 + Mathf.Exp(exponent);
        // Calculate y
        float y = 1.2f / denominator + 0.4f;
        return y;
    }
}
