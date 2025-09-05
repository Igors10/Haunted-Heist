public static class GameData
{
    public static bool is_game_over = false;
    public static bool is_ghost_wild = false;
    public static bool is_gamepad_used = false;
    public static bool are_hints_on = true;
    public static string current_ip = "localhost";
    public static bool is_server = false;
    public static bool is_looping = false;
    public static bool is_restarting = false;
    public static string nickname = "nameless";
    public static int nextID = 0;
    public static character character_selected;

    public static bool disableCreateServerButton = false;
    public static bool disableInputField = false;
}
