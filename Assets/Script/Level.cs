using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] SpriteRenderer floor_sprite;
    [SerializeField] float level_margin_y;
    [SerializeField] float level_margin_x;
    Bounds level_borders;
    public float top_level_border;
    public float bottom_level_border;
    public float left_level_border;
    public float right_level_border;

    private void Start()
    {
        Game.level = GetComponent<Level>();

        level_borders = floor_sprite.bounds;
        top_level_border = level_borders.max.y - level_margin_y;
        bottom_level_border = level_borders.min.y + level_margin_y;
        left_level_border = level_borders.min.x + level_margin_x;
        right_level_border = level_borders.max.x - level_margin_x;
    }
}
