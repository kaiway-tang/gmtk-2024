using UnityEngine;

public class PlayerController : MobileEntity
{
    [SerializeField] GameObject[] gatlingBullets;

    [SerializeField] int tier;
    [SerializeField] PlayerValues[] valRef;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        GameManager.Instance.Player = this;
    }

    // Update is called once per frame
    void Update()
    {
        HandleJump();
        HandleSizeKeys();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        HandleMovement();
        HandleAiming();
        HandleAttacking();
        HandleFacing();        
    }

    void HandleSizeKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            tier = 0;
            transform.localScale = Vector3.one;
            CameraManager.SetSize(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            tier = 1;
            transform.localScale = Vector3.one * 2;
            CameraManager.SetSize(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            tier = 2;
            transform.localScale = Vector3.one * 4;
            CameraManager.SetSize(11);
        }
    }

    #region AIMING

    [SerializeField] Transform turretTrfm;
    Vector3 mousePos;
    bool turretFacingLeft;
    void HandleAiming()
    {
        //turretTrfm.right = mousePos - turretTrfm.position;
        Tools.LerpRotation(turretTrfm, mousePos, valRef[tier].aimSpeed);

        return;

        if (trfm.position.x < mousePos.x)
        {
            vect3 = turretTrfm.localScale;
            vect3.y = Mathf.Abs(vect3.y);
            turretTrfm.localScale = vect3;
        }
        else
        {
            vect3 = turretTrfm.localScale;
            vect3.y = -Mathf.Abs(vect3.y);
            turretTrfm.localScale = vect3;
        }

        return;
        if (trfm.position.x < mousePos.x)
        {
            Tools.LerpRotation(turretTrfm, mousePos, valRef[tier].aimSpeed);
        }
        else
        {
            Tools.LerpRotation(turretTrfm, mousePos, valRef[tier].aimSpeed, 180);
        }
    }

    [SerializeField] Transform firepointTrfm;
    int fireCD;
    void HandleAttacking()
    {
        if (fireCD < 0)
        {
            if (Input.GetMouseButton(0))
            {
                Instantiate(gatlingBullets[tier], firepointTrfm.position, firepointTrfm.rotation);
                fireCD = 3;
            }
        }
        else
        {
            fireCD--;
        }
    }

    #endregion    

    #region MOVEMENT

    void HandleMovement()
    {
        HandleHorizontalMovement();
        HandleFriction();
    }

    bool hasDJump;
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsTouchingGround())
            {
                SetYVelocity(valRef[tier].jumpPower);
            }
            else if (hasDJump)
            {
                SetYVelocity(valRef[tier].doubleJumpPower);
                hasDJump = false;
            }
        }
    }

    void HandleHorizontalMovement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (!Input.GetKey(KeyCode.D))
            {
                AddXVelocity(-valRef[tier].acceleration, -valRef[tier].maxSpeed);
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            AddXVelocity(valRef[tier].acceleration, valRef[tier].maxSpeed);
        }
    }

    void HandleFriction()
    {
        if (IsTouchingGround())
        {
            ApplyXFriction(valRef[tier].groundedFriction);
            hasDJump = true;
        }
        else
        {            
            ApplyXFriction(valRef[tier].aerialFriction);
        }
    }

    #endregion

    void HandleFacing()
    {
        if (trfm.position.x < mousePos.x)
        {
            FaceRight();
        }
        else
        {
            FaceLeft();
        }
    }
}
