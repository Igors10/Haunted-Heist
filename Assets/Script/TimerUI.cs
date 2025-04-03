using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private float gameTimerInSeconds = 300;
    public TMP_Text timerInMinutesAndSeconds;

    
    private void Update()
    {
        // This will start to countdown as soon as the scene is loaded, so trigger this after both players are loaded in.
        CountDownTimer();
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
}
