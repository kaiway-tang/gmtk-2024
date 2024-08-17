using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] public int Damage = 1;
    [SerializeField] public float Velocity = 5f;
    [SerializeField] Transform playerTrfm;

    void Update()
    {
        HandleRotation();
        //HandleAttacks();
    }

    #region Rotation
    public float aimSpeed = 0.1f;
    [SerializeField] private Transform turretTrfm, firepointTrfm;
    Vector3 mousePos;

    private void HandleRotation()
    {
        // Rotate head to mouse position:
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Tools.LerpRotation(turretTrfm, 45, aimSpeed);
        if (playerTrfm.position.x < mousePos.x)
        {
            Tools.LerpRotation(turretTrfm, mousePos, aimSpeed);
        }
        else
        {
            Tools.LerpRotation(turretTrfm, mousePos, aimSpeed);
        }
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


    //private void HandleAttacks()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        GameObject attackOb = Instantiate(_attackPrefabs[(int)CurrentAttackType], _base.position, _base.rotation);
    //        IAttack attack = attackOb.GetComponent<IAttack>();
    //        attack.Initialize(_base, Damage, Velocity);
    //    }
    //}

    #endregion Attack

}
