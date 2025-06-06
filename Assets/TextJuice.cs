using System.Collections;
using TMPro;
using UnityEngine;

public class TextJuice : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public int maxValue = 6;
    private int currentValue = 6;

    private Vector3 originalScale;
    private Coroutine animationCoroutine;

    private void Start()
    {
        originalScale = counterText.rectTransform.localScale;
        UpdateCounterTextValue();
    }

    private void Update()
    {
        // To enable keyboard input for testing: Only for debugging
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
        }
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
}
