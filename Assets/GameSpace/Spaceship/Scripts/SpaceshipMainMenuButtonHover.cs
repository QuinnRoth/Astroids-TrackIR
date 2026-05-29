using UnityEngine;
using System.Collections;

public class SpaceshipMainMenuButtonHover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spaceship;

    [Header("Button Hover")]
    [SerializeField] private float animationTime = 0.25f;

    // Y Values of the spaceship position for each button
    [Header("Main Menu")]
    [SerializeField] private float playButtonPos = 0.85f;
    [SerializeField] private float settingsButtonPos = -3.7f;
    [SerializeField] private float leaderboardButtonPos = -8.8f;
    [SerializeField] private float colorsButtonPos = -13.0f;
    [SerializeField] private float exitButtonPos = -18.2f;

    [Header("Gamemode Menu")]
    [SerializeField] private float tradeShowButtonPos = 0.9f;
    [SerializeField] private float endlessButtonPos = -4.8f;
    [SerializeField] private float wavesButtonPos = -10.5f;
    [SerializeField] private float backButtonPos = -16.1f;

    [Header("Floating Animation")]
    [SerializeField] private float floatingAmplitude = 0.5f;
    [SerializeField] private float floatingSpeed = 1f;
    private Vector3 spaceshipStartPos;

    private Coroutine currentCoroutine;
    private bool coroutineRunning;


    private void Start()
    {
        spaceshipStartPos = transform.position;
    }

    private void Update()
    {
        if (coroutineRunning)
            return;

        float newY = spaceshipStartPos.y + Mathf.Sin(Time.time * floatingSpeed) * floatingAmplitude;
        transform.position = new Vector3(spaceshipStartPos.x, newY, spaceshipStartPos.z);
    }

    public void StartLerping(Vector3 targetPosition, float duration)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
         
        currentCoroutine = StartCoroutine(LerpToPosition(targetPosition, duration));
    }

    private IEnumerator LerpToPosition(Vector3 target, float duration)
    {
        float timeElapsed = 0.0f;
        Vector3 startPosition = spaceship.transform.position;

        coroutineRunning = true;

        while (timeElapsed < duration)
        {
            // Calculate progress from 0 to 1
            float t = timeElapsed / duration;
            spaceship.transform.position = Vector3.Lerp(startPosition, target, t);

            timeElapsed += Time.deltaTime;
            yield return null; // wait until next frame
        }

        spaceship.transform.position = target; // ensure it reaches the final position
        coroutineRunning = false;
    }

    public void HoverPlayButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != playButtonPos)
        {
            newPos.y = playButtonPos;
            StartLerping(newPos, animationTime);
        }
        
    }

    public void HoverSettingsButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != settingsButtonPos)
        {
            newPos.y = settingsButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    public void HoverLeaderboardButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != leaderboardButtonPos)
        {
            newPos.y = leaderboardButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    public void HoverColorsButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != colorsButtonPos)
        {
            newPos.y = colorsButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    public void HoverExitButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != exitButtonPos)
        {
            newPos.y = exitButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    public void HoverTradeShowButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != tradeShowButtonPos)
        {
            newPos.y = tradeShowButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    public void HoverEndlessButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != endlessButtonPos)
        {
            newPos.y = endlessButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    public void HoverWavesButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != wavesButtonPos)
        {
            newPos.y = wavesButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    public void HoverBackButton()
    {
        Vector3 newPos = spaceship.transform.position;
    
        if (newPos.y != backButtonPos)
        {
            newPos.y = backButtonPos;
            StartLerping(newPos, animationTime);
        }
    }

    // Functions to enable and disable spaceship model
    public void EnableSpaceshipModel() { spaceship.GetComponent<MeshRenderer>().enabled = true; }
    public void DisableSpaceshipModel() { spaceship.GetComponent<MeshRenderer>().enabled = false; }
}
