using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera cam;
    static CameraManager self;

    // Interface:
    public static void SetTrauma(int trauma)
    {
        _trauma = Mathf.Max(_trauma, trauma);
    }
    public static void AddTrauma(int trauma)
    {
        _trauma += trauma;
    }


    private Transform _camera;
    private Transform _player;
    [SerializeField] private float FollowSpeed;
    private void Start()
    {
        _camera = Camera.main.transform;
        self = GetComponent<CameraManager>();
        targetSize = 5;
    }

    void FixedUpdate()
    {
        HandleFollow();
        HandleTrauma();
        HandleSizing();
    }

    float targetSize;
    void HandleSizing()
    {
        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.02f)
        {
            cam.orthographicSize += (targetSize - cam.orthographicSize) * 0.2f;
            if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.02f) { cam.orthographicSize = targetSize; }
        }
    }
    public static void SetSize(int size)
    {
        self.targetSize = size;
    }

    #region Follow
    
    private void HandleFollow()
    {
        if (GameManager.Instance.Player != null)
        {
            Vector2 playerPos = GameManager.Instance.Player.transform.position;
            Vector2 cameraPos = _camera.position;
            Vector2 offset = new Vector2(playerPos.x - cameraPos.x, playerPos.y - cameraPos.y);
            Camera.main.transform.position += (Vector3)offset * FollowSpeed;
        }

    }

    #endregion Follow

    #region ScreenShake
    [SerializeField] private static int _trauma;

    private void HandleTrauma()
    {
        if (_trauma > 0)
        {
            _trauma--;
            Vector2 shake = Random.insideUnitCircle * _trauma;
            _camera.position += (Vector3)shake;
        }
    }

    #endregion ScreenShake
}
