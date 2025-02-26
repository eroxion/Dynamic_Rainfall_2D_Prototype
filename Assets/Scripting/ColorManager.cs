using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorManager : MonoBehaviour {
    private Camera mainCamera;
    private float thunderTimer;

    [SerializeField] private Canvas canvas;
    private Image gradientOverlay;

    [SerializeField] private float gradientStartHeightFactor = 0.75f;
    [SerializeField] private float thunderInterval = 3f;
    [SerializeField] private float thunderDuration = 0.1f;

    private Color[] stormColors = {
        new Color(0.529f, 0.808f, 0.980f),
        new Color(0.117f, 0.564f, 1f),
        new Color(0.4f, 0.4f, 0.4f),
        new Color(0.2f, 0.0f, 0.3f),
        Color.black
    };

    void Start() {
        mainCamera = Camera.main;
        thunderTimer = Random.Range(1f, thunderInterval);

        GameObject gradientObject = new GameObject("GradientOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gradientObject.transform.SetParent(canvas.transform, false);

        RectTransform rt = gradientObject.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        gradientOverlay = gradientObject.GetComponent<Image>();
        gradientOverlay.material = new Material(Shader.Find("UI/Default"));
        gradientOverlay.color = new Color(0, 0, 0, 0);
    }

    void Update() {
        float mouseY = Input.mousePosition.y / Screen.height;
        float intensity = 1 - mouseY;

        int index = Mathf.Clamp(Mathf.FloorToInt(intensity * (stormColors.Length - 1)), 0, stormColors.Length - 2);
        float blend = (intensity * (stormColors.Length - 1)) - index;
        Color targetColor = Color.Lerp(stormColors[index], stormColors[index + 1], blend);

        mainCamera.backgroundColor = targetColor;

        float gradientStart = Screen.height * gradientStartHeightFactor;
        float alpha = Mathf.Lerp(0.8f, 0f, intensity);
        gradientOverlay.color = new Color(0, 0, 0, alpha);

        if (intensity >= 0.99f) {
            thunderTimer -= Time.deltaTime;
            if (thunderTimer <= 0) {
                StartCoroutine(ThunderFlash());
                thunderTimer = Random.Range(1f, thunderInterval);
            }
        }
    }

    IEnumerator ThunderFlash() {
        mainCamera.backgroundColor = Color.white;
        yield return new WaitForSeconds(thunderDuration);
        mainCamera.backgroundColor = stormColors[stormColors.Length - 1];
    }
}
