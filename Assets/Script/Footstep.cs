using UnityEngine;

public class Footstep : MonoBehaviour
{
    SpriteRenderer sprite;
    [HideInInspector] public float left_or_right;
    [HideInInspector] public float lifetime;
    float current_lifetime;
    float transparency_decrease;
    float transparency;
    [HideInInspector] public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {
        current_lifetime = 0;
        transparency_decrease = 0.2f * (300f / lifetime);
        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        transform.Translate(left_or_right, 0, 0);
        transparency = 1f;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        current_lifetime++;
        if (current_lifetime % 50 == 0)
        {
            transparency = transparency - transparency_decrease;
            if (current_lifetime == lifetime)
            {
                FootstepManager.deactivated_footsteps.Add(this);
                this.gameObject.SetActive(false);
            }
            sprite.color = new Color(1f, 1f, 1f, transparency);
        }
    }
}
