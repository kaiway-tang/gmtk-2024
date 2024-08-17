using UnityEngine;

public interface IAttack
{
    void Initialize(Transform relative, int damage, float velocity);
}


public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    public static DisplayManager DisplayManager;
    public static CameraManager CameraManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        //
        DisplayManager = GetComponentInChildren<DisplayManager>();
        CameraManager = GetComponentInChildren<CameraManager>();
    }
    #endregion Singleton

    // Player:
    [SerializeField] public PlayerController Player;
}
