using UnityEngine;
using TMPro;

public class EnemySpawnerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timePlayedText;
    [SerializeField] private TextMeshProUGUI wavePassedText;

    private float timePlayed;

    private EnemySpawner spawner;

    private void Start()
    {
        spawner = EnemySpawner.Instance;
        timePlayed = 0f;
    }

    private void Update()
    {
        UpdateTimePlayed();
        UpdateWavePassed();
    }

    // =========================
    // TIME PLAYED
    // =========================
    private void UpdateTimePlayed()
    {
        timePlayed += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timePlayed / 60f);
        int seconds = Mathf.FloorToInt(timePlayed % 60f);

        timePlayedText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    // =========================
    // WAVE PASSED
    // =========================
    private void UpdateWavePassed()
    {
        if (spawner == null) return;

        int wavePassed = spawner.CurrentWaveIndex;
        wavePassedText.text = $"Wave Passed: {wavePassed}";
    }
}
