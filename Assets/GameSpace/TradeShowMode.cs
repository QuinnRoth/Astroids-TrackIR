using UnityEngine;

public class TradeShowMode : MonoBehaviour
{
    // Trade show mode changes also in `Boundary.cs`, `BoundaryWarningFade.cs`, and `SpaceshipMovement.cs`

    [SerializeField] private AsteroidSpawner asteroidSpawner;

    [SerializeField] private readonly int spawnNumber = 1;

    // Spawn settings
    // Starts a little easier, progresses faster, higher top end 
    [Header("Spawner Settings")]
    [SerializeField] float spawnTimerMin = 2;
    [SerializeField] float reachSpawnTimerMin = 30;
    [SerializeField] float initSpawnTimer = 7;
    [SerializeField] int initSpawnAmount = 8;


    private float spawnTimer = 0;
    private float curTime = 0.0f;

    private float spawnEquationM;

    void Start()
    {
        spawnEquationM = (spawnTimerMin - initSpawnTimer) / reachSpawnTimerMin;

        asteroidSpawner.SpawnXAsteroidsNotTowardOrigin(initSpawnAmount - spawnNumber);
    }

    void Update()
    {
        curTime += Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0.0f)
        {
            // Debug.Log("Timer reached 0, spawning an asteroid");

            // Reset Spawn timer based on the equation
            spawnTimer = (spawnEquationM * curTime) + initSpawnTimer;

            if (spawnTimer < spawnTimerMin)
                spawnTimer = spawnTimerMin;

            asteroidSpawner.SpawnXAsteroids(spawnNumber);
        }
    }
}
