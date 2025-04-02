using UnityEngine;

public enum camera_mode
{
    ROBBER,
    GHOST,
    ROBBER_SPECIAL,
    GHOST_SPECIAL
}
public class CameraBehavior : MonoBehaviour
{
    [HideInInspector]
    public Vector2 to_follow;
    [SerializeField] float speed;
    public GameObject robber_filter;
    public GameObject ghost_filter;
    float camera_left_border, camera_right_border, camera_top_border, camera_bottom_border, camera_height, camera_width;

    void FixedUpdate()
    {
        Follow();
        CameraBorders();
    }
    void Follow()
    {
        if (to_follow == null) return;
        Vector2 new_position = new Vector2(to_follow.x, to_follow.y);
        transform.position = Vector2.Lerp(transform.position, to_follow, speed);
        transform.position += new Vector3(0, 0, -10f); // Fix this!!!
    }

    void CameraBorders()
    {
        camera_height = GetComponent<Camera>().orthographicSize;
        camera_width = GetComponent<Camera>().aspect * camera_height;

        camera_left_border = transform.position.x - camera_width / 2;
        camera_bottom_border = transform.position.y - camera_height / 2;
        camera_right_border = transform.position.x + camera_width / 2;
        camera_top_border = transform.position.y + camera_height / 2;

        if (camera_left_border < Game.level.left_level_border) transform.position =
                new Vector3(Game.level.left_level_border + camera_width / 2, transform.position.y, transform.position.z);
        if (camera_bottom_border < Game.level.bottom_level_border) transform.position =
                new Vector3(transform.position.x, Game.level.bottom_level_border + camera_height / 2, transform.position.z);
        if (camera_right_border > Game.level.right_level_border) transform.position =
                new Vector3(Game.level.right_level_border - camera_width / 2, transform.position.y, transform.position.z);
        if (camera_top_border > Game.level.top_level_border) transform.position =
                new Vector3(transform.position.x, Game.level.top_level_border - camera_height / 2, transform.position.z);
    }
    public void SpecialVision(bool is_on, bool is_robber)
    {
        //SFX
        if (is_robber && is_on) AudioManager.instance.PlaySFX("Nightvision");
        else if (is_on) AudioManager.instance.PlaySFX("Stepvision");

        
        if (is_robber)
        {
            robber_filter.SetActive(is_on);

            if (is_on) CameraMode(camera_mode.ROBBER_SPECIAL);
            else CameraMode(camera_mode.ROBBER);
        }
        else 
        {
            ghost_filter.SetActive(is_on);

            if (is_on) CameraMode(camera_mode.GHOST_SPECIAL);
            else CameraMode(camera_mode.GHOST);
        }
    }

    public void CameraMode(camera_mode mode)
    {
        switch (mode)
        {
            case camera_mode.ROBBER:
                GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "UI", "UI_for_robber", "Robber", "NormalVision", "AttackingGhost");
                break;
            case camera_mode.GHOST:
                GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "UI", "Ghost", "UI_for_ghost", "Robber", "AttackingGhost");
                break;
            case camera_mode.ROBBER_SPECIAL:
                GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "UI", "UI_for_robber", "Messages", "Nightvision", "Ghost", "Robber", "AttackingGhost");
                break;
            case camera_mode.GHOST_SPECIAL:
                GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "UI", "Ghost", "Stepvision", "Messages", "Robber", "AttackingGhost");
                break;

        }
    }
}
