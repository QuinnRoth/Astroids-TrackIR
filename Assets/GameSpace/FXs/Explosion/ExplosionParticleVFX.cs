using UnityEngine;

public class ExplosionParticleVFX : MonoBehaviour
{
    // Legacy particle system slots used by the older prefab.
    [SerializeField] private ParticleSystem flashVFX;
    [SerializeField] private ParticleSystem sparksVFX;
    [SerializeField] private ParticleSystem fireVFX;
    [SerializeField] private ParticleSystem smokeVFX;

    [SerializeField] private float destroyPadding = 0.2f;
    [SerializeField] private bool playOnStart = true;

    private ParticleSystem[] particleSystems;
    private bool hasPlayed;

    private void Awake()
    {
        CacheParticleSystems();
    }

    private void Start()
    {
        if (playOnStart)
            PlayVFX();
    }

    public void PlayVFX()
    {
        if (hasPlayed)
            return;

        hasPlayed = true;
        CacheParticleSystems();

        float life = 0f;
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (particleSystem == null)
                continue;

            particleSystem.Play(true);
            life = Mathf.Max(life, GetTotalLifetime(particleSystem));
        }

        Destroy(gameObject, life + destroyPadding);
    }

    private void CacheParticleSystems()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    private float GetTotalLifetime(ParticleSystem particleSystem)
    {
        ParticleSystem.MainModule main = particleSystem.main;
        return GetMaxCurveValue(main.startDelay) + main.duration + GetMaxCurveValue(main.startLifetime);
    }

    private float GetMaxCurveValue(ParticleSystem.MinMaxCurve curve)
    {
        switch (curve.mode)
        {
            case ParticleSystemCurveMode.Constant:
                return curve.constant;
            case ParticleSystemCurveMode.TwoConstants:
                return curve.constantMax;
            case ParticleSystemCurveMode.Curve:
                return GetMaxAnimationCurveValue(curve.curve, curve.curveMultiplier);
            case ParticleSystemCurveMode.TwoCurves:
                return Mathf.Max(
                    GetMaxAnimationCurveValue(curve.curveMin, curve.curveMultiplier),
                    GetMaxAnimationCurveValue(curve.curveMax, curve.curveMultiplier));
            default:
                return 0f;
        }
    }

    private float GetMaxAnimationCurveValue(AnimationCurve curve, float multiplier)
    {
        if (curve == null || curve.length == 0)
            return 0f;

        float maxValue = 0f;
        foreach (Keyframe key in curve.keys)
            maxValue = Mathf.Max(maxValue, key.value * multiplier);

        return maxValue;
    }
}
