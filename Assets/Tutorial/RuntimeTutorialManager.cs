using UnityEngine;
using TMPro;

public class RuntimeTutorialManager : MonoBehaviour
{
    public bool Robber;
    public bool Ghost;

    public GameObject overlay;

    string active_type; //switch to enum

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WipeTutorial();
    }

    void WipeTutorial()
    {



    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateOverlay(float x_pos, float y_pos, string text, string type)
    {
        overlay.GetComponent<RuntimeOverlayScript>().textbox.GetComponent<TextMeshPro>().text = text;
        overlay.GetComponent<RuntimeOverlayScript>().type = type;
        active_type = type;
        //put text in the chid of the overlay
        //plan the overlay in the correct place
        //instantiate overlay
        //bind the type for deleting it from the game
    }
}
