using UnityEngine;

public class Boundary : MonoBehaviour
{
    public enum BoundaryMode
    {
        Wrap,
        Bounce,
        Clamp
    }

    [Header("Boundary Settings")]
    public Vector3 boxSize = new Vector3(600f, 600f, 600f);
    public BoundaryMode boundaryMode = BoundaryMode.Wrap;

    [Header("Bounce Settings")]
    [Range(0f, 1f)]
    public float bounceDamping = 0.9f;

    private Vector3 halfSize;
    private Rigidbody rb;

    void Start()
    {

        halfSize = boxSize * 0.5f;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        switch (boundaryMode)
        {
            case BoundaryMode.Wrap:
                Wrap(ref pos);
                transform.position = pos;
                break;

            case BoundaryMode.Bounce:
                Bounce(ref pos);
                transform.position = pos;
                break;

            case BoundaryMode.Clamp:
                Clamp(ref pos);
                transform.position = pos;
                break;
        }
    }

    void Wrap(ref Vector3 pos)
    {
        if (pos.x > halfSize.x) pos.x = -halfSize.x;
        else if (pos.x < -halfSize.x) pos.x = halfSize.x;

        if (pos.y > halfSize.y) pos.y = -halfSize.y;
        else if (pos.y < -halfSize.y) pos.y = halfSize.y;

        if (pos.z > halfSize.z) pos.z = -halfSize.z;
        else if (pos.z < -halfSize.z) pos.z = halfSize.z;
    }

    void Bounce(ref Vector3 pos)
    {
        if (!rb) return;

        Vector3 vel = rb.linearVelocity;

        if (Mathf.Abs(pos.x) > halfSize.x)
        {
            vel.x *= -bounceDamping;
            pos.x = Mathf.Sign(pos.x) * halfSize.x;
        }

        if (Mathf.Abs(pos.y) > halfSize.y)
        {
            vel.y *= -bounceDamping;
            pos.y = Mathf.Sign(pos.y) * halfSize.y;
        }

        if (Mathf.Abs(pos.z) > halfSize.z)
        {
            vel.z *= -bounceDamping;
            pos.z = Mathf.Sign(pos.z) * halfSize.z;
        }

        rb.linearVelocity = vel;
    }

    void Clamp(ref Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, -halfSize.x, halfSize.x);
        pos.y = Mathf.Clamp(pos.y, -halfSize.y, halfSize.y);
        pos.z = Mathf.Clamp(pos.z, -halfSize.z, halfSize.z);

        if (rb) rb.linearVelocity = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
