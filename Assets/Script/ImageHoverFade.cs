using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageHoverFade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Base Settings")]
    public Image targetImage;
    public float fadeDuration = 0.5f;
    public float startAlpha = 0.1f;
    public float endAlpha = 1f;

    [Header("Scale Animation")]
    public bool enableScale = true;
    public float hoverScale = 1.1f;
    public float selectedScale = 1.15f;
    public float scaleDuration = 0.3f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Rotating Ring")]
    public bool enableRotatingRing = true;
    public Color ringColor = new Color(1f, 1f, 1f, 0.6f);
    public float ringRotationSpeed = 45f;
    public float ringSize = 1.4f;
    public float ringThickness = 8f;

    [Header("Pulse Waves")]
    public bool enablePulseWaves = true;
    public Color waveColor = new Color(1f, 1f, 1f, 0.3f);
    public float waveSpeed = 2f;
    public int waveCount = 3;
    public float maxWaveSize = 2f;

    private Coroutine currentFade;
    private Coroutine currentScale;
    private bool isHovered = false;
    private bool isSelected = false;
    private Vector3 originalScale;

    private Image ringImage;
    private Image[] waveImages;
    private Transform effectsContainer;
    private Coroutine ringCoroutine;
    private Coroutine waveCoroutine;

    private void Start()
    {
        SetAlpha(startAlpha);
        originalScale = transform.localScale;
        CreateEffects();
    }

    private void CreateEffects()
    {
        GameObject containerObj = new GameObject("Effects Container");
        containerObj.transform.SetParent(transform);
        containerObj.transform.localPosition = Vector3.zero;
        containerObj.transform.localScale = Vector3.one;

        containerObj.transform.SetSiblingIndex(0);
        effectsContainer = containerObj.transform;

        if (enableRotatingRing)
            CreateRotatingRing();

        if (enablePulseWaves)
            CreatePulseWaves();

        effectsContainer.gameObject.SetActive(false);
    }

    private void CreateRotatingRing()
    {
        GameObject ringObj = new GameObject("Rotating Ring");
        ringObj.transform.SetParent(effectsContainer);
        ringObj.transform.localPosition = Vector3.zero;
        ringObj.transform.localScale = Vector3.one * ringSize;

        ringImage = ringObj.AddComponent<Image>();

        Texture2D ringTexture = CreateRingTexture(128, ringThickness);
        ringImage.sprite = Sprite.Create(ringTexture, new Rect(0, 0, 128, 128), Vector2.one * 0.5f);
        ringImage.color = new Color(ringColor.r, ringColor.g, ringColor.b, 0f);
        ringImage.raycastTarget = false;
    }

    private void CreatePulseWaves()
    {
        waveImages = new Image[waveCount];

        for (int i = 0; i < waveCount; i++)
        {
            GameObject waveObj = new GameObject($"Pulse Wave {i}");
            waveObj.transform.SetParent(effectsContainer);
            waveObj.transform.localPosition = Vector3.zero;

            Image wave = waveObj.AddComponent<Image>();

            Texture2D waveTexture = CreateCircleOutlineTexture(64, 4f);
            wave.sprite = Sprite.Create(waveTexture, new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
            wave.color = new Color(waveColor.r, waveColor.g, waveColor.b, 0f);
            wave.raycastTarget = false;

            waveImages[i] = wave;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StartFade(startAlpha, endAlpha);

        if (enableScale)
            StartScale(hoverScale);

        if (effectsContainer != null)
            effectsContainer.gameObject.SetActive(true);

        StartVisualEffects();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!isSelected)
        {
            StartFade(endAlpha, startAlpha);

            if (enableScale)
                StartScale(1f);

            StopVisualEffects();
            if (effectsContainer != null)
                effectsContainer.gameObject.SetActive(false);
        }
    }

    private void StartVisualEffects()
    {
        if (enableRotatingRing && ringImage != null)
        {
            if (ringCoroutine != null) StopCoroutine(ringCoroutine);
            ringCoroutine = StartCoroutine(RotateRing());
        }

        if (enablePulseWaves && waveImages != null)
        {
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            waveCoroutine = StartCoroutine(AnimatePulseWaves());
        }
    }

    private void StopVisualEffects()
    {
        if (ringCoroutine != null) { StopCoroutine(ringCoroutine); ringCoroutine = null; }
        if (waveCoroutine != null) { StopCoroutine(waveCoroutine); waveCoroutine = null; }

        StartCoroutine(FadeOutEffects());
    }

    private IEnumerator FadeOutEffects()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            if (ringImage != null) SetEffectAlpha(ringImage, alpha * ringColor.a);

            if (waveImages != null)
                foreach (var wave in waveImages)
                    if (wave != null) SetEffectAlpha(wave, alpha * waveColor.a);

            elapsed += Time.deltaTime;
            yield return null;
        }
        if (ringImage != null) SetEffectAlpha(ringImage, 0f);
        if (waveImages != null)
            foreach (var wave in waveImages)
                if (wave != null) SetEffectAlpha(wave, 0f);
    }

    private IEnumerator RotateRing()
    {
        SetEffectAlpha(ringImage, ringColor.a);
        while (true)
        {
            ringImage.transform.Rotate(0, 0, ringRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator AnimatePulseWaves()
    {
        while (true)
        {
            for (int i = 0; i < waveImages.Length; i++)
            {
                if (waveImages[i] == null) continue;

                float delay = i * (1f / waveCount);
                float time = (Time.time * waveSpeed + delay) % 1f;

                float scale = Mathf.Lerp(0.5f, maxWaveSize, time);
                float alpha = Mathf.Lerp(waveColor.a, 0f, time);

                waveImages[i].transform.localScale = Vector3.one * scale;
                SetEffectAlpha(waveImages[i], alpha);
            }

            yield return null;
        }
    }

    private void StartScale(float targetScale)
    {
        if (currentScale != null) StopCoroutine(currentScale);
        currentScale = StartCoroutine(ScaleImage(targetScale));
    }

    private IEnumerator ScaleImage(float targetScale)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = originalScale * targetScale;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            float progress = elapsed / scaleDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, scaleCurve.Evaluate(progress));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }

    private void SetAlpha(float alpha)
    {
        if (targetImage == null) return;
        Color color = targetImage.color;
        color.a = alpha;
        targetImage.color = color;
    }

    private void SetEffectAlpha(Image image, float alpha)
    {
        if (image == null) return;
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void StartFade(float from, float to)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeImage(from, to));
    }

    private IEnumerator FadeImage(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            SetAlpha(newAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetAlpha(to);
    }

    private void Update()
    {
        bool currentlySelected = EventSystem.current.currentSelectedGameObject == gameObject;

        if (currentlySelected && !isSelected)
        {
            isSelected = true;
            StartFade(startAlpha, endAlpha);

            if (enableScale)
                StartScale(selectedScale);

            if (effectsContainer != null)
                effectsContainer.gameObject.SetActive(true);

            StartVisualEffects();
        }
        else if (!currentlySelected && isSelected && !isHovered)
        {
            isSelected = false;
            StartFade(endAlpha, startAlpha);

            if (enableScale)
                StartScale(1f);

            StopVisualEffects();
            if (effectsContainer != null)
                effectsContainer.gameObject.SetActive(false);
        }
    }

    private Texture2D CreateRingTexture(int size, float thickness)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        Vector2 center = Vector2.one * (size * 0.5f);
        float outerRadius = size * 0.4f;
        float innerRadius = outerRadius - thickness;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);

                if (distance >= innerRadius && distance <= outerRadius)
                {
                    float alpha = 1f - Mathf.Abs(distance - (innerRadius + outerRadius) * 0.5f) / (thickness * 0.5f);
                    alpha = Mathf.Clamp01(alpha);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private Texture2D CreateCircleOutlineTexture(int size, float thickness)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        Vector2 center = Vector2.one * (size * 0.5f);
        float radius = size * 0.4f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);

                float alpha = 1f - Mathf.Abs(distance - radius) / thickness;
                alpha = Mathf.Clamp01(alpha);

                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
