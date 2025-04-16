using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverFadeImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;
    public float fadeDuration = 0.5f;
    public float startAlpha = 0.1f;
    public float endAlpha = 1f;

    private Coroutine currentFade;
    private bool isHovered = false;
    private bool isSelected = false;

    private void SetAlpha(float alpha)
    {
        if (targetImage == null) return;
        Color color = targetImage.color;
        color.a = alpha;
        targetImage.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StartFade(startAlpha, endAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!isSelected)
            StartFade(endAlpha, startAlpha);
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

    private void Start()
    {
        SetAlpha(startAlpha);
    }

    private void Update()
    {
        // Check if this button is currently selected
        bool currentlySelected = EventSystem.current.currentSelectedGameObject == gameObject;

        if (currentlySelected && !isSelected)
        {
            isSelected = true;
            StartFade(startAlpha, endAlpha);
        }
        else if (!currentlySelected && isSelected && !isHovered)
        {
            isSelected = false;
            StartFade(endAlpha, startAlpha);
        }
    }
}
