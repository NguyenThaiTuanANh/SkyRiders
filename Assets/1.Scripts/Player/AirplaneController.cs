using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class AirplaneController : MonoBehaviour
{
    [Header("Movement")]
    public float FlySpeed = 5f;
    public float YawAmount = 120f;
    public float MoveVerticalSpeed = 3f;   // tốc độ lên xuống theo joystick

    [Header("Limit Area")]
    public float MinY = -3f;
    public float MaxY = 5f;

    public float MinZ = -10f;
    public float MaxZ = 10f;

    public float MinX = -10f;
    public float MaxX = 10f;

    [Header("Mobile Joystick")]
    public Joystick moveJoystick; // ← gán joystick ở đây

    [Header("Bomb System")]
    public Transform bombSpawnPoint;
    public GameObject bombPrefab;
    public float bombCooldown = 0.8f;

    [Header("Bullet / Gun (gắn nút UI)")]
    [Tooltip("Prefab đạn (override bởi skin nếu có skinDatabase).")]
    public GameObject bulletPrefab;
    [Tooltip("Danh sách điểm spawn đạn (có thể có nhiều điểm, spawn đồng thời). Nếu trống sẽ dùng bombSpawnPoint.")]
    public List<Transform> bulletSpawnPoints = new List<Transform>();
    [Tooltip("Thời gian chờ giữa mỗi lần bắn (giây).")]
    public float bulletCooldown = 0.15f;

    [Header("Plane Skin (weapon theo skin)")]
    [Tooltip("Database skin. Nếu gán sẽ dùng đạn/bom của skin hiện tại (SaveLoadManager.CurrentSkinId).")]
    [SerializeField] private SO_PlaneSkinDatabase skinDatabase;

    [Header("Skin Models (5 skin GameObjects)")]
    [Tooltip("Danh sách 5 GameObject skin (skinId 0-4). Gán tay hoặc để trống để tự tìm theo tên 'Skin0', 'Skin1', ...")]
    [SerializeField] private List<GameObject> skinModels = new List<GameObject>();

    [Tooltip("Tự động tìm skin models theo tên nếu skinModels trống.")]
    [SerializeField] private bool autoFindSkinModels = true;

    [Tooltip("Prefix tên skin model để tự tìm (vd: 'Skin' → tìm 'Skin0', 'Skin1', ...).")]
    [SerializeField] private string skinNamePrefix = "Skin";

    public static AirplaneController Instance { get; private set; }

    private float Yaw;
    private float bombTimer;
    private float bulletTimer;
    private bool isShooting = false; // Flag để auto-fire khi giữ button

    public GameObject explosionFX;
    private PlayerHealth playerHealth;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.explosionFX = explosionFX;
    }

    void Start()
    {
        InitializeSkinModels();
        ApplyCurrentSkin();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Khởi tạo danh sách skin models: gán tay hoặc tự tìm theo tên.
    /// </summary>
    private void InitializeSkinModels()
    {
        if (skinModels == null)
            skinModels = new List<GameObject>();

        // Nếu đã gán đủ 5 skin thì không cần tìm
        if (skinModels.Count >= SO_PlaneSkinDatabase.SKIN_COUNT)
        {
            // Đảm bảo có đủ 5 phần tử
            while (skinModels.Count < SO_PlaneSkinDatabase.SKIN_COUNT)
                skinModels.Add(null);
            return;
        }

        if (autoFindSkinModels)
        {
            // Tìm các child object theo tên: Skin0, Skin1, Skin2, Skin3, Skin4
            skinModels.Clear();
            for (int i = 0; i < SO_PlaneSkinDatabase.SKIN_COUNT; i++)
            {
                string skinName = skinNamePrefix + i;
                Transform child = transform.Find(skinName);
                if (child == null)
                {
                    // Thử tìm trong toàn bộ children (không phân biệt chữ hoa/thường)
                    foreach (Transform t in transform)
                    {
                        if (t.name.Equals(skinName, System.StringComparison.OrdinalIgnoreCase) ||
                            t.name.Contains(skinName) || t.name.Contains("Skin" + i))
                        {
                            child = t;
                            break;
                        }
                    }
                }
                skinModels.Add(child != null ? child.gameObject : null);
            }
        }
        else
        {
            // Đảm bảo có đủ 5 phần tử
            while (skinModels.Count < SO_PlaneSkinDatabase.SKIN_COUNT)
                skinModels.Add(null);
        }
    }

    /// <summary>
    /// Áp dụng skin hiện tại: model + weapon (đạn/bom).
    /// </summary>
    public void ApplyCurrentSkin()
    {
        ApplyCurrentSkinModel();
        ApplyCurrentSkinWeapons();
    }

    /// <summary>
    /// Áp dụng model skin: enable skin đang chọn, disable các skin khác.
    /// </summary>
    public void ApplyCurrentSkinModel()
    {
        if (SaveLoadManager.Instance == null) return;

        int currentSkinId = SaveLoadManager.Instance.CurrentSkinId;

        // Enable/disable từng skin model
        for (int i = 0; i < skinModels.Count && i < SO_PlaneSkinDatabase.SKIN_COUNT; i++)
        {
            if (skinModels[i] != null)
                skinModels[i].SetActive(i == currentSkinId);
        }
    }

    /// <summary>
    /// Áp dụng đạn/bom theo skin đang chọn (từ SaveLoadManager + skinDatabase).
    /// </summary>
    public void ApplyCurrentSkinWeapons()
    {
        if (skinDatabase == null) return;
        if (SaveLoadManager.Instance == null) return;

        int skinId = SaveLoadManager.Instance.CurrentSkinId;
        PlaneSkinData skin = skinDatabase.GetSkin(skinId);
        if (skin == null) return;

        if (skin.bulletPrefab != null)
            bulletPrefab = skin.bulletPrefab;
        if (skin.bombPrefab != null)
            bombPrefab = skin.bombPrefab;
    }

    void Update()
    {
        // ===== INPUT =====
        float horizontalInput;
        float verticalInput;

        if (moveJoystick != null && moveJoystick.Direction.magnitude > 0.01f)
        {
            // MOBILE
            horizontalInput = moveJoystick.Horizontal;
            verticalInput = -moveJoystick.Vertical;
        }
        else
        {
            // PC
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = -Input.GetAxis("Vertical");
        }

        // ===== MOVE FORWARD =====
        transform.position += transform.forward * FlySpeed * Time.deltaTime;

        // ===== MOVE UP / DOWN =====
        //transform.position -= Vector3.up * verticalInput * MoveVerticalSpeed * Time.deltaTime;

        // ===== ROTATION =====
        Yaw += horizontalInput * YawAmount * Time.deltaTime;

        float pitch = Mathf.Lerp(0, 20, Mathf.Abs(verticalInput)) * Mathf.Sign(verticalInput);
        float roll = Mathf.Lerp(0, 30, Mathf.Abs(horizontalInput)) * -Mathf.Sign(horizontalInput);

        transform.localRotation = Quaternion.Euler(pitch, Yaw, roll);

        // ===== CLAMP POSITION =====
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, MinY, MaxY);
        pos.z = Mathf.Clamp(pos.z, MinZ, MaxZ);
        pos.x = Mathf.Clamp(pos.x, MinX, MaxX);
        transform.position = pos;

        // ===== BOMB =====
        HandleBomb();

        // ===== BULLET (PC: chuột trái hoặc Fire1) =====
        bulletTimer += Time.deltaTime;
#if UNITY_EDITOR || UNITY_STANDALONE
        //if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Mouse0))
        //    isShooting = true;
        //else
        //    isShooting = false;
#endif
        // Auto-fire khi giữ button (isShooting = true)
        if (isShooting)
            TryShootBullet();
    }

    // ================= BOMB =================

    void HandleBomb()
    {
        bombTimer += Time.deltaTime;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKey(KeyCode.Space))
            TryDropBomb();
#endif
    }

    // Gọi từ UI Button (Mobile)
    public void TryDropBomb()
    {
        if (bombTimer < bombCooldown) return;

        bombTimer = 0f;

        Instantiate(
            bombPrefab,
            bombSpawnPoint.position,
            Quaternion.identity
        );
    }

    // ================= BULLET (dùng class Bullet sẵn có) =================

    /// <summary>
    /// Bắt đầu bắn liên tục (auto-fire). Gọi từ UI Button OnPointerDown hoặc OnClick.
    /// </summary>
    public void StartShooting()
    {
        isShooting = true;
    }

    /// <summary>
    /// Dừng bắn. Gọi từ UI Button OnPointerUp hoặc OnClick (nếu dùng toggle).
    /// </summary>
    public void StopShooting()
    {
        isShooting = false;
    }

    /// <summary>
    /// Bắn đạn từ tất cả điểm spawn (bulletSpawnPoints). Spawn đồng thời tại mỗi điểm.
    /// Được gọi tự động khi isShooting = true hoặc gọi trực tiếp từ UI Button.
    /// </summary>
    public void TryShootBullet()
    {
        if (bulletPrefab == null) return;
        if (bulletTimer < bulletCooldown) return;

        bulletTimer = 0f;

        // Lấy danh sách điểm spawn: ưu tiên bulletSpawnPoints, không có thì dùng bombSpawnPoint
        List<Transform> spawnPoints = new List<Transform>();

        if (bulletSpawnPoints != null && bulletSpawnPoints.Count > 0)
        {
            foreach (var point in bulletSpawnPoints)
            {
                if (point != null)
                    spawnPoints.Add(point);
            }
        }

        // Fallback: dùng bombSpawnPoint nếu không có bullet spawn points
        if (spawnPoints.Count == 0 && bombSpawnPoint != null)
        {
            spawnPoints.Add(bombSpawnPoint);
        }

        // Nếu vẫn không có điểm nào, spawn ở phía trước máy bay
        if (spawnPoints.Count == 0)
        {
            spawnPoints.Add(null); // Dùng để spawn tại vị trí mặc định
        }

        // Bullet di chuyển theo transform.right → cần xoay prefab sao cho right = hướng máy bay (forward)
        //Quaternion rot = Quaternion.FromToRotation(Vector3.right, transform.forward);
        //Quaternion rot = Quaternion.LookRotation(transform.forward);

        // Spawn đạn tại tất cả các điểm
        foreach (var spawnPoint in spawnPoints)
        {
            //Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * 1f;
            //GameObject go = Instantiate(bulletPrefab, pos, rot);
            // Bullet (speed, damage) dùng giá trị mặc định trên prefab

            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * 1f;

            Quaternion baseRot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

            Quaternion finalRot = baseRot * Quaternion.Euler(30f, 0f, 0f);
            GamePlaySoudVFX.Instance.PlayerFire();

            Instantiate(bulletPrefab, pos, finalRot);

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TankShot"))
        {
            //GameObject vfx =  Instantiate(explosionFX, transform.position, Quaternion.identity);
            //Destroy(vfx, 0.5f);
            //Destroy(gameObject);
            //LevelManager.Instance.GameOver();
            PlayerHealth player = gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(10f);
                DamageShow dameNum = gameObject.GetComponent<DamageShow>();
                if (dameNum != null)
                {
                    //Debug.Log("HitDame");
                    dameNum.ShowDamage(10, gameObject.transform.position);
                }
                return;
            }
        }
        else if (collision.gameObject.CompareTag("CharShot"))
        {
            DamageShow dameNum = gameObject.GetComponent<DamageShow>();
            PlayerHealth player = gameObject.GetComponent<PlayerHealth>();
            if (dameNum != null)
            {
                player.TakeDamage(5f);
                dameNum.ShowDamage(5, gameObject.transform.position);
            }
        }
        else
        {
            PlayerHealth player = gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(100f);
                DamageShow dameNum = gameObject.GetComponent<DamageShow>();
                if (dameNum != null)
                {
                    Debug.Log("HitDame");
                    dameNum.ShowDamage(100, gameObject.transform.position);
                }
                return;
            }
        }
    }
}
