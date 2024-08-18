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
            CraftMech(MechType.GUNNER);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CraftMech(MechType.REAPER);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
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
        MechType type = GetOuterType();
        SetTier(tier);
        SetType(type);
    }


    #region Utils
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
                if (GetOuterType() == MechType.GUNNER)
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
                if (GetOuterType() == MechType.GUNNER)
                {
                    Instantiate(rockets[tier], firepointTrfm.position, firepointTrfm.rotation);
                    secondaryCD = 200;
                    secondaryTimer = 28;
                }
                else if (GetOuterType() == MechType.REAPER)
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
                else if (GetOuterType() == MechType.CONTROLLER)
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
                if (GetOuterType() == MechType.GUNNER)
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
