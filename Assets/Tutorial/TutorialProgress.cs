using UnityEngine;

public static class TutorialProgress
{
    public static int part = 1; //what stage the tutorial is in

    public static bool is_tutorial_active = true; //will the tutorial activate in the game


    //switch enum?
    public static bool robber_movement;
    public static bool robber_lantern;
    public static bool robber_pickup;
    public static bool robber_light_warning;
    public static bool robber_radar;
    public static bool robber_vent;
    public static bool robber_item_arrow;
    public static bool robber_timer;
    public static bool robber_escape;

    public static bool ghost_movement;
    public static bool ghost_dash;
    public static bool ghost_stepvision;
    public static bool ghost_dash_warning;
    public static bool ghost_objective;
    public static bool ghost_teleport;
    public static bool ghost_timer;
    public static bool ghost_items;
    public static bool ghost_items_gathered;
}
