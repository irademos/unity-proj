using UnityEngine;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    [Header("Profiles")]
    public SkyProfile clearProfile;
    public SkyProfile cloudyProfile;
    public SkyProfile rainProfile;
    public SkyProfile stormProfile;

    [Header("Particles")]
    public ParticleSystem rainFX;
    public ParticleSystem stormFX;

    [Header("Lightning")]
    public Light lightningFlash;
    public float lightningChancePerMinute = 4f;

    private WeatherType current;
    private WeatherType target;

    void Start()
    {
        SetWeather(WeatherType.Clear, immediate: true);
    }

    void Update()
    {
        if (current == WeatherType.Storm)
            MaybeLightning();
    }

    // --- PUBLIC API ---
    public void SetWeather(WeatherType type, bool immediate = false)
    {
        target = type;

        if (immediate)
        {
            ApplyProfile(GetProfile(type));
            current = target;
            return;
        }

        StopAllCoroutines();
        StartCoroutine(BlendToProfile(GetProfile(type), 2f));
        current = type;
    }

    // --- PROFILE HANDLING ---
    private SkyProfile GetProfile(WeatherType t)
    {
        return t switch
        {
            WeatherType.Clear  => clearProfile,
            WeatherType.Cloudy => cloudyProfile,
            WeatherType.Rain   => rainProfile,
            WeatherType.Storm  => stormProfile,
            _ => clearProfile
        };
    }

    private void ApplyProfile(SkyProfile p)
    {
        RenderSettings.skybox = p.skybox;
        RenderSettings.fogDensity = p.fogDensity;
        RenderSettings.ambientLight = p.ambientLight;
        DynamicGI.UpdateEnvironment();

        ActivateParticles(current);
    }

    private IEnumerator BlendToProfile(SkyProfile p, float duration)
    {
        Material startSky = RenderSettings.skybox;
        Material endSky = p.skybox;

        float startFog = RenderSettings.fogDensity;
        Color startAmbient = RenderSettings.ambientLight;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            RenderSettings.fogDensity = Mathf.Lerp(startFog, p.fogDensity, t);
            RenderSettings.ambientLight = Color.Lerp(startAmbient, p.ambientLight, t);

            // Switch skybox halfway through
            if (t > 0.5f)
                RenderSettings.skybox = endSky;

            DynamicGI.UpdateEnvironment();
            yield return null;
        }

        ActivateParticles(target);
    }

    private void ActivateParticles(WeatherType type)
    {
        rainFX?.Stop();
        stormFX?.Stop();

        switch (type)
        {
            case WeatherType.Rain:
                rainFX?.Play();
                break;

            case WeatherType.Storm:
                stormFX?.Play();
                break;
        }
    }

    // --- LIGHTNING ---
    private void MaybeLightning()
    {
        float chance = (lightningChancePerMinute / 60f) * Time.deltaTime;
        if (Random.value < chance)
            StartCoroutine(FlashLightning());
    }

    private IEnumerator FlashLightning()
    {
        if (lightningFlash == null) yield break;

        lightningFlash.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.12f));
        lightningFlash.enabled = false;
    }
}
