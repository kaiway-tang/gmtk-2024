using UnityEngine;

public class HeadRotator : MonoBehaviour
{
    void Update()
    {
        HandleRotation();
        HandleAttacks();
    }



    #region Rotation
    public int RotationSpeed = 1;
    [SerializeField] private Transform _base;

    private void HandleRotation()
    {
        // Rotate head to mouse position:
        Vector3 mousePos = Input.mousePosition;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_base.position);
        Vector3 direction = mousePos - screenPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _base.rotation = Quaternion.Slerp(_base.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), Time.deltaTime * RotationSpeed);
    }
    #endregion Rotation

    #region Attack
    public enum AttackType
    {
        Slash,
        Bullet,
        Spray
    }
    public AttackType CurrentAttackType = AttackType.Slash;

    [SerializeField] private IAttack[] _attackPrefabs;


    private void HandleAttacks()
    {
        // test for input mouse down:
        if (Input.GetMouseButtonDown(0))
        {
            switch (CurrentAttackType)
            {
                case AttackType.Slash:
                    Debug.Log("Slash attack!");
                    break;
                case AttackType.Bullet:
                    Debug.Log("Bullet attack!");
                    break;
                case AttackType.Spray:
                    Debug.Log("Spray attack!");
                    break;
            }
        }
    }

    #endregion Attack

}
