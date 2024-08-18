using UnityEngine;

public class PlayerController : MobileEntity
{
    [SerializeField] GameObject[] gatlingBullets;
    [SerializeField] GameObject[] rockets;

    [SerializeField] ParticleSystem tpPop;

    public int tier;
    public int type;
    const int GUNNER = 0, REAPER = 1, CONTROLLER = 2;
    public PlayerValues[] valRef;

    [SerializeField] GameObject[] gunnerObjs;
    [SerializeField] GameObject[] reaperObjs;
    [SerializeField] GameObject[] controllerObjs;

    public static PlayerController self;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        GameManager.Instance.Player = this;
        self = GetComponent<PlayerController>();
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
        HandleAttackInput();
        HandleAttacking();
        HandleFacing();        
    }

    #region MECH_TYPES

    void HandleSizeKeys()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (type == GUNNER) { SetType(REAPER); }
            else if (type == REAPER) { SetType(GUNNER); }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            tier = 0;
            transform.localScale = Vector3.one * valRef[tier].scale;
            CameraManager.SetSize(valRef[tier].cameraSize);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            tier = 1;
            transform.localScale = Vector3.one * valRef[tier].scale;
            CameraManager.SetSize(valRef[tier].cameraSize);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            tier = 2;
            transform.localScale = Vector3.one * valRef[tier].scale;
            CameraManager.SetSize(valRef[tier].cameraSize);
        }
    }

    void SetType(int newType)
    {
        if (type != newType)
        {
            if (type == GUNNER)
            {
                for (int i = 0; i < gunnerObjs.Length; i++)
                {
                    gunnerObjs[i].SetActive(false);
                }
            }

            if (type == REAPER)
            {
                for (int i = 0; i < reaperObjs.Length; i++)
                {
                    reaperObjs[i].SetActive(false);
                }
            }



            if (newType == GUNNER)
            {
                for (int i = 0; i < gunnerObjs.Length; i++)
                {
                    gunnerObjs[i].SetActive(true);
                }
            }

            if (newType == REAPER)
            {
                for (int i = 0; i < reaperObjs.Length; i++)
                {
                    reaperObjs[i].SetActive(true);
                }
            }

            type = newType;
        }
    }

    #endregion

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

    public static float GetRelativeScaleFactor()
    {
        return self.valRef[self.tier].scale / self.valRef[0].scale;
    }

    public static float GetDamageMultiplier()
    {
        return self.valRef[self.tier].damageMultiplier;
    }

    [SerializeField] Transform firepointTrfm;
    int primaryCD, secondaryCD;
    int secondaryTimer;
    void HandleAttackInput()
    {
        if (primaryCD < 0)
        {
            if (Input.GetMouseButton(0))
            {
                if (type == GUNNER)
                {
                    Instantiate(gatlingBullets[tier], firepointTrfm.position, firepointTrfm.rotation);
                    CameraManager.SetTrauma(1);
                    primaryCD = 4;
                }
                else
                {

                }      
            }
        }
        else
        {
            primaryCD--;
        }

        if (secondaryCD < 0)
        {
            if (Input.GetMouseButton(1))
            {
                if (type == GUNNER)
                {
                    Instantiate(rockets[tier], firepointTrfm.position, firepointTrfm.rotation);
                    secondaryCD = 200;
                    secondaryTimer = 40;
                }
                else
                {
                    RaycastHit2D rayHit = Physics2D.Raycast(trfm.position, mousePos - trfm.position, valRef[tier].blinkDistance, Tools.terrainMask);                    
                    if (rayHit.collider != null)
                    {
                        trfm.position += (mousePos - trfm.position).normalized * rayHit.distance;
                    }
                    else
                    {
                        trfm.position += (mousePos - trfm.position).normalized * valRef[tier].blinkDistance;
                    }

                    tpPop.Play();
                    secondaryCD = 150;
                }
            }
        }
        else
        {
            secondaryCD--;
        }
    }

    void HandleAttacking()
    {
        if (secondaryTimer > 0)
        {
            secondaryTimer--;

            if (secondaryTimer % 10 == 0)
            {
                if (type == GUNNER)
                {
                    Instantiate(rockets[tier], firepointTrfm.position, firepointTrfm.rotation);
                }
            }            
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
                if (IsTouchingGround())
                {
                    AddXVelocity(-valRef[tier].groundedAcceleration, -valRef[tier].maxSpeed);
                }
                else
                {
                    AddXVelocity(-valRef[tier].aerialAcceleration, -valRef[tier].maxSpeed);
                }
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (IsTouchingGround())
            {
                AddXVelocity(valRef[tier].groundedAcceleration, valRef[tier].maxSpeed);
            }
            else
            {
                AddXVelocity(valRef[tier].aerialAcceleration, valRef[tier].maxSpeed);
            }
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
