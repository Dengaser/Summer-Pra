using UnityEngine;

using UnityEngine;

public class SceneAtmosphereInstaller : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    // Делаем цвет светлее и менее насыщенным, чтобы карта не темнела
    public Color fogColor = new Color(0.5f, 0.55f, 0.5f);
    public FogMode fogMode = FogMode.Linear; // Linear проще контролировать по дистанции

    [Tooltip("Дистанция, где туман НАЧИНАЕТСЯ (уведите дальше от персонажа)")]
    public float fogStartDistance = 20f;
    [Tooltip("Дистанция, где туман полностью скрывает обзор")]
    public float fogEndDistance = 100f;

    [Header("Skybox Settings")]
    public Material skyboxMaterial;

    void Start()
    {
        // Настройка глобального тумана (теперь как легкая дымка на горизонте)
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;

        if (fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.fogEndDistance = fogEndDistance;
        }
        else
        {
            // Если используете экспоненциальный, плотность должна быть крошечной
            RenderSettings.fogDensity = 0.01f;
        }

        // Чтобы карта не была слишком темной, можно подсветить ее через Ambient Light
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.3f); // Мягкая общая подсветка

        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
    }
}