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

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { SetTrauma(10); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { SetTrauma(20); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { SetTrauma(40); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { SetTrauma(80); }
        }
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
    public static void SetSize(float size)
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
    [SerializeField] public float shakeStrength;

    Vector3 translateVect;
    private void HandleTrauma()
    {
        if (_trauma > 0)
        {
            _trauma--;
            translateVect = Random.insideUnitCircle.normalized * _trauma * _trauma * shakeStrength;
            translateVect.z = 0;
            _camera.position += translateVect;
        }
    }

    #endregion ScreenShake
}
