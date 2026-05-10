using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class SpaceshipBounce : MonoBehaviour
{
    [SerializeField] private float bounceForce = 10f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("here");
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Debug.Log(gameObject + " Collision with " + collision.gameObject);

            // get the contact normal
            ContactPoint contact = collision.GetContact(0);
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, contact.normal);

            // stop and then bounce
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(reflectDir * bounceForce, ForceMode.Impulse);
        }
    }
}
