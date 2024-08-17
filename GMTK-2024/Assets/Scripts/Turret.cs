using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] public int Damage = 1;
    [SerializeField] public float Velocity = 5f;

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

    [SerializeField] private GameObject[] _attackPrefabs;


    private void HandleAttacks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject attackOb = Instantiate(_attackPrefabs[(int)CurrentAttackType], _base.position, _base.rotation);
            IAttack attack = attackOb.GetComponent<IAttack>();
            attack.Initialize(_base, Damage, Velocity);
        }
    }

    #endregion Attack

}
