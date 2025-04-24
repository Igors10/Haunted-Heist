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
        TutorialProgress.robber_movement        = false;
        TutorialProgress.robber_lantern         = false;
        TutorialProgress.robber_pickup          = false;
        TutorialProgress.robber_light_warning   = false;
        TutorialProgress.robber_radar           = false;
        TutorialProgress.robber_vent            = false;
        TutorialProgress.robber_item_arrow      = false;
        TutorialProgress.robber_timer           = false;
        TutorialProgress.robber_escape          = false;

        TutorialProgress.ghost_movement         = false;
        TutorialProgress.ghost_dash             = false;
        TutorialProgress.ghost_stepvision       = false;
        TutorialProgress.ghost_dash_warning     = false;
        TutorialProgress.ghost_objective        = false;
        TutorialProgress.ghost_teleport         = false;
        TutorialProgress.ghost_timer            = false;
        TutorialProgress.ghost_items            = false;
        TutorialProgress.ghost_items_gathered   = false;
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
