using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Spawn Position")]    // Parent of all asteroids spawned
    [SerializeField] private Transform parentOfAsteroids;
    // [SerializeField] private int spawnRadius = 100;    // Radius of perimiter where asteroids can spawn


    [Header("Asteroid Types")]    // Different types of asteroids to spawn
    [SerializeField] private GameObject basicAsteroidPrefab;
    [SerializeField] private GameObject healingAsteroidPrefab;
    [SerializeField] private GameObject bombAsteroidPrefab;


    [Header("Asteroid Size")]    // Size range of asteroids
    [SerializeField] private float minAsteroidSize = 20f;
    [SerializeField] private float maxAsteroidSize = 50f;


    [Header("Asteroid Move Speed")]    // Move speed range of asteroids
    [SerializeField] private float minAsteroidMoveSpeed = 20f;
    [SerializeField] private float maxAsteroidMoveSpeed = 30f;
 

    [Header("Asteroid Rotation Speed")]    // Rotation speed range of asteroids
    [SerializeField] private float minAsteroidRotSpeed = 1.0f;
    [SerializeField] private float maxAsteroidRotSpeed = 100.0f;


    [Header("Game Mode Scripts")]
    public GameObject TradeShowMode;
    public GameObject EndlessMode;
    public GameObject WaveMode;
    
    // keeps a count of how many asteroids there are
    // incs in SpawnXAsteroids function and Basic Asteroid's Split function, decs in each Asteroid's Die function
    public static int asteroidCount = 0;
    
    private int gameModeSetting = GameModeMenu.gameModeSetting;

    void Start()
    {
        // for testing different game modes in scene
        // gameModeSetting = 2;

        switch (gameModeSetting)
        {
            case 0:
                TradeShowModeOn();
                break;
            
            case 1:
                EndlessModeOn();
                break;

            case 2:
                WaveModeOn();
                break;
        }
    }

    private void TradeShowModeOn()
    {
        TradeShowMode.SetActive(true);
    }

    private void EndlessModeOn()
    {
        EndlessMode.SetActive(true);
    }

    private void WaveModeOn()
    {
        WaveMode.SetActive(true);
    }

    // Spawns X amount of asteroids in the spawn radius
    public void SpawnXAsteroids(int x)
    {
        for (int i = 0; i < x; i++)
        {
            // Prepare asteroid to be instantiated
            GameObject asteroid = null;
            int randomSize = (int)UnityEngine.Random.Range(minAsteroidSize, maxAsteroidSize);
            float randomMoveSpeed = UnityEngine.Random.Range(minAsteroidMoveSpeed, maxAsteroidMoveSpeed);
            float randomRotSpeed = UnityEngine.Random.Range(minAsteroidRotSpeed, maxAsteroidRotSpeed);
            Vector3 randomMoveDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
            Vector3 randomRotDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
            float half = 200f;

            int face = UnityEngine.Random.Range(0, 6);
            
            Vector3 randomPosition = Vector3.zero;
            
            switch (face)
            {
                case 0: // +X face
                    randomPosition = new Vector3(half, Random.Range(-half, half), Random.Range(-half, half));
                    break;
                case 1: // -X face
                    randomPosition = new Vector3(-half, Random.Range(-half, half), Random.Range(-half, half));
                    break;
                case 2: // +Y face
                    randomPosition = new Vector3(Random.Range(-half, half), half, Random.Range(-half, half));
                    break;
                case 3: // -Y face
                    randomPosition = new Vector3(Random.Range(-half, half), -half, Random.Range(-half, half));
                    break;
                case 4: // +Z face
                    randomPosition = new Vector3(Random.Range(-half, half), Random.Range(-half, half), half);
                    break;
                case 5: // -Z face
                    randomPosition = new Vector3(Random.Range(-half, half), Random.Range(-half, half), -half);
                    break;
            }

            if (gameModeSetting == 0)
                randomPosition /= 2;

            // Generate a random number [0-20) and use that number for chance calculations
            int randChance = UnityEngine.Random.Range(0, 10);

            // Spawn asteroids based on chance variable, set their position and rotation, and add them as a child of the chosen parent
            switch (randChance)
            {
                case 1: // Healing Asteroid, 5% of the time
                    asteroid = Instantiate(healingAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    break;
                case 2: // Bomb asteroid, 5% of the time
                    asteroid = Instantiate(bombAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    break;
                default: // Basic asteroid, 90% of the time
                    asteroid = Instantiate(basicAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);

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
                /* iMoveDir = */    randomMoveDir,
                /* iRotDir = */     randomRotDir
            );

            // Debug.Log(asteroid.GetComponent<AsteroidClass>().GetAsteroidType());

            asteroidCount++;
        }
    }

    // Spawn X amount of asteroids in the spawn radius that *do not* target the player when spawned
    public void SpawnXAsteroidsNotTowardOrigin(int x)
    {
        for (int i = 0; i < x; i++)
        {
            // Prepare asteroid to be instantiated
            GameObject asteroid = null;
            int randomSize = (int)UnityEngine.Random.Range(minAsteroidSize, maxAsteroidSize);
            float randomMoveSpeed = UnityEngine.Random.Range(minAsteroidMoveSpeed, maxAsteroidMoveSpeed);
            Vector3 randomRotDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
            float half = 200f;

            int face = UnityEngine.Random.Range(0, 6);
            
            Vector3 randomPosition = Vector3.zero;
            
            /* Detect if movement direction is intersecting origin of scene */
            Vector3 toOrigin = Vector3.zero - asteroid.transform.position;          // problem here: asteroid is null so it has not .transform.position?
            float thresholdAngle = 5.0f; // Tolerance for angle
            Vector3 randomMoveDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
            float asteroidAngle = Vector3.Angle(randomMoveDir, toOrigin);
            if (asteroidAngle <= thresholdAngle)
            {
                int whichMoveDirToChange = Random.Range(0, 3);
                switch (whichMoveDirToChange)
                {
                    case 0:
                        randomMoveDir.x = -randomMoveDir.x;
                        break;
                    case 1:
                        randomMoveDir.y = -randomMoveDir.y;
                        break;
                    case 2:
                        randomMoveDir.z = -randomMoveDir.z;
                        break;
                }
            }

            switch (face)
            {
                case 0: // +X face
                    randomPosition = new Vector3(half, Random.Range(-half, half), Random.Range(-half, half));
                    break;
                case 1: // -X face
                    randomPosition = new Vector3(-half, Random.Range(-half, half), Random.Range(-half, half));
                    break;
                case 2: // +Y face
                    randomPosition = new Vector3(Random.Range(-half, half), half, Random.Range(-half, half));
                    break;
                case 3: // -Y face
                    randomPosition = new Vector3(Random.Range(-half, half), -half, Random.Range(-half, half));
                    break;
                case 4: // +Z face
                    randomPosition = new Vector3(Random.Range(-half, half), Random.Range(-half, half), half);
                    break;
                case 5: // -Z face
                    randomPosition = new Vector3(Random.Range(-half, half), Random.Range(-half, half), -half);
                    break;
            }
            
            if (gameModeSetting == 0)
                randomPosition /= 2;

            // Generate a random number [0-20) and use that number for chance calculations
            int randChance = UnityEngine.Random.Range(0, 20);

            // Spawn asteroids based on chance variable, set their position and rotation, and add them as a child of the chosen parent
            switch (randChance)
            {
                case 1: // Healing Asteroid, 5% of the time
                    asteroid = Instantiate(healingAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    break;
                case 2: // Bomb asteroid, 5% of the time
                    asteroid = Instantiate(bombAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    break;
                default: // Basic asteroid, 90% of the time
                    asteroid = Instantiate(basicAsteroidPrefab, randomPosition, Quaternion.identity, parentOfAsteroids);
                    break;
            }

            // Initialize asteroid, which is the same process for all asteroid types
            asteroid.GetComponent<AsteroidClass>().Init(
                /* iSize = */       randomSize,
                /* iMoveSpeed = */  randomMoveSpeed,
                /* iMoveDir = */    randomMoveDir,
                /* iRotDir = */     randomRotDir
            );

            // Apply the saved color scheme to the newly spawned asteroid
            var apply = asteroid.GetComponent<ApplySavedColors>();
            if (apply != null && asteroid.GetComponent<AsteroidClass>().GetAsteroidType() == AsteroidClass.InheritanceType.Basic)
                apply.ApplyNow();
            else
                Debug.LogError("Cannot find ApplySavedColors component on " + asteroid);
            
            // Debug.Log(asteroid.GetComponent<AsteroidClass>().GetAsteroidType());

            asteroidCount++;
        }
    }
}
