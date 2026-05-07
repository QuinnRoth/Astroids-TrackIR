using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SpaceshipDamage : MonoBehaviour
{
    // Reworked health system for properites
    private int _playerHealth = 3;
    public int playerHealth
    {
        get { return _playerHealth; }
        set { _playerHealth = Mathf.Clamp(value, 0, 3); }
    }

    //private SpaceshipDeathAnimation deathAnimation;
    public GameObject gameOverMenu;
    public GameObject cursorCanvas;

    public DamageFlashEffect damageFlash;
    // private SpaceshipMovement spaceshipMovement; 
    // private SpaceshipShoot spaceshipShoot;

    // Cooldown timer method
    [Header("Cooldown Timer Method")]
    public float damageCooldown = 1f;
    // private float lastDamageTime = 0f;
    // private float startTime;

    // Coroutine method
    [Header("Coroutine Method")]
    public float damageBlinkDuration = 0.8f;
    public float damageBlinkTime = 0.1f;
    public float screenFadingDuration = 1.0f;
    public float screenIsBlackDuration = 1.0f;
    [SerializeField] private Renderer shipModelRenderer;
    public Image fadeScreenImage;
    private bool canTakeDamage = true;

    // private void Start()
    // {
    //     startTime = Time.time;
    // }

    public void HandleTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Asteroid"))
        {
            //** On damage cooldown method **//
            // // Only take damage if enough time has passed
            // if (Time.time - startTime - lastDamageTime >= damageCooldown)
            // {
            //     TakeDamage();
            //     lastDamageTime = Time.time;  // reset cooldown
            // }
            // else
            // {
            //     Debug.Log("Hit asteroid, but cooldown active — no damage taken.");
            // }

            //** On damage ship reoritentation method **//
            if (canTakeDamage)
            {
                TakeDamage();

                // Blink the spaceship model to show that the spaceship has been damaged
                // During this time, the spaceship cannot be damaged
                StartCoroutine(TakeDamageReorientation());
            }
        }

        if (playerHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private void TakeDamage()
    {
        // damageFlash.Flash();
        SoundManager.PlaySound(SoundType.DAMAGE, 0.5f);
        playerHealth -= 1;

        Debug.Log("Spaceship collided with asteroid! -1 Health");
    }

    public void GainHealth()
    {
        playerHealth += 1;
    }

    IEnumerator Die()
    {
        SoundManager.PlaySound(SoundType.DEATH, 0.75f);
        GetComponentInParent<SpaceshipDeathAnimation>().TriggerDeath();

        var camera = transform.Find("CameraHolder");
        if (camera)
            camera.transform.SetParent(null);
        else if (Camera.main)
            Camera.main.transform.SetParent(null);

        Destroy(gameObject, 3f);

        yield return new WaitForSeconds(2f);

        gameOverMenu.SetActive(true);
        cursorCanvas.SetActive(true);
    }

    private IEnumerator TakeDamageReorientation()
    {
        canTakeDamage = false;

        yield return StartCoroutine(BlinkShipModel());

        /**== Not In Use ==**/ //resets spaceship to center
        // yield return StartCoroutine(FadeToBlack());

        // // Move, stop, and reorient spaceship
        // transform.position = Vector3.zero;
        // transform.rotation = Quaternion.identity;
        // Rigidbody rb = GetComponent<Rigidbody>();
        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;

        // StartCoroutine(FadeFromBlack());

        canTakeDamage = true;
    }

    private IEnumerator BlinkShipModel()
    {
        for (float i = 0.0f; i < 1.0f; i += damageBlinkTime * (1/damageBlinkDuration))
        {
            shipModelRenderer.enabled = !shipModelRenderer.enabled;
            // Debug.Log(shipModelRenderer.enabled);
            yield return new WaitForSeconds(damageBlinkTime);
        }
    }

    /**== Not In Use ==**/
    // private IEnumerator FadeToBlack()
    // {
    //     for (float i = 0.0f; i < screenFadingDuration; i += Time.deltaTime)
    //     {
    //         Color tempColor = fadeScreenImage.color;
    //         tempColor.a = i / screenFadingDuration;
    //         fadeScreenImage.color = tempColor;

    //         yield return new WaitForSeconds(Time.deltaTime);
    //     }
    // }

    // private IEnumerator FadeFromBlack()
    // {
    //     for (float i = 0.0f; i < screenFadingDuration; i += Time.deltaTime)
    //     {
    //         Color tempColor = fadeScreenImage.color;
    //         tempColor.a = screenFadingDuration - (i / screenFadingDuration);
    //         fadeScreenImage.color = tempColor;

    //         yield return new WaitForSeconds(Time.deltaTime);
    //     }
    // }
}


