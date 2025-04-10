using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Vector3 target;
    [SerializeField] float hide_distance;
    [SerializeField] SpriteRenderer arrow_sprite;
   
    // Update is called once per frame
    void Update()
    {
        ArrowRotation();
        CheckDistance();
    }

    void ArrowRotation()
    {
        Vector3 target_direction = (target - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, target_direction);
    }
    void CheckDistance()
    {
        if (Vector2.Distance(target, transform.position) < hide_distance)
        {
            arrow_sprite.enabled = false;
        }
        else if (!arrow_sprite.enabled) arrow_sprite.enabled = true;
    }
}
