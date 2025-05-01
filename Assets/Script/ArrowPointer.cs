using UnityEngine;
using System.Collections;

public class ArrowPointer : MonoBehaviour
{
    public Vector3 target;
    [SerializeField] float hide_distance;
    [SerializeField] SpriteRenderer arrow_sprite;
    bool target_close = false;
    [SerializeField] float arrow_pointing_interval;
    [HideInInspector] public GameObject object_pointer;
    [SerializeField] bool object_pointer_used;

    private void Start()
    {
       object_pointer = GameObject.Find("ObjectPointer");
       if (object_pointer == null) Debug.Log("Object pointer is not found");
    }

    // Update is called once per frame
    void Update()
    {
        ArrowRotation();
        CheckDistance();
        CheckGameOver();
    }

    void CheckGameOver()
    {
        // Disable those when game over

        if (GameData.is_game_over) Destroy(this.gameObject);
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
            if (arrow_sprite.enabled && object_pointer != null && object_pointer_used) object_pointer.GetComponent<ObjectPointer>().ActivatePointer(target);
            arrow_sprite.enabled = false;
        }
        else if (!arrow_sprite.enabled)
        {
            arrow_sprite.enabled = true;
            object_pointer.GetComponent<ObjectPointer>().DeactivatePointer();
        }
    }

}
