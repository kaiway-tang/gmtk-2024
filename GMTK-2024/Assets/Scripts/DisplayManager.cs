using UnityEngine;
using UnityEngine.UI;

public class DisplayManager : MonoBehaviour
{
    //
    [SerializeField] private Image _crosshair;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // move cursor to mouse position:
        _crosshair.transform.position = Input.mousePosition;
    }
}
