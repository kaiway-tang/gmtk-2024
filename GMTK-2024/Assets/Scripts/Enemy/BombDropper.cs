using UnityEngine;

public class BombDropper : Skirmisher
{
    //

    int _timer = 0;
    int _numBombs = 0;
    [SerializeField] GameObject _bombPrefab;
    bool _running = false;

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        _timer++;
        if (_timer % 120 == 0)
        {
            Instantiate(_bombPrefab, transform.position, Quaternion.identity);
            _numBombs++;
        }

        // if the enemy has dropped 5 bombs, it will stop running:
        if (_numBombs >= 5 && _running)
        {
            _running = false;
            base.minRange = 7;
        }
    }

    public override bool TakeDamage(int amount = 0, int sourceID = 0)
    {
        _numBombs = 0;
        _running = true;
        base.minRange = 50;
        Debug.Log(base.minRange);

        return base.TakeDamage();
    }
}
