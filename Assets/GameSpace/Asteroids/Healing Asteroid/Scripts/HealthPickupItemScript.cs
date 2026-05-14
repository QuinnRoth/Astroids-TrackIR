using UnityEngine;

public class HealthPickupItem : MonoBehaviour
{
    private float rotationsPerMinute = 10.0f;

    private void Update()
    {
        transform.Rotate(0.0f, 6.0f * rotationsPerMinute * Time.deltaTime, 0.0f);
    }

    private void OnTriggerEnter(Collider collider) 
    {
        if (collider.CompareTag("Spaceship"))
        {
            collider.GetComponentInParent<SpaceshipDamage>().GainHealth();
            Destroy(gameObject);
        }
    }
}
