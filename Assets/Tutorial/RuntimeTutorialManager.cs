using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using System.Collections;
using static UnityEditor.Progress;

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

    //struct that allows for easy managing of activated tutorial messages
    public record Prompt
    {
        public float x_pos;
        public float y_pos;
        public string text;
        public string type;
        public float priority;
        public float time_activated;
        public Prompt(float x_pos, float y_pos, string text, string type, float priority)
        {
            this.x_pos = x_pos;
            this.y_pos = y_pos;
            this.text = text;
            this.type = type;
            this.priority = priority;
            this.time_activated = 0;
        }
    }

    //creating a list(queue) of tutorial messages to show, that will keep track of priority and what has been shown
    List<Prompt> TutorialPromptsToShow = new List<Prompt>();



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

        overlay.GetComponent<RuntimeOverlayScript>().priority = 100f;

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
        //activating the top message from the queue
        if (TutorialPromptsToShow.Count > 0)
        {
            //removing the message from the queue after 5 seconds
            if (TutorialPromptsToShow[0].time_activated > 5f)
            {
                TutorialPromptsToShow.RemoveAt(0);
                // marking this type as shown
                TutorialProgress.has_this_been_shown[TutorialPromptsToShow[0].type] = true;
            }

            ActivateOverlay(TutorialPromptsToShow[0].x_pos, TutorialPromptsToShow[0].y_pos, TutorialPromptsToShow[0].text, TutorialPromptsToShow[0].type, TutorialPromptsToShow[0].priority);
            TutorialPromptsToShow[0].time_activated += Time.deltaTime;
        }

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
        if ((!goRight || !goLeft || !goUp || !goDown) && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_movement"] == false)
        {
            //ActivateOverlay(0, 0, "Use the <color=yellow>{key}</color> to move.", "robber_movement", 9);
            AddToQueue(0, 0, "Use the <color=yellow>{key}</color> to move.", "robber_movement", 9);

        }

        //-----------------------------------------------------------------------------------

        //robber escape
        if (items_gathered && TutorialProgress.has_this_activated["robber_pickup"] && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_escape"] == false)
        {
            AddToQueue(0, 0, "Escape the mansion through one of the <color=yellow>Doors</color> to win!", "robber_escape", 1);
        }

        //-----------------------------------------------------------------------------------

        //robber radar
        if (ghost_near && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_radar"] == false)
        {
            AddToQueue(0, 0, "Your light is flickering, which means that the <color=#099>Ghost</color> is close! Use your <color=green>Night Vision</color> <color=yellow>{key}</color> to see it.", "robber_radar", 2);
                
        }

        //-------------------------------------------------------------------------------------

        //robber time
        if (timer_near_end && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_timer"] == false)
        {
            AddToQueue(0, 0, "Watch out for the <color=yellow>time limit</color>!", "robber_timer", 3);
        }

        //-------------------------------------------------------------------------------------

        //robber vent
        if (vent_near && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_vent"] == false)
        {
            AddToQueue(0, 0, "You can use <color=yellow>{key}</color> to crawl through vents.", "robber_vent", 4);
        }

        if (robber_vented && TutorialProgress.has_this_activated["robber_vent"] == true)
        {
            AddToQueue(0, 0, "Vents become <color=yellow>closed</color> after being used. They will open again after some time has passed.", "robber_vent_used", 3);
        }
        //-------------------------------------------------------------------------------------

        //robber pick up
        if (item_near && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_pickup"] == false)
        {
            AddToQueue(0, 0, "Collect an item by holding <color=yellow>{key}</color> while using your <color=yellow>Lantern</color>.", "robber_pickup", 5);
        }

        //-------------------------------------------------------------------------------------

        //Robber pick up done
        if (item_gathered && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_pickup_done"] == false)
        {
            AddToQueue(0, 0, "Your goal is to collect all of the <color=yellow>Items</color> listed on the right. Just watch out for the <color=#099>Ghost</color>!", "robber_pickup_done", 4);
        }

        //-------------------------------------------------------------------------------------

        //Robber life lost
        if (robber_caught && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_life_lost"] == false)
        {
            AddToQueue(0, 0, "Ouch! The <color=#099>Ghost</color> just caught you. Don't worry you have two more lives.", "robber_life_lost", 1);
        }

        //-------------------------------------------------------------------------------------

        //robber lantern
        if (TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_lantern"] == false)
        {
            AddToQueue(0, 0, "Use your <color=yellow>Lantern</color> <color=yellow>{key}</color> to find Items around the map. Be careful though, the <color=#099>Ghost</color> can see the light from it!", "robber_lantern", 6);
        }

        //-------------------------------------------------------------------------------------

        //robber light warning
        if (light_active && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_light_warning"] == false)
        {
            AddToQueue(0, 0, "The ghost can see you", "robber_light_warning", 7);
        }

        //--------------------------------------------------------------------------------------

        //ghost aiming NEW
        if (TutorialProgress.Robber && ghost_aiming && TutorialProgress.has_this_activated["ghost_aiming"] == false)
        {
            AddToQueue(0, 0, "This arrow shows the <color=#099>Ghost's</color> location when it is preparing for a <color=red>Dash.</color>.", "ghost_aiming", 5);
        }

        //-------------------------------------------------------------------------------------------

        //robber_item_arrow
        if (item_arrow_active && TutorialProgress.Robber && TutorialProgress.has_this_activated["robber_item_arrow"] == false)
        {
            AddToQueue(0, 0, "Follow the yellow <color=yellow>arrow</color> to find Items yet to be collected.", "robber_item_arrow", 8);
        }

        //--------------------------------------------------------------------------------------

        //GHOST#####################################################################################################

        //ghost movement
        if ((!goRight || !goLeft || !goUp || !goDown) && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_movement"] == false)
        {
            AddToQueue(0, 0, "Use the <color=yellow>{key}</color> to move.", "ghost_movement", 9);
        }

        //------------------------------------------------------------------------------------------

        //ghost item gathered
        if (TutorialProgress.has_this_activated["item_gathered"] && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_item_gathered"] == false)
        {
            //AddToQueue(0, 0, "string text", "ghost_item_gathered", 8);
        }


        //------------------------------------------------------------------------------------------

        //ghost items gathered
        if (TutorialProgress.has_this_activated["ghost_item_gathered"] && TutorialProgress.has_this_activated["items_gathered"] && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_items_gathered"] == false)
        {
            //AddToQueue(0, 0, "string text", "ghost_items_gathered", 1);
        }

        //------------------------------------------------------------------------------------------

        //ghost timer
        if (timer_near_end && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_timer"] == false)
        {
            AddToQueue(0, 0, "Delay the <color=green>Robber</color> a bit longer to gain an advantage!", "ghost_timer", 2);
        }

        //-------------------------------------------------------------------------------------------

        //ghost dash
        if (TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_dash"] == false)
        {
            AddToQueue(0, 0, "Your goal is to find and catch the <color=green>Robber</color>, using your <color=red>Dash</color> <color=yellow>{key}</color>.", "ghost_dash", 5);
        }

        //-------------------------------------------------------------------------------------------

        //ghost vent near NEW
        if (TutorialProgress.Ghost && vent_near_ghost && TutorialProgress.has_this_activated["ghost_vent"] == false)
        {
            AddToQueue(0, 0, "The <color=green>Robber</color> can use vents to move around quickly.", "ghost_vent", 5);
        }

       
        //-------------------------------------------------------------------------------------------

        //ghost dash warning
        if (TutorialProgress.has_this_activated["ghost_dash"] && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_dash_warning"] == false && ghost_dashed)
        {
            AddToQueue(0, 0, "<color=red>Dashing</color> can let you move through objects, but it also reveals your location to the <color=green>Robber</color>.", "ghost_dash_warning", 6);
        }

        //-------------------------------------------------------------------------------------------

        //ghost objective
        if (TutorialProgress.has_this_activated["ghost_dash"] && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_objective"] == false)
        {
            AddToQueue(0, 0, "Find the <color=green>Robber</color> and <color=red>Dash</color> into him to catch him.", "ghost_objective", 7);
                
        }

        //-----------------------------------------------------------------------------------------


        //ghost teleport
        if (teleported && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_teleport"] == false)
        {
            AddToQueue(0, 0, "You will be teleported away after catching the <color=green>Robber.</color> Catch him <color=yellow>twice more</color> to win!", "ghost_teleport", 1);

        }

        //------------------------------------------------------------------------------------------

        //ghost stepvision
        if (TutorialProgress.has_this_activated["ghost_dash"] && steps_near && TutorialProgress.Ghost && TutorialProgress.has_this_activated["ghost_stepvision"] == false)
        {
            AddToQueue(0, 0, "You can use your <color=#27DDFF>Step Vision</color> <color=yellow>{key}</color> to see footprints the <color=green>Robber.</color> leaves behind.", "ghost_stepvision", 4);
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


    public void AddToQueue(float x_pos, float y_pos, string text, string type, float priority)
    {
        TutorialPromptsToShow.Add(new Prompt(x_pos, y_pos, text, type, priority));
        Debug.Log("RT_Tutorial: added to queue type: " + type + " with text: " + text);
        TutorialPromptsToShow.Sort((a, b) => b.priority.CompareTo(a.priority)); // Sorting the list by priority (highest first)
        Debug.Log("RT_Tutorial: queue sorted by priority");
        Debug.Log("RT_Tutorial: queue count: " + TutorialPromptsToShow.Count);
        Debug.Log("RT_Tutorial: " + TutorialPromptsToShow[0].type + " is at the top of the queue");
        TutorialProgress.has_this_activated[type] = true; // Marking this type as activated
        Debug.Log("RT_Tutorial: marked " + type + " as activated");

        for(int i = TutorialPromptsToShow.Count - 1; i >= 0; i--)
        {
            if (TutorialPromptsToShow[i].time_activated != 0)
            {
                TutorialPromptsToShow[i].time_activated = 0;
            }
        }
    }

    public bool ActivateOverlay(float x_pos, float y_pos, string text, string type, float priority)
    {
        RuntimeOverlayScript overlay_script = overlay.GetComponent<RuntimeOverlayScript>();

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
        foreach (var key in TutorialProgress.has_this_activated.Keys.ToList())
        {
            TutorialProgress.has_this_activated[key] = false;
        }

        foreach (var key in TutorialProgress.has_this_been_shown.Keys.ToList())
        {
            TutorialProgress.has_this_been_shown[key] = false;
        }

        TutorialProgress.Robber = false;
        TutorialProgress.Ghost = false;
    }
}
