using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MobileEntity
{
    [SerializeField] GameObject[] gatlingBullets;
    [SerializeField] GameObject[] rockets;

    [SerializeField] GameObject[] flameObjs;
    [SerializeField] GameObject[] forceFields;

    [SerializeField] ParticleSystem tpPop;

    public enum MechType { INVALID, GUNNER, REAPER, CONTROLLER }; // this enum corresponds 1:1 with ResourceManager.Resources.
    public List<MechType> activeMechs;
    [SerializeField] public MechType DefaultType;

    public PlayerValues[] valRef;

    [SerializeField] SpriteRenderer capsuleSprite;
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

        // Set Default Type:
        CraftMech(DefaultType);
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
            ResourceManager.Resource resource = GameManager.ResourceManager.CheckCraftable();
            if (resource != ResourceManager.Resource.Invalid)
            {
                GameManager.ResourceManager.HandleCraft(resource);
                CraftMech((MechType)resource);
            }
        }
    }

    #region EJECTING

    void HandleEjecting()
    {
        if (Input.GetKeyDown(EJECT))
        {
            int tier = GetTier();
            if (tier > 0)
            {
                Instantiate(ejectShells[tier], trfm.position, Quaternion.identity);
                SetYVelocity(valRef[tier].ejectPower);
                activeMechs.RemoveAt(activeMechs.Count - 1);
                OnTypeChange();
            }
        }
    }

    #endregion

    #region MECH_TYPES

    void HandleSizeKeys()
    {
        // Duplicate current outmost shell:
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CraftMech(GetOuterType());
        }
        // Add new outer shell:
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeMechs.RemoveAt(activeMechs.Count - 1);
            CraftMech(MechType.GUNNER);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeMechs.RemoveAt(activeMechs.Count - 1);
            CraftMech(MechType.REAPER);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activeMechs.RemoveAt(activeMechs.Count - 1);
            CraftMech(MechType.CONTROLLER);
        }
    }
    private void CraftMech(MechType type)
    {
        if (type == MechType.INVALID)
        {
            type = (MechType)ResourceManager.GetRandom();
        }
        // Add outer shell of type if possible:
        int tier = GetTier();
        if (tier < 2)
        {
            activeMechs.Add(type);
            OnTypeChange();
        }
    }
    private void OnTypeChange()
    {
        // Update abilities based on outer shell:
        int tier = GetTier();
        base.Tier = tier;
        MechType type = GetOuterType();
        SetTier(tier);
        SetType(type);
    }


    #region Utils
    void SetTier(int pTier)
    {
        if (pTier > 2) { pTier = 2; return; }
        Tier = pTier;
        transform.localScale = Vector3.one * valRef[Tier].scale;
        CameraManager.SetSize(valRef[Tier].cameraSize);

        primaryCD = 0;
        secondaryCD = 0;
        secondaryTimer = 0;
    }
    void SetType(MechType newType)
    {
        // Disable old sprites:
        for (int i = 0; i < gunnerObjs.Length; i++)
        {
            gunnerObjs[i].SetActive(false);
        }
        for (int i = 0; i < reaperObjs.Length; i++)
        {
            reaperObjs[i].SetActive(false);
        }
        for (int i = 0; i < controllerObjs.Length; i++)
        {
            controllerObjs[i].SetActive(false);
        }
        // Enable new:
        if (newType == MechType.GUNNER)
        {
            for (int i = 0; i < gunnerObjs.Length; i++)
            {
                gunnerObjs[i].SetActive(true);
            }
        }
        if (newType == MechType.REAPER)
        {
            for (int i = 0; i < reaperObjs.Length; i++)
            {
                reaperObjs[i].SetActive(true);
            }
        }
        if (newType == MechType.CONTROLLER)
        {
            for (int i = 0; i < controllerObjs.Length; i++)
            {
                controllerObjs[i].SetActive(true);
            }
        }
        //
        capsuleSprite.color = GameManager.ResourceManager.GetResourceColor((ResourceManager.Resource)newType);
    }
    private MechType GetOuterType()
    {
        // Gets type of the outmost shell:
        return activeMechs[activeMechs.Count - 1];
    }
    private int GetTier()
    {
        return activeMechs.Count - 1; // tiers are 0 - 2.
    }

    #endregion Utils
    #endregion

    #region AIMING

    [SerializeField] Transform turretTrfm;
    public Vector3 mousePos;
    bool turretFacingLeft;
    void HandleAiming()
    {
        //turretTrfm.right = mousePos - turretTrfm.position;
        Tools.LerpRotation(turretTrfm, mousePos, valRef[Tier].aimSpeed);

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
            Tools.LerpRotation(turretTrfm, mousePos, valRef[Tier].aimSpeed);
        }
        else
        {
            Tools.LerpRotation(turretTrfm, mousePos, valRef[Tier].aimSpeed, 180);
        }
    }

    public static float GetRelativeScaleFactor()
    {
        return self.valRef[self.Tier].scale / self.valRef[0].scale;
    }

    public static float GetDamageMultiplier()
    {
        return self.valRef[self.Tier].damageMultiplier;
    }

    [SerializeField] Transform firepointTrfm;
    int primaryCD, secondaryCD;
    int secondaryTimer;

    Vector2 REAPER_blinkTargetPos;

    void HandleAttackInput()
    {
        MechType type = GetOuterType();
        if (primaryCD < 0)
        {
            if (Input.GetMouseButton(0))
            {
                if (type == MechType.GUNNER)
                {
                    Instantiate(gatlingBullets[Tier], firepointTrfm.position, firepointTrfm.rotation);
                    CameraManager.SetTrauma(1);
                    primaryCD = 4;
                }
                else if (GetOuterType() == MechType.CONTROLLER)
                {
                    Instantiate(flameObjs[Tier], firepointTrfm.position, firepointTrfm.rotation);
                    primaryCD = 5;
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
                if (type == MechType.GUNNER)
                {
                    Instantiate(rockets[Tier], firepointTrfm.position, firepointTrfm.rotation);
                    secondaryCD = 200;
                    secondaryTimer = 28;
                }
                else if (type == MechType.REAPER)
                {
                    RaycastHit2D rayHit = Physics2D.Raycast(trfm.position, mousePos - trfm.position, valRef[Tier].blinkDistance, Tools.terrainMask);
                    if (rayHit.collider != null)
                    {
                        REAPER_blinkTargetPos = (mousePos - trfm.position).normalized * rayHit.distance;
                    }
                    else
                    {
                        REAPER_blinkTargetPos = (mousePos - trfm.position).normalized * valRef[Tier].blinkDistance;
                    }
                    secondaryTimer = 5;
                    secondaryCD = 150;
                }
                else if (type == MechType.CONTROLLER)
                {
                    Instantiate(forceFields[Tier], firepointTrfm.position, firepointTrfm.rotation);
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
                if (GetOuterType() == MechType.GUNNER)
                {
                    Instantiate(rockets[Tier], firepointTrfm.position, firepointTrfm.rotation);
                }
            }
            if (GetOuterType() == MechType.REAPER)
            {
                // Dash:
                int tier = GetTier();
                tpPop.transform.localScale = Vector3.one * (tier + 1);
                tpPop.Emit(3);
                transform.position += (Vector3)REAPER_blinkTargetPos * 0.2f;
                SetYVelocity(0);
                SetXVelocity(0);
                // Get collider overlap:
                Collider2D[] colliders = Physics2D.OverlapCircleAll(trfm.position, 1.5f * tier, Tools.hurtMask);
                for (int i = 0; i < colliders.Length; i++)
                {
                    HPEntity hpEntity = colliders[i].GetComponent<HPEntity>();
                    if (hpEntity != null)
                    {
                        hpEntity.TakeDamage((int)(30 * valRef[tier].damageMultiplier));
                        if (hpEntity.HP <= 0)
                        {
                            secondaryCD = secondaryTimer;
                        }
                    }
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
                SetYVelocity(valRef[Tier].jumpPower);
            }
            else if (hasDJump)
            {
                SetYVelocity(valRef[Tier].doubleJumpPower);
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
                    if (rb.velocity.x < valRef[Tier].groundedAcceleration)
                    {
                        AddXVelocity(-valRef[Tier].groundedAcceleration, -valRef[Tier].maxSpeed);
                    }
                }
                else
                {
                    AddXVelocity(-valRef[Tier].aerialAcceleration, -valRef[Tier].maxSpeed);
                }
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (IsTouchingGround())
            {
                if (rb.velocity.x > -valRef[Tier].groundedAcceleration)
                {
                    AddXVelocity(valRef[Tier].groundedAcceleration, valRef[Tier].maxSpeed);
                }
            }
            else
            {
                AddXVelocity(valRef[Tier].aerialAcceleration, valRef[Tier].maxSpeed);
            }
        }
    }

    void HandleFriction()
    {
        if (IsTouchingGround())
        {
            ApplyXFriction(valRef[Tier].groundedFriction);
            hasDJump = true;
        }
        else
        {
            ApplyXFriction(valRef[Tier].aerialFriction);
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

    #region HP Entity Overrides
    public override bool TakeDamage(int amount = 0, int sourceID = 0)
    {
        CameraManager.SetTrauma(4);
        // Reaper dash ignores damage:
        if (GetOuterType() == MechType.REAPER && secondaryTimer > 0)
        {
            return false;
        }
        return base.TakeDamage(amount, sourceID);
    }

    #endregion HP Entity Overrides
}
