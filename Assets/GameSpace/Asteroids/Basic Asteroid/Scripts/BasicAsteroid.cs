using UnityEngine;

public class BasicAsteroid : AsteroidClass
{
    // Asteroid prefab for splitting
    [SerializeField] private GameObject asteroidPrefab;

    // Number of times the asteroid can split
    public int splitTotal = 3;
    public int curSplitNum = 3;

    // Number of asteroids the asteroid can split into
    [SerializeField] private int splitSize = 2;

    // Set the type of the asteroid
    public override void InitType(InheritanceType t)
    {
        type = t;
    }

    // Override the die method to fit the asteroid type (basic)
    public override void Die(bool diedByBomb)
    {
        if (!TryBeginDeath())
            return;
        
        // Debug.Log("Basic Asteroid Hit!");
        if (GameModeMenu.gameModeSetting == 2)      // scoring works differently in wave mode
            ScoreManager.Instance.AddExactScore(10);
        else
            ScoreManager.Instance.AddScore(1);

        AsteroidSpawner.asteroidCount--;
        PlayDeathFX();

        // Don't split if the asteroid was exploded by a bomb asteroid
        if (!diedByBomb)
            Split();

        Destroy(gameObject);
    }

    // Override the fx method to fit the asteroid type (basic)
    public override void PlayDeathFX()
    {
        SpawnDeathExplosion();
        SoundManager.PlaySound(SoundType.EXPLOSION);
    }

    // Split the asteroid on death into two new asteroids
    private void Split()
    {
        // Guard statement, no more splitting once split number has been exhausted
        if (curSplitNum - 1 == 0)
            return;

        // Spawn two asteroids
        for (int i = 0; i < splitSize; i++)
        {
            // Prepare for child asteroid instantiation
            // int newSize = size - ((size / 3) * (curSplitNum - (curSplitNum - 1))); // Size will be reduced for each split
            int newSize = size * 2 / 3;     // the previous line simplified
            // int newSize = size * ((curSplitNum - 1) / splitTotal);

            float randomMoveSpeed = Random.Range(16.0f, 24.0f);
            Vector3 randomMoveDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            Vector3 randomRotDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            Vector3 leftPosition = new Vector3(this.transform.position.x - size, this.transform.position.y, this.transform.position.z);
            Vector3 rightPosition = new Vector3(this.transform.position.x + size, this.transform.position.y, this.transform.position.z);

            // Spawn child asteroid at the parent like we do in the asteroid spawner script, setting its parent to this objects parent
            GameObject asteroid = Instantiate(asteroidPrefab, leftPosition, Quaternion.identity, this.transform.parent);

            // Initialize child asteroid
            asteroid.GetComponent<AsteroidClass>().Init(newSize, randomMoveSpeed, randomMoveDir, randomRotDir);

            // Increment asteroid counter
            AsteroidSpawner.asteroidCount++;

            // Reduce the split number of the asteroid
            asteroid.GetComponent<BasicAsteroid>().curSplitNum -= 1;

            // Apply the saved color scheme to the newly spawned asteroid
            var apply = asteroid.GetComponent<ApplySavedColors>();
            if (apply != null)
                apply.ApplyNow();
        }
    }
}
