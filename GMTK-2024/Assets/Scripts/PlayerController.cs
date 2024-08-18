using UnityEngine;

public class PlayerController : MobileEntity
{
    [SerializeField] GameObject[] gatlingBullets;
    [SerializeField] GameObject[] rockets;

    [SerializeField] GameObject[] forceFields;

    [SerializeField] ParticleSystem tpPop;

    public int type;
    const int GUNNER = 0, REAPER = 1, CONTROLLER = 2;
    public PlayerValues[] valRef;

    [SerializeField] GameObject[] gunnerObjs;
    [SerializeField] GameObject[] reaperObjs;
    [SerializeField] GameObject[] controllerObjs;

    [SerializeField] GameObject[] ejectShells;

    public static PlayerController self;

    // Controls:
    private KeyCode EJECT = KeyCode.LeftShift;
    private KeyCode JUMP = KeyCode.Space;
    private KeyCode JUMP_ALT = KeyCode.W;
    private KeyCode CRAFT = KeyCode.Q;

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
        HandleEjecting();

        HandleCrafting();
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

    void HandleCrafting()
    {
        if (Input.GetKeyDown(CRAFT))
        {
            if (GameManager.ResourceManager.HandleCraft(GameManager.ResourceManager.CheckCraftable()))
            {
                SetTier(Mathf.Min(tier + 1, 2));
            }
        }
    }

    #region EJECTING

    void HandleEjecting()
    {
        if (Input.GetKeyDown(EJECT))
        {
            if (tier > 0)
            {
                Instantiate(ejectShells[tier], trfm.position, Quaternion.identity);
                SetTier(tier - 1);
                SetYVelocity(valRef[tier].ejectPower);
            }
        }
    }

    #endregion

    #region MECH_TYPES

    void HandleSizeKeys()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetTier(tier + 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetType(GUNNER);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetType(REAPER);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetType(CONTROLLER);
        }
    }

    void SetTier(int pTier)
    {
        if (pTier > 2) { pTier = 2; return; }
        tier = pTier;
        transform.localScale = Vector3.one * valRef[tier].scale;
        CameraManager.SetSize(valRef[tier].cameraSize);

        primaryCD = 0;
        secondaryCD = 0;
        secondaryTimer = 0;
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
            if (type == CONTROLLER)
            {
                for (int i = 0; i < controllerObjs.Length; i++)
                {
                    controllerObjs[i].SetActive(false);
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
            if (newType == CONTROLLER)
            {
                for (int i = 0; i < controllerObjs.Length; i++)
                {
                    controllerObjs[i].SetActive(true);
                }
            }

            type = newType;
        }
    }

    #endregion

    #region AIMING

    [SerializeField] Transform turretTrfm;
    public Vector3 mousePos;
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
                    secondaryTimer = 28;
                }
                else if (type == REAPER)
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
                else if (type == CONTROLLER)
                {
                    Instantiate(forceFields[tier], firepointTrfm.position, firepointTrfm.rotation);
                    secondaryCD = 20;
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

            if (secondaryTimer % 7 == 0)
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
        if (Input.GetKeyDown(JUMP) || Input.GetKeyDown(JUMP_ALT))
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
                    if (rb.velocity.x < valRef[tier].groundedAcceleration)
                    {
                        AddXVelocity(-valRef[tier].groundedAcceleration, -valRef[tier].maxSpeed);
                    }
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
                if (rb.velocity.x > -valRef[tier].groundedAcceleration)
                {
                    AddXVelocity(valRef[tier].groundedAcceleration, valRef[tier].maxSpeed);
                }
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


    #region COLLIDING
    public void OnEnter(Collider2D collision)
    {
        // Resource Drops:
        if (collision.gameObject.layer == ResourceDrop.Layer)
        {
            ResourceDrop resourceDrop = collision.gameObject.GetComponent<ResourceDrop>();
            if (resourceDrop != null)
            {
                resourceDrop.OnPickup();
                GameManager.ResourceManager.AddResource(resourceDrop.GetResource(), collision.gameObject.transform.position);
            }
        }
    }


    #endregion COLLIDING
}
