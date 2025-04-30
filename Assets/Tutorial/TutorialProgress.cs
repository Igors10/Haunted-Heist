using System.Collections.Generic;
using UnityEngine;

public static class TutorialProgress
{
    public static int part = 1; //what stage the tutorial is in


    //TUTORIAL AT RUNTIME
    public static bool is_tutorial_active = true; //will the tutorial activate in the game


    public static List<bool> tutorial_bools = new List<bool>();

    /*
    0 bool robber_movement;
    1 bool robber_lantern;
    2 bool robber_pickup;
    3 bool robber_light_warning;
    4 bool robber_radar;
    5 robber_vent;
    6 robber_item_arrow;
    7 bool robber_timer;
    8 bool robber_escape;

    9 bool ghost_movement;
    10 bool ghost_dash;
    11 bool ghost_stepvision;
    12 bool ghost_dash_warning;
    13 bool ghost_objective;
    14 bool ghost_teleport;
    15 bool ghost_timer;
    16 bool ghost_items;
    17 bool ghost_items_gathered;

    */
}
