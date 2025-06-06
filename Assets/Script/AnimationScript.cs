using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    bool playing;
    [SerializeField] float frame_intervals;
    Image image;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    public IEnumerator PlayAnimation()
    {
        playing = true;
        image.enabled = true;

        for (int a = 0; a < frames.Length; a++)
        {
            image.sprite = frames[a];
            yield return new WaitForSeconds(frame_intervals);
        }

        playing = false;
        image.enabled = false;
    }

    
}
