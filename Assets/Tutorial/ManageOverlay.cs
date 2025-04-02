using UnityEngine;
using UnityEngine.WSA;

public class ManageOverlay : MonoBehaviour
{
    float previousNumber = 0;
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
            Transform selectedOverlay = gameObject.transform.GetChild(number - 1);
            selectedOverlay.gameObject.SetActive(true);
            previousNumber = number;
        }
    }
}
