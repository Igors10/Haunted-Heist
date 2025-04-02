using System.Collections;
using TMPro;
using UnityEngine;

public class NewUI : MonoBehaviour
{
    [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject[] heartObjects;
    [SerializeField] private GameObject[] emptyHearts;

    [SerializeField] private float gameTimerInSeconds = 300;
    public TMP_Text timerInMinutesAndSeconds;

    // -----------Health Methods-----------

    public void UpdateHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHeartDisplay();
    }

    private void UpdateHeartDisplay()
    {
        for (int i = 0; i < heartObjects.Length; i++)
        {
            // currentHealth here is a variable I just made up, this needs to be replaced with the actual player health.
            heartObjects[i].SetActive(i < currentHealth);
        }
    }

    private void Start()
    {
        UpdateHeartDisplay();
    }

    private void Update()
    {
        // This will start to countdown as soon as the scene is loaded, so trigger this after both players are loaded in.
        CountDownTimer();
        // For debugging purposes only - Can remove once the player health is properly implemented
        ControlHealth();
    }

    // -----------Debugging Methods-----------

    public void DecreaseHealth()
    {
        UpdateHealth(currentHealth - 1);
        BlinkTheHearts(); // Blink the hearts when health is decreased, so you can reuse this method in update maybe, when player health is decreased.
    }

    public void IncreaseHealth()
    {
        UpdateHealth(currentHealth + 1);
    }

    public void ControlHealth()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseHealth();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseHealth();
        }
    }

    // -----------Timer Methods-----------

    public void CountDownTimer()
    {
        if (gameTimerInSeconds > 0)
        {
            gameTimerInSeconds -= Time.deltaTime;
        }

        int minutes = Mathf.FloorToInt(gameTimerInSeconds / 60);
        int seconds = Mathf.FloorToInt(gameTimerInSeconds % 60);

        timerInMinutesAndSeconds.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (gameTimerInSeconds <= 0)
        {
            gameTimerInSeconds = 0;
            // GAME IS OVER / Ghost wins
        }
    }

    // -----------Blinking Hearts-----------

    public void BlinkTheHearts()
    {
        StartCoroutine(BlinkingHearts());
    }

    private IEnumerator BlinkingHearts()
    {
        // Store original active states of hearts
        bool[] originalFilledStates = new bool[heartObjects.Length];

        for (int i = 0; i < heartObjects.Length; i++)
        {
            originalFilledStates[i] = heartObjects[i].activeSelf;
        }

        for (int j = 0; j < 5; j++) // Number of times the hearts blink
        {
            // Toggle hearts OFF
            for (int i = 0; i < heartObjects.Length; i++)
            {
                heartObjects[i].SetActive(false);
                emptyHearts[i].SetActive(false);
            }

            yield return new WaitForSeconds(0.2f);

            // Toggle hearts ON
            for (int i = 0; i < heartObjects.Length; i++)
            {
                heartObjects[i].SetActive(originalFilledStates[i]);
                emptyHearts[i].SetActive(!originalFilledStates[i]);
            }

            yield return new WaitForSeconds(0.2f);
        }
        UpdateHeartDisplay();
    }

}
