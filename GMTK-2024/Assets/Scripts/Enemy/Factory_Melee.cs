using UnityEngine;

public class Factory_Melee : MonoBehaviour
{
    // 
    [SerializeField] Transform _factoryPrefab;
    [SerializeField] ParticleSystem _spawnParticle;
    [SerializeField] int _spawnInterval;

    // Animation:
    private Vector2 _animation;
    private Transform _spawning;


    private int _timer = 0;
    private void FixedUpdate()
    {
        _timer++;

        if (_timer > _spawnInterval)
        {
            _timer = 0;
            Spawn();
        }
    }
    private void Spawn()
    {
        _animTimer = 0;
        _animation = transform.position;
        _spawning = Instantiate(_factoryPrefab, _animation, Quaternion.identity);
    }
    private float _animTimer = 0;
    private void Update()
    {
        _animTimer += Time.deltaTime;
        if (_spawning)
        {
            // Move enemy up:
            _spawning.transform.position = _animation;
            _animation += Vector2.up * Time.deltaTime * 8f;
            // Spawn:
            ParticleSystem.EmissionModule emission = _spawnParticle.emission;
            emission.enabled = true;
        }
        if (_animTimer > 0.5f)
        {
            // Animation over: 
            _animTimer = 0;
            _spawning = null;
            ParticleSystem.EmissionModule emission = _spawnParticle.emission;
            emission.enabled = false;
        }
    }
}
