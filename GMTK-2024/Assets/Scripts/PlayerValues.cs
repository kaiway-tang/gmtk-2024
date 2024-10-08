using UnityEngine;

// Create a custom ScriptableObject class
[CreateAssetMenu(fileName = "NewData", menuName = "Custom/DataObject")]
public class PlayerValues : ScriptableObject
{
    public float scale, cameraSize;

    public float groundedAcceleration, aerialAcceleration, maxSpeed;
    public float groundedFriction, aerialFriction;
    public float jumpPower, doubleJumpPower, ejectPower;

    public float aimSpeed;
    public float damageMultiplier;

    public float blinkDistance;
}
