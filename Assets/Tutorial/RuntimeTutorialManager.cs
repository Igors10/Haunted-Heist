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
    [SerializeField] TextMeshProUGUI hint_input_text;
    [SerializeField] bool is_anim_on;

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
    [HideInInspector] public bool item_near = false;
    bool item_arrow_active = false;
    bool item_arrow_actived = false;
    [HideInInspector] public bool robber_vented = false;
    bool robber_life_lost = false;
    bool robber_caught = false;
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

        overlay.GetComponent<RuntimeOverlayScript>().priority = 10f;

        // Setting the rt_tutorial on or off based on the prev input (on by default, becomes off after playing once)
        hint_input_text.gameObject.SetActive(true);
        SwitchHintMode(GameData.are_hints_on);
        overlay.SetActive(false);
    }

    void HintInput() // Turning the Embedded tutorial on and off on Y button
    {
        if (Input.GetButtonDown("RT_TutorialOn/Off"))
        {
            GameData.are_hints_on = (GameData.are_hints_on) ? false : true;
            SwitchHintMode(GameData.are_hints_on);
        }
    }

    void SwitchHintMode(bool is_on)
    {
        hint_input_text.text = (is_on) ? "Hints are <color=green> On.</color> <color=yellow>(Y)</color>" : "Hints are <color=red>Off</color> <color=yellow>(Y)</color>";

        overlay.SetActive(is_on);
    }

    // Update is called once per frame
    void Update()
    {
        HintInput();
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


        //items gathered (multiple - all of them)
        if (item_reference.all_items_collected && !items_gathered)
        {
            items_gathered = true;
        }

        //item_near

        //item gathered
        if (item_reference.item_picked && !item_gathered)
        {
            item_gathered = true;
        }


        //timer check This is getting called a lot for some reason (Its always robbin time)
        if (Game.Instance.timer != null && Game.Instance.timer.current_time < 60 && !timer_near_end && false)
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

        //robber_vented;
        // Is done in Vent script line 205

        //robber_life_lost
        
           
        if (TutorialProgress.Ghost && Game.Instance.ghost.Value != null && Game.Instance.ghost.Value.GetComponent<GhostScript>().ghostUI != null && 
            Game.Instance.ghost.Value.GetComponent<GhostScript>().ghostUI.souls.currentHealth > 0 && !robber_life_lost)
        {
            robber_life_lost = true;
        }

        if (TutorialProgress.Robber && Game.Instance.robber.Value != null && Game.Instance.robber.Value.GetComponent<RobberScript>().robberUI != null &&
            Game.Instance.robber.Value.GetComponent<RobberScript>().robberUI.hp.currentHealth < 3)
        {
            robber_caught = true;
        }



        //light_active
        if (Game.Instance.robber.Value != null && Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf)
        {
            light_active = true;
        }

        //robber_near;
        if (Game.Instance.robber.Value != null && Game.Instance.ghost.Value != null && Vector2.Distance(Game.Instance.robber.Value.transform.position, Game.Instance.ghost.Value.transform.position) < 9f)
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
            ActivateOverlay(0, 0, "Use the <color=yellow>{key}</color> to move.", "robber_movement", 9);
        }

        if ((goRight || goLeft || goUp || goDown) && TutorialProgress.has_this_activated["robber_movement"] == true)
        {
            StartCoroutine(DeactivateBoolTimer(1.5f, "robber_movement")); // Will disappear after 5 seconds
        }

        //-----------------------------------------------------------------------------------

        //robber escape
        if (items_gathered && TutorialProgress.tutorial_bools["robber_pickup"] && TutorialProgress.tutorial_bools["robber_movement"]
             && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_escape"] == false)
        {
            ActivateOverlay(0, 0, "Escape the mansion through one of the <color=yellow>Doors</color> to win!", "robber_escape", 1);
        }

        if (timer_end && TutorialProgress.has_this_activated["robber_escape"] == true)
        {
            DeactivateBool("robber_escape");
        }
        //-----------------------------------------------------------------------------------



        //robber radar
        if (TutorialProgress.tutorial_bools["robber_movement"] && ghost_near && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_radar"] == false)
        {
            if (ActivateOverlay(0, 0, "Your light is flickering, which means that the <color=#099>Ghost</color> is close! Use your <color=green>Night Vision</color> <color=yellow>{key}</color> to see it.", "robber_radar", 2)) 
                StartCoroutine(DeactivateBoolTimer(5f, "robber_radar")); // Will disappear after 5 seconds
        }

        if (timer_end && TutorialProgress.has_this_activated["robber_radar"] == true)
        {
            DeactivateBool("robber_radar");
        }
        //-------------------------------------------------------------------------------------

        //robber time
        if (TutorialProgress.tutorial_bools["robber_movement"] && timer_near_end && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_timer"] == false)
        {
            ActivateOverlay(0, 0, "Watch out for the <color=yellow>time limit</color>!", "robber_timer", 3);
        }

        if (timer_end && TutorialProgress.has_this_activated["robber_time"] == true)
        {
            DeactivateBool("robber_timer");
        }
        //-------------------------------------------------------------------------------------

        //robber vent
        if (TutorialProgress.tutorial_bools["robber_movement"] && vent_near && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_vent"] == false)
        {
            if (ActivateOverlay(0, 0, "You can use <color=yellow>{key}</color> to crawl through vents.", "robber_vent", 4))
                StartCoroutine(DeactivateBoolTimer(5f, "robber_vent")); // Will disappear after 5 seconds
        }

        if (robber_vented && TutorialProgress.has_this_activated["robber_vent"] == true)
        {
            DeactivateBool("robber_vent");

            ActivateOverlay(0, 0, "Vents become <color=yellow>closed</color> after being used. They will open again after some time has passed.", "robber_vent_used", 3);

            StartCoroutine(DeactivateBoolTimer(5f, "robber_vent_used")); // Will disappear after 5 seconds
        }
        //-------------------------------------------------------------------------------------

        //robber pick up
        if (TutorialProgress.tutorial_bools["robber_movement"] && item_near
            && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_pickup"] == false)
        {
            ActivateOverlay(0, 0, "Collect an item by holding <color=yellow>{key}</color> while using your <color=yellow>Lantern</color>.", "robber_pickup", 5);
        }

        if (item_gathered && TutorialProgress.has_this_activated["robber_pickup"] == true)
        {
            DeactivateBool("robber_pickup");
        }
        //-------------------------------------------------------------------------------------

        //Robber pick up done
        if (item_gathered && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_pickup_done"] == false)
        {
            if (ActivateOverlay(0, 0, "Your goal is to collect all of the <color=yellow>Items</color> listed on the right. Just watch out for the <color=#099>Ghost</color>!", "robber_pickup_done", 4))
                StartCoroutine(DeactivateBoolTimer(10f, "robber_pickup_done")); // Will disappear after 5 seconds
        }
        //-------------------------------------------------------------------------------------

        //Robber life lost
        if (robber_caught && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_life_lost"] == false)
        {
            if (ActivateOverlay(0, 0, "Ouch! The <color=#099>Ghost</color> just caught you. Don't worry you have two more lives.", "robber_life_lost", 1))
                StartCoroutine(DeactivateBoolTimer(5f, "robber_life_lost")); // Will disappear after 5 seconds
        }
        //-------------------------------------------------------------------------------------

        //robber lantern
        if (TutorialProgress.tutorial_bools["robber_movement"] && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_lantern"] == false)
        {
            ActivateOverlay(0, 0, "Use your <color=yellow>Lantern</color> <color=yellow>{key}</color> to find Items around the map. Be careful though, the <color=#099>Ghost</color> can see the light from it!", "robber_lantern", 6);
        }

        if (item_gathered && TutorialProgress.has_this_activated["robber_lantern"] == true)
        {
            DeactivateBool("robber_lantern");
        }
        //-------------------------------------------------------------------------------------

        //robber light warning
        if (TutorialProgress.tutorial_bools["robber_movement"] && light_active && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_light_warning"] == false) // this check is happening continuosly fix it
        {
            //ActivateOverlay(0, 0, "The ghost can see you", "robber_light_warning", 7);

            //StartCoroutine(DeactivateBoolTimer(4f, "robber_light_warning")); // Will disappear after 7 seconds
        }
        //--------------------------------------------------------------------------------------
        
        //ghost aiming NEW
        if (TutorialProgress.Robber && ghost_aiming && TutorialProgress.tutorial_bools["ghost_aiming"] == false)
        {
            if (ActivateOverlay(0, 0, "This arrow shows the <color=#099>Ghost's</color> location when it is preparing for a <color=red>Dash.</color>.", "ghost_aiming", 5)) StartCoroutine(DeactivateBoolTimer(7f, "ghost_aiming")); // Will disappear after 7 seconds
        }

        if (item_gathered && TutorialProgress.has_this_activated["ghost_aiming"] == true)
        {
            DeactivateBool("ghost_aiming");
        }
        //-------------------------------------------------------------------------------------------

        //robber_item_arrow
        if (TutorialProgress.tutorial_bools["robber_movement"] && item_arrow_active && TutorialProgress.Robber && TutorialProgress.tutorial_bools["robber_item_arrow"] == false)
        {
            if (ActivateOverlay(0, 0, "Follow the yellow <color=yellow>arrow</color> to find Items yet to be collected.", "robber_item_arrow", 8)) 
                StartCoroutine(DeactivateBoolTimer(8f, "robber_item_arrow")); // Will disappear after 5 seconds
        }

        //--------------------------------------------------------------------------------------

        //GHOST#####################################################################################################

        //ghost movement
        if ((!goRight || !goLeft || !goUp || !goDown) && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_movement"] == false)
        {
            ActivateOverlay(0, 0, "Use the <color=yellow>{key}</color> to move.", "ghost_movement", 9);
        }

        if ((goRight || goLeft || goUp || goDown) && TutorialProgress.has_this_activated["ghost_movement"] == true)
        {
            StartCoroutine(DeactivateBoolTimer(1.5f, "ghost_movement")); // Will disappear after 5 seconds
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
            ActivateOverlay(0, 0, "Delay the <color=green>Robber</color> a bit longer to gain an advantage!", "ghost_timer", 2);
        }

        if (timer_end && TutorialProgress.has_this_activated["ghost_timer"] == true)
        {
            DeactivateBool("ghost_timer");
        }
        //-------------------------------------------------------------------------------------------

        //ghost dash
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_dash"] == false)
        {
            ActivateOverlay(0, 0, "Your goal is to find and catch the <color=green>Robber</color>, using your <color=red>Dash</color> <color=yellow>{key}</color>.", "ghost_dash", 5);
        }

        if (ghost_dashed && TutorialProgress.has_this_activated["ghost_dash"] == true)
        {
            DeactivateBool("ghost_dash");
        }
        //-------------------------------------------------------------------------------------------

        //ghost vent near NEW
        if (TutorialProgress.Ghost && vent_near_ghost && TutorialProgress.tutorial_bools["ghost_vent"] == false)
        {
            ActivateOverlay(0, 0, "The <color=green>Robber</color> can use vents to move around quickly.", "ghost_vent", 5);

            // Will disappear after 7 seconds
            StartCoroutine(DeactivateBoolTimer(7f, "ghost_vent"));
        }

       
        //-------------------------------------------------------------------------------------------

        //ghost dash warning
        if (TutorialProgress.tutorial_bools["ghost_movement"] && TutorialProgress.tutorial_bools["ghost_dash"] && TutorialProgress.Ghost 
            && TutorialProgress.tutorial_bools["ghost_dash_warning"] == false && ghost_dashed)
        {
            if (ActivateOverlay(0, 0, "<color=red>Dashing</color> can let you move through objects, but it also reveals your location to the <color=green>Robber</color>.", "ghost_dash_warning", 6))
                StartCoroutine(DeactivateBoolTimer(5f, "ghost_dash_warning")); // Will disappear after 5 seconds
        }

        if (TutorialProgress.has_this_activated["ghost_dash_warning"] == true)
        {
            //timer deactivation 
            
        }

        //-------------------------------------------------------------------------------------------

        //ghost objective
        if (TutorialProgress.tutorial_bools["ghost_dash"] && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_objective"] == false)
        {
            ActivateOverlay(0, 0, "Find the <color=green>Robber</color> and <color=red>Dash</color> into him to catch him.", "ghost_objective", 7);
                
        }

        if (TutorialProgress.has_this_activated["ghost_objective"] == true && robber_life_lost)
        {
            //time deactivation
            StartCoroutine(DeactivateBoolTimer(5f, "ghost_objective")); // Will disappear after 5 seconds
        }

        //-----------------------------------------------------------------------------------------


        //ghost teleport
        if (teleported && TutorialProgress.Ghost && TutorialProgress.tutorial_bools["ghost_teleport"] == false)
        {
            ActivateOverlay(0, 0, "You will be teleported away after catching the <color=green>Robber.</color> Catch him <color=yellow>twice more</color> to win!", "ghost_teleport", 1);

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
            ActivateOverlay(0, 0, "You can use your <color=#27DDFF>Step Vision</color> <color=yellow>{key}</color> to see footprints the <color=green>Robber.</color> leaves behind.", "ghost_stepvision", 4);
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

    public bool ActivateOverlay(float x_pos, float y_pos, string text, string type, float priority)
    {
        

        RuntimeOverlayScript overlay_script = overlay.GetComponent<RuntimeOverlayScript>();

        if (overlay_script.type == type || overlay_script.priority < priority || GameData.are_hints_on == false || GameData.is_game_over) return false;
        
        if (!overlay_script.CanActivate()) return false;
        overlay_script.lastActivatedTime = Time.time;

         // do nothing if it's already the desired overlay or priority is lower

        if (TutorialProgress.has_this_activated.ContainsKey(type))
            TutorialProgress.has_this_activated[type] = true;

        // Activating the overlay
        overlay_script.gameObject.SetActive(true);
        overlay_script.Activate(text, type, priority); // Im passing the text here now
        if (is_anim_on) StartCoroutine(overlay_script.smoke_animation.PlayAnimation());

        // Debugging (bacause it doenst work sukaaa((((((  )
        Debug.Log("RT_Tutorial: activated type: " + type + " with text: " + text);

        return true;
    }

    public void WipeTutorial()
    {
        foreach (var key in TutorialProgress.tutorial_bools.Keys.ToList())
        {
            TutorialProgress.tutorial_bools[key] = false;
        }

        foreach (var key in TutorialProgress.has_this_activated.Keys.ToList())
        {
            TutorialProgress.has_this_activated[key] = false;
        }

        TutorialProgress.Robber = false;
        TutorialProgress.Ghost = false;
    }

    void DeactivateBool(string bool_name)
    {
        Debug.Log("RT_Tutorial: deactivated type: " + bool_name);
        TutorialProgress.tutorial_bools[bool_name] = true;
    }
}
