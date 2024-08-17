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

    // ResourceManager
    public static ResourceManager Resources;
    // Start is called before the first frame update
    void Start()
    {
        Resources = new ResourceManager();
        Resources.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
