using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipShoot : MonoBehaviour
{
    [SerializeField] private Transform spaceshipModel;
    public GameObject laserObject;
    public CameraShake cameraShake;
    private InputAction shootAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
        shootAction.performed += _ => ShootLaser();
        shootAction.Enable();
    }

    public void OnDestroy()
    {
        shootAction.Disable();
    }

    // Hitscan Method
    void ForceRaycast()
    {
        // Declare the ray
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 50);

        // Declare the container for hit data
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData))
        {
            AsteroidClass asteroid = hitData.collider.GetComponentInParent<AsteroidClass>();
            if (asteroid != null)
            {
                asteroid.hitByLaser = true;
                asteroid.Die(false);
            }
        }
    }

    // Projectile Method
    void ShootLaser()
    {
        // Use the ship's rotation so lasers face the same direction as before
        Vector3 forwardByQuat = transform.rotation * transform.forward;
        Quaternion spawnRotation = Quaternion.Euler(forwardByQuat);

        /* For the visual lasers */
        // Left/right local offsets (adjust X to move left/right, Y/Z if needed)
        Vector3 leftLocal  = new Vector3(-0.4f, 0f, 0f);
        Vector3 rightLocal = new Vector3( 0.4f, 0f, 0f);

        // Convert local offsets to world space so they follow the ship's rotation/position
        Vector3 leftWorldPos  = transform.TransformPoint(leftLocal);
        Vector3 rightWorldPos = transform.TransformPoint(rightLocal);

        // Instantiate left laser
        GameObject laserLeft = Instantiate(laserObject, leftWorldPos, spawnRotation);
        laserLeft.GetComponent<Laser>().forwardDirection = transform.forward;

        // Instantiate right laser
        GameObject laserRight = Instantiate(laserObject, rightWorldPos, spawnRotation);
        laserRight.GetComponent<Laser>().forwardDirection = transform.forward;

        /* Put a third laser in the middle to hopefully help with collision detection */
        // Vector3 middleLocal = new Vector3(0.0f, 0.0f, 0.0f);
        // Vector3 middleWorldPos = transform.TransformPoint(middleLocal);
        // GameObject laserMiddle = Instantiate(laserObject, middleWorldPos, spawnRotation);
        // laserMiddle.GetComponent<Laser>().forwardDirection = transform.forward;
        // laserMiddle.GetComponent<MeshRenderer>().enabled = false;

        // Shake da camera oh yeah shake shake shake
        StartCoroutine(cameraShake.Shake(.1f, .01f));

        // Make a sound when shooting a laser
        SoundManager.PlaySound(SoundType.LASER);
    }
}
