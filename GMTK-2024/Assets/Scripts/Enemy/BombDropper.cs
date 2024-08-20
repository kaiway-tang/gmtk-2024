using UnityEngine;

public class BombDropper : MonoBehaviour
{
    //

    int _timer = 0;
    [SerializeField] GameObject _bombPrefab;


    private void FixedUpdate()
    {
        _timer++;
        if (_timer % 120 == 0)
        {
            Instantiate(_bombPrefab, transform.position + Vector3.down, Quaternion.identity);
        }
    }
}
