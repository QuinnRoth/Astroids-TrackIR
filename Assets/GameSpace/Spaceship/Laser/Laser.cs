using UnityEngine;

public class Laser : MonoBehaviour
{
    public Vector3 forwardDirection;
    [SerializeField] private float speed;
    [SerializeField] private float raycastSize = 6.0f;
    private LayerMask layerMask;

    private float time = 0.0f;
    private float timeout = 1.0f;

    private Vector3 lastPosition;
    [SerializeField] private float wrapDetectionDistance = 100f;

    private Renderer rend;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Asteroids");
    }

    void Start()
    {
        lastPosition = transform.position;
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        ForceRaycast();

        transform.Translate(forwardDirection * speed * Time.deltaTime);

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved > wrapDetectionDistance)
        {
            HandleWrap();
        }

        lastPosition = transform.position;

        time += Time.deltaTime;
        if (time >= timeout)
            Destroy(gameObject);
    }

    void HandleWrap()
    {
        if (rend != null)
            rend.enabled = false;
    }

    // Hitscan Method
    void ForceRaycast()
    {
        RaycastHit hitData;

        float moveDistance = speed * Time.deltaTime;

        if (Physics.SphereCast(
                transform.position,
                raycastSize,
                forwardDirection,
                out hitData,
                moveDistance,
                layerMask))
        {
            AsteroidClass asteroid = hitData.collider.GetComponentInParent<AsteroidClass>();
            if (asteroid == null)
                return;

            var ghostBoundary = asteroid.GetComponent<GhostBoundary>();
            if (ghostBoundary != null)
            {
                Camera cam = Camera.main;
                Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                Vector3 explosionPosition = ghostBoundary.ClosestGhostToLaser(cam, screenCenter);
                asteroid.SetExplosionPosition(explosionPosition);
            }

            asteroid.hitByLaser = true;
            asteroid.Die(false);

            Destroy(gameObject);
        }
    }
}
