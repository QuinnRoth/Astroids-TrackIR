using UnityEngine;

public class WaveMode : MonoBehaviour
{
    public GameObject gameOverMenu;

    [SerializeField] private AsteroidSpawner asteroidSpawner;

    // // Spawn settings
    [Header("Spawner Settings")]
    [SerializeField] readonly int initWaveAmount = 1;
    [SerializeField] readonly int incWaveAmount = 3;
    [SerializeField] readonly int totalWaves = 1;

    private int waveNumber = 0;

    private bool gameFinished = false;

    void Update()
    {
        if (AsteroidSpawner.asteroidCount <= 0)
        {
            if (waveNumber < totalWaves)
            {
                // Debug.Log($"New wave: wave {waveNumber}");

                asteroidSpawner.SpawnXAsteroids((incWaveAmount * waveNumber) + initWaveAmount);
                waveNumber++;

                // add bonus score for completing a wave
                ScoreManager.Instance.AddExactScore(waveNumber * 100);
            }
            else{                //finish game mode
                // Debug.Log("Finshed wave mode");
                if (!gameFinished)
                {
                    gameFinished = true;
                    ScoreManager.Instance.AddExactScore(waveNumber * 200);          // bonus for the final wave

                    CursorInput.Instance.SetCursorVisible(true);
                    gameOverMenu.SetActive(true);
                }
            }
        }
        
    }
}
