using UnityEngine;

public class ManageOverlay : MonoBehaviour
{
    float previousNumber = 0;
    [SerializeField] AnimationScript smoke;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Activate(TutorialProgress.part);

        
    }

    void Activate(int number)
    {
        if (number != previousNumber)
        {
            StartCoroutine(smoke.PlayAnimation());
            Transform selectedOverlay = gameObject.transform.GetChild(number - 1);
            selectedOverlay.gameObject.SetActive(true);
            previousNumber = number;
        }
    }
}
