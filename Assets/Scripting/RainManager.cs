using UnityEngine;

public class RainManager : MonoBehaviour {
    [SerializeField] private ParticleSystem rain;
    private Camera mainCamera;
    private ParticleSystem.TrailModule trails;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    private float previousMouseX;
    private float windStrength;
    private float windVelocity;

    public float minEmissionRate = 0f;
    public float maxEmissionRate = 1000f;
    public float minSpeed = 2f;
    public float maxSpeed = 20f;
    public float minTrailLifetime = 0.05f;
    public float maxTrailLifetime = 0.15f;
    public float windEffectMultiplier = 5f;
    public float windDecayRate = 3f;

    void Start() {
        mainCamera = Camera.main;

        if (rain == null) {
            Debug.LogError("Rain Particle System not found!");
            return;
        }

        if (!rain.isPlaying) {
            rain.Play();
        }

        trails = rain.trails;
        velocityModule = rain.velocityOverLifetime;
        velocityModule.enabled = true;
    }

    void Update() {
        if (rain == null) return;

        float mouseY = Input.mousePosition.y / Screen.height;
        float intensity = 1 - mouseY;

        var emission = rain.emission;
        emission.rateOverTime = Mathf.Lerp(minEmissionRate, maxEmissionRate, intensity);

        var main = rain.main;
        main.gravityModifier = Mathf.Lerp(minSpeed, maxSpeed, intensity);

        trails.lifetime = Mathf.Lerp(minTrailLifetime, maxTrailLifetime, intensity);

        float mouseX = Input.mousePosition.x;
        float mouseDeltaX = (mouseX - previousMouseX) / Time.deltaTime;
        previousMouseX = mouseX;

        windVelocity += mouseDeltaX * windEffectMultiplier * Time.deltaTime;
        windVelocity = Mathf.Clamp(windVelocity, -50f, 50f);
        windVelocity = Mathf.Lerp(windVelocity, 0, windDecayRate * Time.deltaTime);

        velocityModule.x = new ParticleSystem.MinMaxCurve(windVelocity);
    }
}
