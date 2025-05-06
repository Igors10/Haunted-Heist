using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;

public class RuntimeTutorialManager : MonoBehaviour
{
    public GameObject overlay;

    //booleans to check the level of progression of the game and tailoring the tutorial messages
    bool goLeft = false;
    bool goRight = false;
    bool goUp = false;
    bool goDown = false;

    bool items_gathered = false;
    bool item_gathered = false;
    bool ghost_near = false;
    bool timer_near_end = false;
    bool timer_end = false;
    bool item_near = false;
    bool item_arrow_active = false;
    bool item_arrow_actived = false;
    bool robber_vented = false;
    bool robber_life_lost = false;
    bool robber_near = false;
    bool ghost_dashed = false;
    bool vent_near = false;
    bool light_active = false;
    bool steps_near = false;
    bool teleported = false;

    GameObject timer_reference;
    ItemLottery item_reference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WipeTutorial();
        timer_reference = GameObject.Find("Timer");
        item_reference = Game.Instance.item_lottery;
    }

    // Update is called once per frame
    void Update()
    {
        //chechking for the movement
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (!goUp || !goDown || !goLeft || !goRight)
        {
            if (x > 0)
            {
                goRight = true;
            }

            if (y > 0)
            {
                goUp = true;
            }

            if (y < 0)
            {
                goDown = true;
            }

            if (x < 0)
            {
                goLeft = true;
            }
        }
        

        //items check
        if(item_reference.all_items_collected && !items_gathered)
        {
            items_gathered = true;
        }
        
        //item_near;

        if(item_reference.item_picked && !item_gathered)
        {
            item_gathered = true;
        }
       

        //timer check
        if(timer_reference.GetComponent<TimerUI>().current_time < 60 && !timer_near_end)
        {
            timer_near_end = true;
        }

        if (timer_reference.GetComponent<TimerUI>().current_time <= 0 && !timer_end)
        {
            timer_end = true;
        }

        //ghost checkers
        
        //ghost_dashed;
        //ghost_near;
        //teleported;

        //robber checkers
        
        //item_arrow_active;
        //item_arrow_actived;
        //robber_vented;

        if (Game.Instance.robber.Value.GetComponent<RobberScript>().robberUI.hp.currentHealth < 3 && !robber_life_lost)
        {
            robber_life_lost = true;
        }
        
        //light_active
        //robber_near;

        //steps check
        //steps_near;
        //vent_near;
        

        //implementation of the functions
        //ROBBER#############################################################################################

        //robber movement
        if (!goRight || !goLeft || !goUp || !goDown && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0, "string text", "robber_movement", 9);
        }

        if (goRight && goLeft && goUp && goDown)
        {
            DeactivateBool("robber_movement");
        }
        //-----------------------------------------------------------------------------------

        //robber escape
        if (TutorialProgress.tutorial_bools["items_gathered"] && TutorialProgress.tutorial_bools["robber_pickup"] && TutorialProgress.tutorial_bools["robber_movement"] && TutorialProgress.tutorial_bools["robber_lantern"] && TutorialProgress.Robber)
        {
            ActivateOverlay(0,0, "string text", "robber_escape", 1);
        }
        //-----------------------------------------------------------------------------------

        //robber radar
        if (TutorialProgress.tutorial_bools["robber_movement"] && ghost_near && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0,"string text", "robber_radar", 2);
        }

        if (robber_life_lost)
        {
            DeactivateBool("robber_radar");
        }
        //-------------------------------------------------------------------------------------

        //robber time
        if (TutorialProgress.tutorial_bools["robber_movement"] && timer_near_end && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0, "string text", "robber_timer", 3);
        }

        if (timer_end)
        {
            DeactivateBool("robber_timer");
        }
        //-------------------------------------------------------------------------------------

        //robber vent
        if (TutorialProgress.tutorial_bools["robber_movement"] && vent_near && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0, "string text", "robber_vent", 4);
        }

        if (robber_vented)
        {
            DeactivateBool("robber_vent");
        }
        //-------------------------------------------------------------------------------------

        //robber pick up
        if (TutorialProgress.tutorial_bools["robber_movement"] && TutorialProgress.tutorial_bools["robber_lantern"] && item_near && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0, "string text", "robber_pickup", 6);
        }

        if (item_gathered)
        {
            DeactivateBool("robber_pickup");
        }
        //-------------------------------------------------------------------------------------

        //robber lantern
        if (TutorialProgress.tutorial_bools["robber_movement"] && item_near && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0, "string text", "robber_lantern", 5);
        }

        if (item_gathered)
        {
            DeactivateBool("robber_lantern");
        }
        //-------------------------------------------------------------------------------------

        //robber light warning
        if (TutorialProgress.tutorial_bools["robber_movement"] && light_active && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0, "string text", "robber_light_warning", 7);
        }

        if (robber_life_lost)
        {
            DeactivateBool("robber_light_warning");
        }
        //--------------------------------------------------------------------------------------

        //robber_item_arrow
        if (TutorialProgress.tutorial_bools["robber_movement"] && item_arrow_active && TutorialProgress.Robber)
        {
            ActivateOverlay(0, 0, "string text", "robber_item_arrow", 8);
        }

        if (item_arrow_actived)
        {
            DeactivateBool("robber_item_arrow");
        }
        //--------------------------------------------------------------------------------------

        //GHOST#####################################################################################################

        //ghost movement
        if (!goRight || !goLeft || !goUp || !goDown && TutorialProgress.Ghost)
        {
            ActivateOverlay(0, 0, "string text", "ghost_movement", 9);
        }

        if (goRight && goLeft && goUp && goDown)
        {
            DeactivateBool("ghost_movement");
        }
        //------------------------------------------------------------------------------------------

        //ghost item gathered
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.tutorial_bools["item_gathered"] && TutorialProgress.Ghost)
        {
            ActivateOverlay(0, 0, "string text", "ghost_item_gathered", 8);
        }


        //------------------------------------------------------------------------------------------

        //ghost items gathered
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.tutorial_bools["ghost_item_gathered"] && TutorialProgress.tutorial_bools["items_gathered"] && TutorialProgress.Ghost)
        {
            ActivateOverlay(0, 0, "string text", "ghost_items_gathered", 1);
        }

        //------------------------------------------------------------------------------------------

        //ghost timer
        if (TutorialProgress.tutorial_bools["ghost_movement"] && timer_near_end && TutorialProgress.Ghost)
        {
            ActivateOverlay(0, 0, "string text", "ghost_timer", 2);
        }

        if (timer_end)
        {
            DeactivateBool("ghost_timer");
        }
        //-------------------------------------------------------------------------------------------

        //ghost dash
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.Ghost)
        {
            ActivateOverlay(0, 0, "string text", "ghost_dash", 5);
        }

        if (ghost_dashed)
        {
            DeactivateBool("ghost_dash");
        }
        //-------------------------------------------------------------------------------------------

        //ghost dash warning
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.tutorial_bools["ghost_dash"] && TutorialProgress.Ghost)
        {
            ActivateOverlay(0, 0, "string text", "ghost_dash_warning", 6);
        }

        //-------------------------------------------------------------------------------------------

        //ghost objective
        if (TutorialProgress.tutorial_bools["ghost_dash"] && robber_near && TutorialProgress.Ghost)
        {
            ActivateOverlay(0, 0, "string text", "ghost_objective", 7);
        }

        //-----------------------------------------------------------------------------------------


        //ghost teleport
        if (TutorialProgress.tutorial_bools["ghost_objective"] && teleported && TutorialProgress.Ghost)
        {
            ActivateOverlay(0,0, "string text", "ghost_teleport", 3);
        }

        //------------------------------------------------------------------------------------------

        //ghost stepvision
        if (TutorialProgress.tutorial_bools["ghost_dash"] && steps_near && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_stepvision"])
        {
            ActivateOverlay(0, 0, "string text", "ghost_stepvision", 4);
        }

        //------------------------------------------------------------------------------------------
    }

    public void ActivateOverlay(float x_pos, float y_pos, string text, string type, float priority)
    {
        RuntimeOverlayScript overlay_script = overlay.GetComponent<RuntimeOverlayScript>();

        if (overlay.activeSelf)
        {
            if (overlay_script.priority < priority)
            {
                overlay_script.textbox.GetComponent<TextMeshPro>().text = text;
                overlay_script.type = type;
                overlay_script.priority = priority;
            }
            else return;
        }
        else
        {
            overlay_script.textbox.GetComponent<TextMeshPro>().text = text;
            overlay_script.type = type;
            overlay_script.priority = priority;
        }

        overlay_script.Activate();
    }

    void WipeTutorial()
    {
        foreach (var key in TutorialProgress.tutorial_bools.Keys.ToList())
        {
            TutorialProgress.tutorial_bools[key] = false;
        }
    }

    void DeactivateBool(string bool_name)
    {
        TutorialProgress.tutorial_bools[bool_name] = true;
    }
}
