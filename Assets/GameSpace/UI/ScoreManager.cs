using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float score = 0;
    private int count = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (GameModeMenu.gameModeSetting == 2 && AsteroidSpawner.asteroidCount > 0)  // wave mode (time based score)
            score -= Time.deltaTime;
    }

    public void AddScore(int amount)
    {
        score += amount + count;
        count++;
        // Debug.Log("Score: " + score);
    }

    public void AddExactScore(int amount)
    {
        score += amount;
        // Debug.Log("Score " + score);
    }

    public int GetScore() => (int)score;
}
