using UnityEngine;

public class SceneAtmosphereInstaller : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color fogColor = new Color(0.2f, 0.23f, 0.2f); // грязный серо-зеленый
    public FogMode fogMode = FogMode.ExponentialSquared;
    public float fogDensity = 0.05f;

    [Header("Skybox Settings")]
    public Material skyboxMaterial;

    void Start()
    {
        // Автоматически настраиваем туман для сцены при старте игры
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogDensity = fogDensity;

        // Автоматически подменяем скайбокс
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
    }
}