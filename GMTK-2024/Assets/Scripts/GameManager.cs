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
    public static ResourceManager ResourceManager;


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
        ResourceManager = GetComponentInChildren<ResourceManager>();
    }
    #endregion Singleton

    // Player:
    [SerializeField] public PlayerController Player; // set by player upon Start
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] public Vector2 SpawnPos;

    // Update is called once per frame
    void Update()
    {
        if (Player == null && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Instantiate(PlayerPrefab, SpawnPos, Quaternion.identity);
        }
    }

    public void RestartGame()
    {
        LevelManager.levelsCompleted = 0;
    }
}
