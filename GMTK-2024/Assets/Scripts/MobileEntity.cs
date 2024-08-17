using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEntity : HPEntity
{
    [SerializeField] protected Transform reflectionTrfm;
    [SerializeField] protected Rigidbody2D rb;
    public float knockbackFactor = 1;
    public OnGround[] touchingTerrain; //0-4: ground, front, ceiling, backLow, backHigh
    protected bool facing = FACING_RIGHT;
    protected const bool FACING_LEFT = false, FACING_RIGHT = true;

    int gravityDisable; float defaultGravity;

    protected static Vector2 vect2; //passive vect2 to avoid declaring new Vector2 repeatedly
    protected static Vector3 vect3; // ^^

    protected new void Start()
    {
        base.Start();
    }
    protected new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void AddXVelocity(float amount, float max)
    {
        vect2 = rb.velocity;

        if (amount > 0)
        {
            if (vect2.x > max)
            {
                return;
            }
            else
            {
                vect2.x += amount;
                if (vect2.x > max)
                {
                    vect2.x = max;
                }
            }
        }
        else
        {
            if (vect2.x < max)
            {
                return;
            }
            else
            {
                vect2.x += amount;
                if (vect2.x < max)
                {
                    vect2.x = max;
                }
            }
        }

        rb.velocity = vect2;
    }
    protected void SetXVelocity(float value)
    {
        vect2 = rb.velocity;
        vect2.x = value;
        rb.velocity = vect2;
    }
    public void AddForwardXVelocity(float amount, float max)
    {
        if (IsFacingLeft()) { AddXVelocity(-amount, -max); }
        else { AddXVelocity(amount, max); }
    }

    public void AddForwardVelocity(float x, float y)
    {
        if (IsFacingRight()) { vect2.x = x; }
        else { vect2.x = -x; }
        vect2.y = y;

        rb.velocity = vect2;
    }

    protected void AddYVelocity(float amount, float max)
    {
        vect2 = rb.velocity;
        vect2.y += amount;

        vect2 = rb.velocity;

        if (amount > 0)
        {
            if (vect2.y > max)
            {
                return;
            }
            else
            {
                vect2.y += amount;
                if (vect2.y > max)
                {
                    vect2.y = max;
                }
            }
        }
        else
        {
            if (vect2.y < max)
            {
                return;
            }
            else
            {
                vect2.y += amount;
                if (vect2.y < max)
                {
                    vect2.y = max;
                }
            }
        }

        rb.velocity = vect2;
    }
    protected void SetYVelocity(float value)
    {
        vect2 = rb.velocity;
        vect2.y = value;
        rb.velocity = vect2;
    }

    protected void ApplyXFriction(float amount)
    {
        vect2 = rb.velocity;

        if (vect2.x > 0)
        {
            vect2.x -= amount;
            if (vect2.x < 0)
            {
                vect2.x = 0;
            }
        }
        else
        {
            vect2.x += amount;
            if (vect2.x > 0)
            {
                vect2.x = 0;
            }
        }

        rb.velocity = vect2;
    }

    protected void ApplyYFriction(float amount)
    {
        vect2 = rb.velocity;

        if (vect2.y > 0)
        {
            vect2.y -= amount;
            if (vect2.y < 0)
            {
                vect2.y = 0;
            }
        }
        else
        {
            vect2.y += amount;
            if (vect2.y > 0)
            {
                vect2.y = 0;
            }
        }

        rb.velocity = vect2;
    }

    float magnitude, ratio;
    protected void ApplyDirectionalFriction(float amount)
    {
        if (Mathf.Abs(rb.velocity.x) > 0.0001f || Mathf.Abs(rb.velocity.y) > 0.0001f)
        {
            vect2.x = rb.velocity.x;
            vect2.y = rb.velocity.y;
            magnitude = vect2.magnitude;
            ratio = (magnitude - amount) / magnitude;

            if (ratio > 0)
            {
                vect2.x = rb.velocity.x * ratio;
                vect2.y = rb.velocity.y * ratio;

                rb.velocity = vect2;
            }
            else
            {
                vect2.x = 0;
                vect2.y = 0;

                rb.velocity = vect2;
            }
        }
        else
        {
            vect2.x = 0;
            vect2.y = 0;

            rb.velocity = vect2;
        }
    }

    protected void SetVelocity(float x, float y)
    {
        vect2.x = x; vect2.y = y;
        rb.velocity = vect2;
    }

    public bool IsFacingRight()
    {
        return facing;
    }
    public bool IsFacingLeft()
    {
        return !facing;
    }
    protected void FaceRight()
    {
        if (facing == FACING_RIGHT) { return; }
        vect3 = reflectionTrfm.localScale;
        vect3.x = Mathf.Abs(vect3.x);
        reflectionTrfm.localScale = vect3;
        facing = FACING_RIGHT;
    }
    protected void FaceLeft()
    {
        if (facing == FACING_LEFT) { return; }
        vect3 = reflectionTrfm.localScale;
        vect3.x = -Mathf.Abs(vect3.x);
        reflectionTrfm.localScale = vect3;
        facing = FACING_LEFT;
    }

    public bool IsTouchingGround()
    {
        return touchingTerrain[0].touchCount > 0;
    }

    protected void DisableGravity()
    {
        if (gravityDisable < 1)
        {
            defaultGravity = rb.gravityScale;
            rb.gravityScale = 0;
        }
        gravityDisable++;
    }
    protected void EnableGravity()
    {
        gravityDisable--;
        if (gravityDisable < 1)
        {
            rb.gravityScale = defaultGravity;
            gravityDisable = 0;
        }
    }

    public void TakeKnockback(Vector3 source, int power)
    {
        rb.velocity = (trfm.position - source).normalized * power * knockbackFactor;
    }
    public void TakeKnockback(Vector2 knockback)
    {
        rb.velocity = knockback * knockbackFactor;
    }
}
