using UnityEngine;
using TMPro;
using System.Collections;

public class StartText : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] GameObject title_screen;
    public float fade_duration = 1f;
    public float wait_between_fades = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        if (text != null)
        {
            StartCoroutine(FadeLoop());
        }
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FadeText(0f, 1f)); // Fade in
            yield return new WaitForSeconds(wait_between_fades);
            yield return StartCoroutine(FadeText(1f, 0f)); // Fade out
            yield return new WaitForSeconds(wait_between_fades);
        }
    }

    IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = text.color;

        while (elapsed < fade_duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fade_duration);
            text.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        text.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    void StartInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            title_screen.SetActive(false);
        }
    }

    private void Update()
    {
        StartInput();
    }

}
