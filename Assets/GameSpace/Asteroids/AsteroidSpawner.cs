using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Spawn Position")]    // Parent of all asteroids spawned
    [SerializeField] private Transform parentOfAsteroids;
    [SerializeField] private Transform spaceshipTransform;
    [SerializeField] private Camera mainCamera;


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
    
    private int gameModeSetting;

    public void StartGame()
    {
        Debug.Log("Starting game...");
        asteroidCount = 0;

        gameModeSetting = GameModeMenu.gameModeSetting;

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

    public void RemoveAllAsteroids(){
        foreach (Transform child in parentOfAsteroids)
        {
            AsteroidClass asteroid = child.TryGetComponent<AsteroidClass>(out AsteroidClass a) ? a : null;
            if (asteroid != null)
            {
                asteroid.Die(true);
            }
        }
    }

    // Generates a random spawn position on a cube face while avoiding the spaceship and camera view
    private Vector3 RangeWithExclusionZone(float half, float exclusionRadius)
    {
        Vector3 randomPosition;

        // DEBUG
        bool attemptplus = false;

        do
        {
            if (attemptplus)
                Debug.Log("Attempting to spawn asteroid in exclusion zone");
            attemptplus = true;

            int face = UnityEngine.Random.Range(0, 6);

            switch (face)
            {
                case 0: // +X face
                    randomPosition = new Vector3(half, UnityEngine.Random.Range(-half, half), UnityEngine.Random.Range(-half, half));
                    break;
                case 1: // -X face
                    randomPosition = new Vector3(-half, UnityEngine.Random.Range(-half, half), UnityEngine.Random.Range(-half, half));
                    break;
                case 2: // +Y face
                    randomPosition = new Vector3(UnityEngine.Random.Range(-half, half), half, UnityEngine.Random.Range(-half, half));
                    break;
                case 3: // -Y face
                    randomPosition = new Vector3(UnityEngine.Random.Range(-half, half), -half, UnityEngine.Random.Range(-half, half));
                    break;
                case 4: // +Z face
                    randomPosition = new Vector3(UnityEngine.Random.Range(-half, half), UnityEngine.Random.Range(-half, half), half);
                    break;
                default: // -Z face
                    randomPosition = new Vector3(UnityEngine.Random.Range(-half, half), UnityEngine.Random.Range(-half, half), -half);
                    break;
            }

            if (gameModeSetting == 0)
                randomPosition /= 2f;
        }
        while (
            (randomPosition - spaceshipTransform.position).sqrMagnitude < exclusionRadius * exclusionRadius ||
            IsInsideCameraView(randomPosition)
        );

        return randomPosition;
    }

    private bool IsInsideCameraView(Vector3 worldPosition)
    {
        if (!mainCamera)
            return false;

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(worldPosition);
        if (viewportPosition.z <= 0f)
            return false;

        const float viewPadding = 0.05f;

        return viewportPosition.x > viewPadding && viewportPosition.x < 1f - viewPadding &&
               viewportPosition.y > viewPadding && viewportPosition.y < 1f - viewPadding;
    }

    // Spawns X amount of asteroids in the spawn radius
    public void SpawnXAsteroids(int x)
    {
        for (int i = 0; i < x; i++)
        {

            float half = 200f;
            float exclusionRadius = half / 2f;
            Vector3 randomPosition = RangeWithExclusionZone(half, exclusionRadius);

            // Generate a random number [0-20] and use that number for chance calculations
            int randChance = UnityEngine.Random.Range(0, 20);

            // Spawn asteroids based on chance variable, set their position and rotation, and add them as a child of the chosen parent
            SpawnOneAsteroid(randChance, randomPosition);

            // Debug.Log(asteroid.GetComponent<AsteroidClass>().GetAsteroidType());
        }
    }

    public GameObject SpawnOneAsteroid(int asteroidType, Vector3 position)
    {
        GameObject asteroid = null;
        int randomSize = (int)UnityEngine.Random.Range(minAsteroidSize, maxAsteroidSize);
        float randomMoveSpeed = UnityEngine.Random.Range(minAsteroidMoveSpeed, maxAsteroidMoveSpeed);
        float randomRotSpeed = UnityEngine.Random.Range(minAsteroidRotSpeed, maxAsteroidRotSpeed);
        Vector3 randomMoveDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
        Vector3 randomRotDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;


        switch (asteroidType)
        {
            case 1: // Healing Asteroid
                asteroid = SpawnHealingAsteroid(position);
                break;
            case 2: // Bomb asteroid
                asteroid = SpawnBombAsteroid(position);
                break;
            default: // Basic asteroid
                asteroid = SpawnBasicAsteroid(asteroid, position);
                break;
        }

        // Initialize asteroid, which is the same process for all asteroid types
        asteroid.GetComponent<AsteroidClass>().Init(
            /* iSize = */       randomSize,
            /* iMoveSpeed = */  randomMoveSpeed,
            /* iMoveDir = */    randomMoveDir,
            /* iRotDir = */     randomRotDir
        );  

        asteroidCount++;

        return asteroid;
    }

    public GameObject SpawnBasicAsteroid(GameObject asteroid, Vector3 position)
    {
        asteroid =Instantiate(basicAsteroidPrefab, position, Quaternion.identity, parentOfAsteroids);
                            // Apply the saved color scheme to the newly spawned asteroid
        var apply = asteroid.GetComponent<ApplySavedColors>();
        if (apply != null && asteroid.GetComponent<AsteroidClass>().GetAsteroidType() == AsteroidClass.InheritanceType.Basic)
            apply.ApplyNow();
        else
            Debug.LogError("Cannot find ApplySavedColors component on " + asteroid);
        
        return asteroid;
    }

    public GameObject SpawnHealingAsteroid(Vector3 position)
    {
        return Instantiate(healingAsteroidPrefab, position, Quaternion.identity, parentOfAsteroids);
    }

    public GameObject SpawnBombAsteroid(Vector3 position)
    {
        return Instantiate(bombAsteroidPrefab, position, Quaternion.identity, parentOfAsteroids);
    }

    // Spawn a set of tutorial asteroids
    public void SpawnTutorialAstroids()
    {
        SpawnXAsteroidsNotTowardOrigin(5);
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
            float exclusionRadius = half / 2f;
            Vector3 randomPosition = RangeWithExclusionZone(half, exclusionRadius);

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

                    // Apply the saved color scheme to the newly spawned asteroid
                    var apply = asteroid.GetComponent<ApplySavedColors>();
                    if (apply != null && asteroid.GetComponent<AsteroidClass>().GetAsteroidType() == AsteroidClass.InheritanceType.Basic)
                        apply.ApplyNow();
                    else
                        Debug.LogError("Cannot find ApplySavedColors component on " + asteroid);

                    break;
            }
            
            /* Detect if movement direction is intersecting origin of scene */
            Vector3 toOrigin = Vector3.zero - randomPosition;
            float thresholdAngle = 5.0f; // Tolerance for angle
            Vector3 randomMoveDir = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 
                                                UnityEngine.Random.Range(-1.0f, 1.0f),
                                                UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
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
}
