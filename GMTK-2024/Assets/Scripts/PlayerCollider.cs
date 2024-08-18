using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerController.OnEnter(collision);
    }
}
