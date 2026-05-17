using UnityEngine;

public class AsteroidClass : MonoBehaviour
{
    public enum InheritanceType
    {
        Basic,
        Healing,
        Bomb
    }

    // Explosions will be used on all asteroid types, so they can be set up here
    [SerializeField] protected ExplosionParticleVFX explosionVFX;
    
    [SerializeField] protected InheritanceType type;
    protected int size;
    protected float moveSpeed;
    protected float rotSpeed;
    protected Vector3 moveDir;
    protected Vector3 rotDir;

    public bool hitByLaser = false;
    private bool isDying = false;

    private float lifetime = 0.0f;
    private float maxLifetime = 240.0f; // four minutes

    public void Init(int iSize, 
                     float iMoveSpeed, 
                     Vector3 iMoveDir, 
                     Vector3 iRotDir)
    {
        size = iSize;
        moveSpeed = iMoveSpeed;
        moveDir = iMoveDir;
        rotDir = iRotDir;

        rotSpeed = Random.Range(-2.0f, 2.0f) * moveSpeed;

        transform.localScale = new Vector3(size, size, size);
    }

    // Move the asteroid each frame while accounting for framerate
    private void Update()
    {
        transform.Rotate(rotDir * rotSpeed * Time.deltaTime);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Debug.Log("Lifetime Expired");
            Destroy(gameObject);
        }
    }

    public InheritanceType GetAsteroidType()
    {
        return type;
    }

    protected bool TryBeginDeath()
    {
        if (isDying)
            return false;

        isDying = true;
        return true;
    }

    protected void SpawnDeathExplosion()
    {
        if (explosionVFX == null)
        {
            Debug.LogWarning($"{name} has no explosion VFX assigned.", this);
            return;
        }

        ExplosionParticleVFX explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        float explosionScale = Mathf.Clamp(transform.lossyScale.x * 0.08f, 1f, 5f);
        explosion.transform.localScale = Vector3.one * explosionScale;
        explosion.PlayVFX();
    }

    // Displays the asteroid hitbox when Gimozs are turned on
    private void OnDrawGizmos()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider == null)
            return;

        Gizmos.color = Color.green;
        Vector3 worldCenter = transform.TransformPoint(collider.center);
        float worldRadius = collider.radius * transform.lossyScale.x;
        Gizmos.DrawWireSphere(worldCenter, worldRadius);
    }

    /*=== Virtual functions for inheritence ===*/
    public virtual void InitType(InheritanceType t) {}
    public virtual void Die(bool diedByBomb) {}
    public virtual void PlayDeathFX() {}
}
