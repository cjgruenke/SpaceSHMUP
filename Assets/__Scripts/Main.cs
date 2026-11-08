using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Attached to GameManager. Spawns enemies, runs waves and bosses, keeps score, and handles Game Over.
public class Main : MonoBehaviour {
    static private Main S;

    [Header("Set in Inspector: Enemies")]
    public GameObject[] prefabEnemies;         // Enemy_0 to Enemy_3
    public GameObject prefabBoss;              // Enemy_4
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;

    [Header("Set in Inspector: Waves")]
    public float waveDuration = 30f;           // Seconds per wave
    public float bossDelay = 15f;              // Seconds into each wave before the boss appears
    public float spawnRateIncrease = 1.25f;    // Spawn rate multiplier for each new wave

    [Header("Set in Inspector: UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI announceText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverStatsText;

    [Header("Set Dynamically")]
    public int score = 0;
    public int highScore = 0;
    public int wave = 0;
    public bool gameOver = false;

    private float camWidth;
    private float camHeight;
    private float waveStartTime;
    private bool bossSpawnedThisWave = false;
    private GameObject boss;

    void Awake() {
        S = this;
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;

        highScore = PlayerPrefs.GetInt("ShmupHighScore", 0);
        gameOverPanel.SetActive(false);
        announceText.gameObject.SetActive(false);

        StartWave(1);
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    void Update() {
        if (!gameOver) {
            float t = Time.time - waveStartTime;

            if (!bossSpawnedThisWave && t >= bossDelay) {
                SpawnBoss();
                bossSpawnedThisWave = true;
            }

            if (t >= waveDuration) {
                StartWave(wave + 1);
            }
        }

        UpdateUI();
    }

    void StartWave(int newWave) {
        wave = newWave;
        waveStartTime = Time.time;
        bossSpawnedThisWave = false;

        // Each new wave spawns enemies faster
        if (wave > 1) {
            enemySpawnPerSecond *= spawnRateIncrease;
        }

        Announce("WAVE " + wave);
    }

    void SpawnEnemy() {
        if (gameOver) return;

        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]);
        PlaceAtTop(go);

        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    void SpawnBoss() {
        if (boss != null) return;   // Only one boss at a time

        boss = Instantiate(prefabBoss);
        PlaceAtTop(boss);
        Announce("BOSS INCOMING!");
    }

    // Puts an enemy at a random x position just above the top of the screen
    void PlaceAtTop(GameObject go) {
        float inset = enemyInsetDefault;
        BoundsCheck bc = go.GetComponent<BoundsCheck>();
        if (bc != null) inset = Mathf.Abs(bc.radius);

        float xMin = -camWidth + inset;
        float xMax = camWidth - inset;
        go.transform.position = new Vector3(Random.Range(xMin, xMax), camHeight + inset, 0);
    }

    void Announce(string message) {
        announceText.text = message;
        announceText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideAnnounce));
        Invoke(nameof(HideAnnounce), 2f);
    }

    void HideAnnounce() {
        announceText.gameObject.SetActive(false);
    }

    void UpdateUI() {
        scoreText.text = "Score: " + score;
        highScoreText.text = "High: " + Mathf.Max(score, highScore);
        waveText.text = "Wave " + wave;

        int shield = (Hero.S != null) ? Hero.S.shieldLevel : 0;
        shieldText.text = "Shield: " + Mathf.Max(0, shield);
    }

    void GameOver() {
        gameOver = true;
        CancelInvoke(nameof(SpawnEnemy));

        bool newBest = score > highScore;
        if (newBest) {
            highScore = score;
            PlayerPrefs.SetInt("ShmupHighScore", highScore);
            PlayerPrefs.Save();
        }

        gameOverStatsText.text = "Score: " + score
            + "\nWave reached: " + wave
            + "\n" + (newBest ? "NEW HIGH SCORE!" : "High Score: " + highScore);

        announceText.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    // Hooked up to RestartButton's On Click ()
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called by Enemy when it is destroyed by the player's shots
    static public void ENEMY_DESTROYED(Enemy e) {
        if (S == null || S.gameOver) return;
        S.score += e.score;
    }

    // Called by Hero when it is destroyed
    static public void HERO_DIED() {
        if (S == null) return;
        S.Invoke(nameof(GameOver), 1f);
    }
}