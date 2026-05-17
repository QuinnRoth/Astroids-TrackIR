using UnityEngine;

public class HealingAsteroid : AsteroidClass
{
    // Health item that drops when the asteroid is destroyed
    [SerializeField] private GameObject healthPickupItemPrefab;

    // Set the type of the asteroid
    public override void InitType(InheritanceType t)
    {
        type = t;
    }

    // Override the die method to fit the asteroid type (healing)
    public override void Die(bool diedByBomb)
    {
        if (!TryBeginDeath())
            return;

        Debug.Log("Healing Asteroid Hit!");
        ScoreManager.Instance.AddScore(1);
        AsteroidSpawner.asteroidCount--;
        PlayDeathFX();

        SpawnHealItem();

        Destroy(gameObject);
    }

    // Override the fx method to fit the asteroid type (healing)
    public override void PlayDeathFX()
    {
        SpawnDeathExplosion();
        SoundManager.PlaySound(SoundType.EXPLOSION);
    }

    private void SpawnHealItem()
    {
        GameObject healthPickupItem = Instantiate(healthPickupItemPrefab, transform.position, Quaternion.identity);
    }
}
