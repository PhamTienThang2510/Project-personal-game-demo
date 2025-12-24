using UnityEngine;

[System.Serializable]
public struct WaveSegment
{
    public GameObject prefab;
    public int enemyCount;
    public float spawnDelay;
}

[System.Serializable]
public struct Wave
{
    public string waveName;
    public WaveSegment[] segments;
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Reference")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField, Range(0f, 360f)] private float arcAngle = 120f;
    [SerializeField] private bool spawnInFrontOfPlayer = true;

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    public int CurrentWaveIndex => currentWaveIndex;
    private int currentWaveIndex;
    private int currentSegmentIndex;

    private int spawnedInSegment;
    private int aliveEnemyCount;

    private float spawnTimer;

    private PlayerController playerController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (waves == null || waves.Length == 0) return;

        Wave wave = waves[currentWaveIndex];
        WaveSegment segment = wave.segments[currentSegmentIndex];

        spawnTimer += Time.deltaTime;

        // ===== SPAWN THEO SEGMENT =====
        if (spawnedInSegment < segment.enemyCount &&
            spawnTimer >= segment.spawnDelay)
        {
            spawnTimer = 0f;
            SpawnEnemy(segment.prefab);
            spawnedInSegment++;
        }

        // ===== SANG SEGMENT TIẾP =====
        if (spawnedInSegment >= segment.enemyCount)
        {
            MoveToNextSegmentIfNeeded();
        }

        // ===== KẾT THÚC WAVE =====
        if (AllSegmentsSpawned() && aliveEnemyCount == 0)
        {
            NextWave();
        }
    }

    // =========================
    // SEGMENT / WAVE LOGIC
    // =========================

    private void MoveToNextSegmentIfNeeded()
    {
        if (currentSegmentIndex < waves[currentWaveIndex].segments.Length - 1)
        {
            currentSegmentIndex++;
            spawnedInSegment = 0;
            spawnTimer = 0f;
        }
    }

    private bool AllSegmentsSpawned()
    {
        return currentSegmentIndex >= waves[currentWaveIndex].segments.Length - 1 &&
               spawnedInSegment >= waves[currentWaveIndex].segments[currentSegmentIndex].enemyCount;
    }

    private void NextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
            currentWaveIndex = 0;

        currentSegmentIndex = 0;
        spawnedInSegment = 0;
        aliveEnemyCount = 0;
        spawnTimer = 0f;
    }

    // =========================
    // REGISTER / UNREGISTER
    // =========================

    public void RegisterEnemy()
    {
        aliveEnemyCount++;
    }

    public void UnregisterEnemy()
    {
        aliveEnemyCount--;
        if (aliveEnemyCount < 0)
            aliveEnemyCount = 0;
    }

    // =========================
    // SPAWN
    // =========================

    private void SpawnEnemy(GameObject prefab)
    {
        if (player == null || prefab == null) return;

        Vector2 dir = GetArcDirection();
        Vector3 pos = player.position + (Vector3)dir * spawnRadius;

        PoolManager.Instance.Spawn(prefab.name, pos);
        RegisterEnemy();
    }

    private Vector2 GetArcDirection()
    {
        float halfArc = arcAngle * 0.5f;
        float randomAngle = Random.Range(-halfArc, halfArc);

        float baseAngle = 0f;

        if (spawnInFrontOfPlayer && playerController != null)
        {
            Vector2 moveDir = playerController.PlayerDirection;
            if (moveDir != Vector2.zero)
                baseAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        }

        float rad = (baseAngle + randomAngle) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
