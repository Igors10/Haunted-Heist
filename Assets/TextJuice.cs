using System.Collections;
using TMPro;
using UnityEngine;

public class TextJuice : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public int maxValue = 6;
    private int currentValue = 7;

    private Vector3 originalScale;
    private Coroutine animationCoroutine;

    //Item stolen text
    [SerializeField] TextMeshProUGUI stolen_text;
    [SerializeField] float fade_duration;
    [SerializeField] float wait_between_fades;

    private void Start()
    {
        originalScale = counterText.rectTransform.localScale;
        UpdateCounterTextValue();
    }

    private void Update()
    {
        // To enable keyboard input for testing: Only for debugging
        /*
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentValue != 0)
            {
                currentValue--;
                UpdateCounterTextWithJuice();
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentValue != maxValue)
            {
                currentValue++;
                UpdateCounterTextWithJuice();
            }
        }*/
    }

    private void UpdateCounterTextValue()
    {
        currentValue--; // Decrese the value by 1 
        counterText.text = $"{currentValue} / {maxValue}";
    }

    // This method should be called when the value changes, so when the robber picks up item you can call this one
    public void UpdateCounterTextWithJuice()
    {
        UpdateCounterTextValue();

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(PlayJuiceAnimation());
    }

    private IEnumerator PlayJuiceAnimation()
    {
        float duration = 0.25f;
        float elapsed = 0f;

        Color originalColor = Color.white;
        Color highlightColor = new Color(1f, 0f, 0f); // This is red colour

        // Make "stolen text" appear
        StartCoroutine(TextAnimation());

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            counterText.color = Color.Lerp(highlightColor, originalColor, t);

            float scale = Mathf.Lerp(1.5f, 1f, t);
            counterText.rectTransform.localScale = originalScale * scale;

            yield return null;
        }

        counterText.color = originalColor;
        counterText.rectTransform.localScale = originalScale;
    }
    IEnumerator TextAnimation()
    {
        stolen_text.gameObject.SetActive(true);
        yield return StartCoroutine(FadeText(0f, 1f)); // Fade in
        yield return new WaitForSeconds(wait_between_fades);
        yield return StartCoroutine(FadeText(1f, 0f)); // Fade out
        stolen_text.gameObject.SetActive(false);
    }
    IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = stolen_text.color;

        while (elapsed < fade_duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fade_duration);
            stolen_text.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        stolen_text.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
