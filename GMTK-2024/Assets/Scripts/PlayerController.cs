using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MobileEntity
{

    [SerializeField] PlayerValues valRef;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        HandleJump();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();

        HandleMovement();
    }

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
                SetYVelocity(valRef.jumpPower);
            }
            else if (hasDJump)
            {
                SetYVelocity(valRef.doubleJumpPower);
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
                FaceLeft();
                AddXVelocity(-valRef.acceleration, -valRef.maxSpeed);
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            FaceRight();
            AddXVelocity(valRef.acceleration, valRef.maxSpeed);
        }
    }

    void HandleFriction()
    {
        if (IsTouchingGround())
        {
            ApplyXFriction(valRef.groundedFriction);
            hasDJump = true;
        }
        else
        {            
            ApplyXFriction(valRef.aerialFriction);
        }
    }

    #endregion
}
