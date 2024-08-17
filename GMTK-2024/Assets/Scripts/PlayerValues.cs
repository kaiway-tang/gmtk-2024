using UnityEngine;

// Create a custom ScriptableObject class
[CreateAssetMenu(fileName = "NewData", menuName = "Custom/DataObject")]
public class PlayerValues : ScriptableObject
{
    public float acceleration, maxSpeed;
    public float groundedFriction, aerialFriction;
    public float jumpPower, doubleJumpPower;    
}
