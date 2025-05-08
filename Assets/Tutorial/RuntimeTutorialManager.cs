using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using System.Collections;

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
    [HideInInspector] public bool robber_vented = false;
    bool robber_life_lost = false;
    bool robber_near = false;
    [HideInInspector] public bool ghost_dashed = false;
    bool ghost_aiming = false; // new
    [HideInInspector] public bool vent_near = false;
    [HideInInspector] public bool vent_near_ghost = false; // new
    bool light_active = false;
    bool steps_near = false;
    [HideInInspector] public bool teleported = false;

    ItemLottery item_reference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WipeTutorial();
        item_reference = Game.Instance.item_lottery;

        // Passing reference to it to game script
        Game.Instance.rt_tutorial = this;

        // Startting the coroutine that checks for steps_near
        StartCoroutine(StepsNearCheck());
    }

    // Update is called once per frame
    void Update()
    {
        // Some debugging stuff
        // Debug.Log(TutorialProgress.Robber); that is not the problem, TutorialProgress.Robber is assigned

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
       

        //timer check This is getting called a lot for some reason (Its always robbin time)
        if(Game.Instance.timer != null && Game.Instance.timer.current_time < 60 && !timer_near_end && false)
        {
            timer_near_end = true;
        }

        if (Game.Instance.timer != null && Game.Instance.timer.current_time <= 0 && !timer_end && timer_near_end)
        {
            timer_end = true;
        }

        //ghost checkers

        //ghost aiming***
        if (Game.Instance.robber.Value != null && Game.Instance.robber.Value.GetComponent<Player>().indication.gameObject.activeSelf)
        {
            ghost_aiming = true;
        }

        //ghost_dashed;
        // Is done in GhostScript line 411

        //ghost_near;
        if (Game.Instance.robber.Value != null && Game.Instance.ghost.Value != null && Vector2.Distance(Game.Instance.robber.Value.transform.position, Game.Instance.ghost.Value.transform.position) < 9f)
        {
            ghost_near = true;
        }

        //teleported;
        // Is done in GhostScript line 657


        //robber checkers

        //item_arrow_active;
        if (Game.Instance.robber.Value != null && Game.Instance.robber.Value.GetComponent<RobberScript>().item_arrow.gameObject.activeSelf)
        {
            item_arrow_active = true;
        }
        //item_arrow_actived;

        //robber_vented;
        // Is done in Vent script line 249 

        //robber_life_lost
        if (Game.Instance.robber.Value != null && Game.Instance.robber.Value.GetComponent<RobberScript>().robberUI.hp.currentHealth < 3 && !robber_life_lost)
        {
            robber_life_lost = true;
        }

        //light_active
        if (Game.Instance.robber.Value != null && Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf)
        {
            light_active = true;
        }

        //robber_near;
        if (Game.Instance.robber.Value != null && Game.Instance.ghost.Value != null &&  Vector2.Distance(Game.Instance.robber.Value.transform.position, Game.Instance.ghost.Value.transform.position) < 9f)
        {
            ghost_near = true;
        }

        //steps check
        //steps_near;
        //Is done in this script below line 370

        //vent_near;
        // Is done in Vent script line 91

        //vent_near_ghost***
        //Is done in GhostScript line 721

        //implementation of the functions
        //ROBBER#############################################################################################

        //robber movement
        if ((!goRight || !goLeft || !goUp || !goDown) && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_movement"] == false) 
        {
            ActivateOverlay(0, 0, "move around", "robber_movement", 9);
        }

        if (goRight && goLeft && goUp && goDown && TutorialProgress.has_this_activated["robber_movement"] == true)
        {
            DeactivateBool("robber_movement");
        }
        //-----------------------------------------------------------------------------------

        //robber escape
        if (TutorialProgress.tutorial_bools["items_gathered"] && TutorialProgress.tutorial_bools["robber_pickup"] && TutorialProgress.tutorial_bools["robber_movement"]
            && TutorialProgress.tutorial_bools["robber_lantern"] && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_escape"] == false)
        {
            ActivateOverlay(0,0, "get out of here", "robber_escape", 1);
        }
        //-----------------------------------------------------------------------------------

        //robber radar
        if (TutorialProgress.tutorial_bools["robber_movement"] && ghost_near && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_radar"] == false)
        {
            ActivateOverlay(0, 0,"this bitch is close", "robber_radar", 2);
        }

        if (robber_life_lost && TutorialProgress.has_this_activated["robber_radar"] == true) // those happen before they are activated
        {
            DeactivateBool("robber_radar");
        }
        //-------------------------------------------------------------------------------------

        //robber time
        if (TutorialProgress.tutorial_bools["robber_movement"] && timer_near_end && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_timer"] == false)
        {
            ActivateOverlay(0, 0, "its robbin time", "robber_timer", 3);
        }

        if (timer_end && TutorialProgress.has_this_activated["robber_time"] == true)
        {
            DeactivateBool("robber_timer");
        }
        //-------------------------------------------------------------------------------------

        //robber vent
        if (TutorialProgress.tutorial_bools["robber_movement"] && vent_near && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_vent"] == false)
        {
            ActivateOverlay(0, 0, "use vent", "robber_vent", 4);
        }

        if (robber_vented && TutorialProgress.has_this_activated["robber_vent"] == true)
        {
            DeactivateBool("robber_vent");
        }
        //-------------------------------------------------------------------------------------

        //robber pick up
        if (TutorialProgress.tutorial_bools["robber_movement"] && TutorialProgress.tutorial_bools["robber_lantern"] && item_near
            && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_pickup"] == false)
        {
            ActivateOverlay(0, 0, "pick that up", "robber_pickup", 6);
        }

        if (item_gathered && TutorialProgress.has_this_activated["robber_pickup"] == true)
        {
            DeactivateBool("robber_pickup");
        }
        //-------------------------------------------------------------------------------------

        //robber lantern
        if (TutorialProgress.tutorial_bools["robber_movement"] && item_near && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_lantern"] == false)
        {
            ActivateOverlay(0, 0, "Let the light in", "robber_lantern", 5);
        }

        if (item_gathered && TutorialProgress.has_this_activated["robber_lantern"] == true)
        {
            DeactivateBool("robber_lantern");
        }
        //-------------------------------------------------------------------------------------

        //robber light warning
        if (TutorialProgress.tutorial_bools["robber_movement"] && light_active && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_light_warning"] == false) // this check is happening continuosly fix it
        {
            ActivateOverlay(0, 0, "The ghost can see you", "robber_light_warning", 7);

            StartCoroutine(DeactivateBoolTimer(7f, "robber_light_warning")); // Will disappear after 7 seconds
        }

        
        //--------------------------------------------------------------------------------------

        //robber_item_arrow
        if (TutorialProgress.tutorial_bools["robber_movement"] && item_arrow_active && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_item_arrow"] == false)
        {
            ActivateOverlay(0, 0, "string text", "robber_item_arrow", 8);
        }

        if (item_arrow_actived && TutorialProgress.has_this_activated["robber_item_arrow"] == true)
        {
            DeactivateBool("robber_item_arrow");
        }
        //--------------------------------------------------------------------------------------

        //GHOST#####################################################################################################

        //ghost movement
        if ((!goRight || !goLeft || !goUp || !goDown) && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_movement"] == false)
        {
            ActivateOverlay(0, 0, "string text", "ghost_movement", 9);
        }

        if (goRight && goLeft && goUp && goDown && TutorialProgress.has_this_activated["ghost_movement"] == true)
        {
            DeactivateBool("ghost_movement");
        }
        //------------------------------------------------------------------------------------------

        //ghost item gathered
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.tutorial_bools["item_gathered"]
            && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_item_gathered"] == false)
        {
            //ActivateOverlay(0, 0, "string text", "ghost_item_gathered", 8);
        }


        //------------------------------------------------------------------------------------------

        //ghost items gathered
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.tutorial_bools["ghost_item_gathered"]
            && TutorialProgress.tutorial_bools["items_gathered"] && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_items_gathered"] == false)
        {
            //ActivateOverlay(0, 0, "string text", "ghost_items_gathered", 1);
        }

        //------------------------------------------------------------------------------------------

        //ghost timer
        if (TutorialProgress.tutorial_bools["ghost_movement"] && timer_near_end && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_timer"] == false)
        {
            ActivateOverlay(0, 0, "string text", "ghost_timer", 2);
        }

        if (timer_end && TutorialProgress.has_this_activated["ghost_timer"] == true)
        {
            DeactivateBool("ghost_timer");
        }
        //-------------------------------------------------------------------------------------------

        //ghost dash
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_dash"] == false)
        {
            ActivateOverlay(0, 0, "string text", "ghost_dash", 5);
        }

        if (ghost_dashed && TutorialProgress.has_this_activated["ghost_dash"] == true)
        {
            DeactivateBool("ghost_dash");
        }
        //-------------------------------------------------------------------------------------------

        //ghost aiming NEW
        if (TutorialProgress.Robber && ghost_aiming && TutorialProgress.tutorial_bools["ghost_dash"] == false)
        {
            ActivateOverlay(0, 0, "string text", "ghost_aiming", 5);

            StartCoroutine(DeactivateBoolTimer(7f, "ghost_aiming")); // Will disappear after 7 seconds
        }

        
        //-------------------------------------------------------------------------------------------

        //ghost vent near NEW
        if (TutorialProgress.Ghost && vent_near_ghost && TutorialProgress.tutorial_bools["ghost_vent"] == false)
        {
            ActivateOverlay(0, 0, "string text", "ghost_vent", 5);

            // Will disappear after 7 seconds
            StartCoroutine(DeactivateBoolTimer(7f, "ghost_vent"));
        }

       
        //-------------------------------------------------------------------------------------------

        //ghost dash warning
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.tutorial_bools["ghost_dash"] && TutorialProgress.Ghost 
            && TutorialProgress.tutorial_bools["ghost_dash_warning"] == false)
        {
            ActivateOverlay(0, 0, "when you dash the <color = red>robber</color> can see you", "ghost_dash_warning", 6);
        }

        if (TutorialProgress.has_this_activated["ghost_dash_warning"] == true)
        {
            //timer deactivation 
            StartCoroutine(DeactivateBoolTimer(5f, "ghost_dash_warning")); // Will disappear after 5 seconds
        }

        //-------------------------------------------------------------------------------------------

        //ghost objective
        if (TutorialProgress.tutorial_bools["ghost_dash"] && robber_near && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_objective"] == false)
        {
            ActivateOverlay(0, 0, "string text", "ghost_objective", 7);
        }

        if (TutorialProgress.has_this_activated["ghost_objective"] == true)
        {
            //time deactivation
            StartCoroutine(DeactivateBoolTimer(5f, "ghost_objective")); // Will disappear after 5 seconds
        }

        //-----------------------------------------------------------------------------------------


        //ghost teleport
        if (TutorialProgress.tutorial_bools["ghost_objective"] && teleported && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_teleport"] == false)
        {
            ActivateOverlay(0,0, "string text", "ghost_teleport", 3);
        }

        if (TutorialProgress.has_this_activated["ghost_teleport"] == true)
        {
            //time deactivation
            StartCoroutine(DeactivateBoolTimer(5f, "ghost_teleport"));
        }

        //------------------------------------------------------------------------------------------

        //ghost stepvision
        if (TutorialProgress.tutorial_bools["ghost_dash"] && steps_near && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_stepvision"] == false)
        {
            ActivateOverlay(0, 0, "string text", "ghost_stepvision", 4);
        }

        if (TutorialProgress.has_this_activated["ghost_stepvision"] == true)
        {
            //time deactivation
            StartCoroutine(DeactivateBoolTimer(5f, "ghost_stepvision"));
        }

        //------------------------------------------------------------------------------------------
    }

    IEnumerator StepsNearCheck()
    {
        while (steps_near == false)
        {
            GameObject[] footsteps = GameObject.FindGameObjectsWithTag("Footstep");

            for (int a = 0; a < footsteps.Length; a++)
            {
                if (Vector2.Distance(Game.Instance.ghost.Value.transform.position, footsteps[a].transform.position) < 9f)
                {
                    steps_near = true;
                }
            }

            yield return new WaitForSeconds(15f); // it will check if any footsteps are close every 15 seconds
        }
    }

    IEnumerator DeactivateBoolTimer(float delay_in_seconds, string bool_to_deactivate)
    {
        yield return new WaitForSeconds(delay_in_seconds);

        DeactivateBool(bool_to_deactivate);
    }

    public void ActivateOverlay(float x_pos, float y_pos, string text, string type, float priority)
    {
        RuntimeOverlayScript overlay_script = overlay.GetComponent<RuntimeOverlayScript>();

        if (overlay_script.type == type || overlay_script.priority > priority) return; // do nothing if it's already the desired overlay or priority is lower

        // Activating the overlay
        overlay_script.gameObject.SetActive(true);
        overlay_script.Activate(text); // Im passing the text here now

        // Debugging (bacause it doenst work sukaaa)
        Debug.Log("RT_Tutorial: activated type: " + type + " with text: " + text);

        TutorialProgress.has_this_activated[type] = true;

        if (overlay.activeSelf)
        {
            overlay_script.type = type;
            overlay_script.priority = priority;
        }
    }

    void WipeTutorial()
    {
        foreach (var key in TutorialProgress.tutorial_bools.Keys.ToList())
        {
            TutorialProgress.tutorial_bools[key] = false;
        }

        foreach (var key in TutorialProgress.has_this_activated.Keys.ToList())
        {
            TutorialProgress.tutorial_bools[key] = false;
        }
    }

    void DeactivateBool(string bool_name)
    {
        Debug.Log("RT_Tutorial: deactivated type: " + bool_name);
        TutorialProgress.tutorial_bools[bool_name] = true;
    }
}
