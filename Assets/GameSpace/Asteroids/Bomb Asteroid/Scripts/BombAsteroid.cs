using System.Collections.Generic;
using UnityEngine;

public class BombAsteroid : AsteroidClass
{
    // Radius of field to check for asteroids when exploding
    [SerializeField] private int explosionRadius = 5;

    // Set the type of the asteroid
    public override void InitType(InheritanceType t)
    {
        type = t;
    }

    // Override the die method to fit the asteroid type (bomb)
    public override void Die(bool diedByBomb)
    {
        if (!TryBeginDeath())
            return;

        // Debug.Log("Bomb Asteroid Hit!");
        if (GameModeMenu.gameModeSetting == 2)      // scoring works differently in wave mode
            ScoreManager.Instance.AddExactScore(10);
        else
            ScoreManager.Instance.AddScore(1);
            
        AsteroidSpawner.asteroidCount--;
        PlayDeathFX();

        // Don't explode other astroids if this asteroid was exploded
        if (!diedByBomb)
            Explode();

        Destroy(gameObject);
    }

    // Override the fx method to fit the asteroid type (bomb)
    public override void PlayDeathFX()
    {
        SpawnDeathExplosion();
        SoundManager.PlaySound(SoundType.EXPLOSION);
    }

    // Explode asteroids in a radius around itself when destroyed
    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<AsteroidClass> hitAsteroids = new HashSet<AsteroidClass>();
        int i = 0;
        foreach (Collider hitCollider in hitColliders)
        {
            AsteroidClass asteroid = hitCollider.GetComponentInParent<AsteroidClass>();
            if (asteroid == null) continue;
            if (!hitAsteroids.Add(asteroid)) continue;
            // Debug.Log("DEBUG: Hit collider " + i  + ": " + hitCollider);
            if (asteroid.gameObject == this.gameObject) continue;
            asteroid.Die(true); // diedByBomb = true
            i++;
        }
    }
}
