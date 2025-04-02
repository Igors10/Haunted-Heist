using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] float loading_animation_interval;
    [SerializeField] TextMeshProUGUI loading_text;
    void Start()
    {
        StartCoroutine(LoadingAnim(loading_animation_interval));
    }

    IEnumerator LoadingAnim(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            loading_text.text = "Loading.";

            yield return new WaitForSeconds(interval);

            loading_text.text = "Loading..";

            yield return new WaitForSeconds(interval);

            loading_text.text = "Loading...";
        }
    }
    
}
