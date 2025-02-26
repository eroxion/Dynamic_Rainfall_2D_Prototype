using UnityEngine;

public class RainController : MonoBehaviour {
    [SerializeField] private ParticleSystem rain;
    private Camera mainCamera;
    private ParticleSystem.TrailModule trails;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    private float previousMouseX; // Store last frame's mouse X position
    private float windStrength;   // Wind force based on cursor movement
    private float windVelocity;   // Wind acceleration factor

    public float minEmissionRate = 0f;   // No rain
    public float maxEmissionRate = 1000f; // Heavy rain
    public float minSpeed = 2f;
    public float maxSpeed = 20f;
    public float minTrailLifetime = 0.05f; // Small trail for realism
    public float maxTrailLifetime = 0.15f; // Slightly longer in heavy rain
    public float windEffectMultiplier = 5f; // Controls wind acceleration
    public float windDecayRate = 3f; // How fast wind slows down

    void Start() {
        mainCamera = Camera.main;

        if (rain == null) {
            Debug.LogError("Rain Particle System not found!");
            return;
        }

        if (!rain.isPlaying) {
            rain.Play(); // Start rain
        }

        // Get required modules
        trails = rain.trails;
        velocityModule = rain.velocityOverLifetime;
        velocityModule.enabled = true; // Enable velocity control
    }

    void Update() {
        if (rain == null) return;

        // Get mouse Y position and normalize (0 = top, 1 = bottom)
        float mouseY = Input.mousePosition.y / Screen.height;
        float intensity = 1 - mouseY; // Inverted so bottom = heavy rain

        // Adjust emission rate (number of raindrops)
        var emission = rain.emission;
        emission.rateOverTime = Mathf.Lerp(minEmissionRate, maxEmissionRate, intensity);

        // Adjust raindrop fall speed
        var main = rain.main;
        main.gravityModifier = Mathf.Lerp(minSpeed, maxSpeed, intensity);

        // Adjust trail effect based on rain intensity
        trails.lifetime = Mathf.Lerp(minTrailLifetime, maxTrailLifetime, intensity);

        // Calculate wind effect based on mouse speed
        float mouseX = Input.mousePosition.x;
        float mouseDeltaX = (mouseX - previousMouseX) / Time.deltaTime; // Get cursor speed
        previousMouseX = mouseX;

        // Accelerate wind based on cursor speed
        windVelocity += mouseDeltaX * windEffectMultiplier * Time.deltaTime;
        windVelocity = Mathf.Clamp(windVelocity, -50f, 50f); // Prevent extreme values

        // Gradually slow wind down when cursor stops
        windVelocity = Mathf.Lerp(windVelocity, 0, windDecayRate * Time.deltaTime);

        // Apply wind effect to the velocity over lifetime module
        velocityModule.x = new ParticleSystem.MinMaxCurve(windVelocity);

        // Debugging (Remove if not needed)
        Debug.Log("Wind Velocity: " + windVelocity);
    }
}
