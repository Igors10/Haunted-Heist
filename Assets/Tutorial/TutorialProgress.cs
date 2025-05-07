using System.Collections.Generic;
using UnityEngine;

public static class TutorialProgress
{
    public static int part = 1; //what stage the tutorial is in

    //which instance of the tutorial must be shown
    public static bool Robber;
    public static bool Ghost;


    //TUTORIAL AT RUNTIME
    public static bool is_tutorial_active = true; //will the tutorial activate in the game

    public static Dictionary<string, bool> tutorial_bools = new Dictionary<string, bool>()
    {
        { "robber_movement", false },
        { "robber_lantern", false },
        { "robber_pickup", false },
        { "robber_light_warning", false },
        { "robber_radar", false },
        { "robber_vent", false },
        { "robber_item_arrow", false },
        { "robber_timer", false },
        { "robber_escape", false },
        { "robber_ghost_aiming", false }, // new 


        { "ghost_movement", false },
        { "ghost_vent", false }, // new
        { "ghost_dash", false }, 
        { "ghost_stepvision", false },
        { "ghost_dash_warning", false },
        { "ghost_objective", false },
        { "ghost_teleport", false },
        { "ghost_timer", false },
        { "ghost_items", false },
        { "ghost_items_gathered", false }
    };

    /*
    0 bool 1 robber_escape;
    1 bool 2 robber_radar;
    2 bool 3 robber_timer;
    3 bool 4 robber_vent;
    4 bool 5 robber_lantern;
    5 bool 6 robber_pickup;
    6 bool 7 robber_light_warning;
    7 bool 8 robber_item_arrow;
    8 bool 9 robber_movement;

    9  bool 1 ghost_items_gathered;
    10 bool 2 ghost_timer;
    11 bool 3 ghost_teleport;
    12 bool 4 ghost_stepvision;
    13 bool 5 ghost_dash;
    14 bool 6 ghost_dash_warning;
    15 bool 7 ghost_objective;
    16 bool 8 ghost_items;
    17 bool 9 ghost_movement;
    */
    
}
