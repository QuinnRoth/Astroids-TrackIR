using UnityEngine;

public class MainMenuAsteroidSpawner : MonoBehaviour
{
    [Header("Spawn Position")]    // Parent of all asteroids spawned
    [SerializeField] private Transform parentOfAsteroids;
    [SerializeField] private int spawnHeightMin = -350;    // Height range min where asteroids can spawn
    [SerializeField] private int spawnHeightMax = 350;    // Height range max where asteroids can spawn
    [SerializeField] private int spawnPosXLeft = -750;
    [SerializeField] private int spawnPosXRight = 750;
    [SerializeField] private int spawnDepthMin = 100;     // Depth range min where asteroids can spawn
    [SerializeField] private int spawnDepthMax = 200;     // Depth range max where asteroid scan spawn


    [Header("Asteroid Types")]    // Different types of asteroids to spawn
    [SerializeField] private GameObject basicAsteroidPrefab;
    [SerializeField] private GameObject healingAsteroidPrefab;
    [SerializeField] private GameObject bombAsteroidPrefab;


    [Header("Asteroid Size")]    // Size range of asteroids
    [SerializeField] private float minAsteroidSize = 20f;
    [SerializeField] private float maxAsteroidSize = 50f;


    [Header("Asteroid Move Speed")]    // Move speed range of asteroids
    [SerializeField] private float minAsteroidMoveSpeed = 14.0f;
    [SerializeField] private float maxAsteroidMoveSpeed = 20.0f;
 

    [Header("Asteroid Rotation Speed")]    // Rotation speed range of asteroids
    [SerializeField] private float minAsteroidRotSpeed = 1.0f;
    [SerializeField] private float maxAsteroidRotSpeed = 100.0f;

    [Header("Asteroid Materials")]
    [SerializeField] private Material basicMaterial;
    [SerializeField] private Material bombMaterial;
    [SerializeField] private Material bombFuseMaterial;
    [SerializeField] private Material healMaterial;

    [Header("Spawn Timer")]
    private float time = 0.0f;
    [SerializeField] private float spawnTime = 10.0f;

    void Start()
    {
        SpawnXAsteroids(2);
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >= spawnTime)
        {
            SpawnXAsteroids(2);
            time = 0.0f;
        }
    }

    // Spawns X amount of asteroids in the spawn radius
    public void SpawnXAsteroids(int x)
    {
        for (int i = 0; i < x; i++)
        {
            Vector3 moveDir;
            bool moveLeft = Random.Range(0, 2) == 1;
            float startPosX;
            if (moveLeft)
            {
                moveDir = new Vector3(1.0f, Random.Range(-0.5f, 0.5f), 0.0f);
                startPosX = spawnPosXLeft;
            }
            else
            {
                moveDir = new Vector3(-1.0f, Random.Range(-0.5f, 0.5f), 0.0f);
                startPosX = spawnPosXRight;
            }
            
            // Prepare asteroid to be instantiated
            GameObject asteroid = null;
            int randomSize = (int)UnityEngine.Random.Range(minAsteroidSize, maxAsteroidSize);
            float randomMoveSpeed = UnityEngine.Random.Range(minAsteroidMoveSpeed, maxAsteroidMoveSpeed);
            Vector3 randomRotDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
            Vector3 randomPosition = new Vector3(startPosX, Random.Range(spawnHeightMin, spawnHeightMax), Random.Range(spawnDepthMin, spawnDepthMax));

            // Generate a random number [0-20) and use that number for chance calculations
            int randChance = UnityEngine.Random.Range(0, 20);

            // Spawn asteroids based on chance variable, set their position and rotation, and add them as a child of the chosen parent
            switch (randChance)
            {
                case 1: // Healing Asteroid, 5% of the time
                    asteroid = Instantiate(healingAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    asteroid.transform.GetChild(0).GetComponent<MeshRenderer>().material = healMaterial;
                    break;
                case 2: // Bomb asteroid, 5% of the time
                    asteroid = Instantiate(bombAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    asteroid.transform.GetChild(0).GetComponent<MeshRenderer>().material = bombMaterial;
                    asteroid.transform.GetChild(1).GetComponent<MeshRenderer>().material = bombFuseMaterial;
                    break;
                default: // Basic asteroid, 90% of the time
                    asteroid = Instantiate(basicAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    asteroid.transform.GetChild(0).GetComponent<MeshRenderer>().material = basicMaterial;

                    // Apply the saved color scheme to the newly spawned asteroid
                    var apply = asteroid.GetComponent<ApplySavedColors>();
                    if (apply != null && asteroid.GetComponent<AsteroidClass>().GetAsteroidType() == AsteroidClass.InheritanceType.Basic)
                        apply.ApplyNow();
                    else
                        Debug.LogError("Cannot find ApplySavedColors component on " + asteroid);

                    break;
            }

            // Initialize asteroid, which is the same process for all asteroid types
            asteroid.GetComponent<AsteroidClass>().Init(
                /* iSize = */       randomSize,
                /* iMoveSpeed = */  randomMoveSpeed,
                /* iMoveDir = */    moveDir,
                /* iRotDir = */     randomRotDir
            );

            // Disable ghost asteroids and boundary script in main menu
            asteroid.GetComponent<GhostBoundary>().enabled = false;
            asteroid.GetComponent<Boundary>().enabled = false;
        }
    }
}