using UnityEngine;

// Create a custom ScriptableObject class
[CreateAssetMenu(fileName = "NewData", menuName = "Custom/DataObject")]
public class PlayerValues : ScriptableObject
{
    public int intValue;
    public float floatValue;
}
