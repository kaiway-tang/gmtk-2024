using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        playerController.OnEnter(collision);
    }
}
