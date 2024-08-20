using UnityEngine;


public enum SoundType
{
    GUN1, GUN2,
    EJECT,
    JUMP,
    WALK,
    CRAFT1, CRAFT2,
    EXPLOSION,
    SLASH, DASH,
    FLAMETHROWER, FORCEFIELD,
    KEYCOLLECT, BUTTON,
    DAMAGED1, DAMAGED2,
}
public class SoundManager : MonoBehaviour
{
    // 
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] AudioSource _SFX_normal;
    [SerializeField] AudioSource _SFX_spam;
    [SerializeField] AudioSource _SFX_continuous;
    [SerializeField] AudioSource _BGM;

    private void FixedUpdate()
    {
        HandleContinuousSound();
        HandleCooldownSound();
    }

    #region Normal Sound
    public void PlaySound(SoundType type, float p1, float p2, float volume, Vector2 position)
    {
        AudioSource source = (type == SoundType.GUN1 || type == SoundType.GUN2 || type == SoundType.SLASH) ? _SFX_spam : _SFX_normal;
        float pitch = Random.Range(p1, p2);
        source.pitch = pitch;
        source.volume = volume;
        source.transform.position = position;
        source.PlayOneShot(_clips[(int)type]);
    }
    #endregion Normal Sound

    #region Continuous Sound (flamethrower)
    int _duration = 0;
    int _queuedDuration = 0;
    const int FADE_OUT_DURATION = 20;
    public void PlayContinuousSound(SoundType type, int duration)
    {
        _SFX_continuous.clip = _clips[(int)type];
        if (_duration == 0)
        {
            _SFX_continuous.Play();
        }
        _queuedDuration = duration + FADE_OUT_DURATION;
    }
    private void HandleContinuousSound()
    {
        int queued = _queuedDuration > 0 ? 5 : 0;
        _duration = Mathf.Clamp(_duration - 1 + queued, 0, 20);
        _queuedDuration = Mathf.Max(0, _queuedDuration - queued);
        if (_duration <= FADE_OUT_DURATION)
        {
            _SFX_continuous.volume = (_duration / (float)FADE_OUT_DURATION) * 0.08f * (PlayerController.GetRelativeScaleFactor() + 1);
        }
        else
        {
            _SFX_continuous.volume = 1f;
        }
        if (_duration == 0)
        {
            _SFX_continuous.Stop();
        }
    }
    #endregion Continuous Sound (flamethrower)

    #region Singleton Sound (eject)


    #endregion Singleton Sound (eject)

    #region Cooldown Sound (walk)

    int _cooldown = 0;
    public void PlayCooldownSound(SoundType type, float p1, float p2, float volume, Vector2 position, int cooldown)
    {
        if (_cooldown == 0)
        {
            PlaySound(type, p1, p2, volume, position);
            _cooldown = cooldown;
        }
    }
    private void HandleCooldownSound()
    {
        Debug.Log(_cooldown);
        _cooldown = Mathf.Max(_cooldown - 1, 0);
    }

    #endregion Cooldown Sound (walk)

    #region Music
    private void Start()
    {
        _BGM.Play();
    }

    #endregion
}
