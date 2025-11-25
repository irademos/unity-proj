using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayLengthMinutes = 5f;
    [Range(0, 1)] public float timeOfDay = 0.25f; // 0 = midnight, .25 = sunrise, .5 = noon

    [Header("Lights")]
    public Light sun;
    public Light moon;

    [Header("Intensity Curves")]
    public AnimationCurve sunIntensity;
    public AnimationCurve moonIntensity;
    [Header("Skyboxes (Optional)")]
    public Material daySkybox;
    public Material nightSkybox;
    public float nightStart = 0.55f;   // sunset
    public float dayStart = 0.45f;     // sunrise


    float DaySpeed => 1f / (dayLengthMinutes * 60f);

    void Update()
    {
        // Time progression
        timeOfDay += DaySpeed * Time.deltaTime;
        if (timeOfDay > 1f) timeOfDay -= 1f;

        // Rotate the sun & moon
        float sunAngle = timeOfDay * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 90f, 0f);
        moon.transform.rotation = Quaternion.Euler(sunAngle + 180f, 90f, 0f);

        // Switch skybox based on time
        if (timeOfDay > nightStart || timeOfDay < dayStart)
        {
            RenderSettings.skybox = nightSkybox;
        }
        else
        {
            RenderSettings.skybox = daySkybox;
        }

        DynamicGI.UpdateEnvironment();  // refresh reflections


        // Intensity from curves
        sun.intensity = sunIntensity.Evaluate(timeOfDay);
        moon.intensity = moonIntensity.Evaluate(timeOfDay);
    }
}
