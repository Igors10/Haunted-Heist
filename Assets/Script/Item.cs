using UnityEngine;

public enum item_level
{
    Easy,
    Medium,
    Hard
}
public class Item : MonoBehaviour
{
    public item_level level;
    public SpriteRenderer sprite;
    public int item_id;

    [HideInInspector] public bool is_ready_for_pickup;
    public GameObject pickUp_image;
    [SerializeField] Sprite spacebar_icon;
    [SerializeField] Sprite right_tab_icon;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;

        // Deactivate self if no sprite attached by ItemLottery
        if (sprite.sprite == null) this.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Check if the flashlight is off and disable the pickup image
        if (Game.Instance.robber.Value != null && Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == false)
        {
            PickUpImage(false);
        }
    }

    void PickUpImage(bool is_active)
    {
        pickUp_image.GetComponent<SpriteRenderer>().sprite = (GameData.is_gamepad_used) ? right_tab_icon : spacebar_icon;
        pickUp_image.SetActive(is_active);
        is_ready_for_pickup = is_active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
            {
                // if the robber is in range of the item, show the pick up image
                PickUpImage(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
            {
                // if the robber is in range of the item, show the pick up image
                PickUpImage(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            // if the robber is out of range of the item, hide the pick up image
            PickUpImage(false);
        }
    }

    public void OnRobberEnterRange(Collider2D collision)
    {
        if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
        {
            PickUpImage(true);
        }
    }

    public void OnRobberStayInRange(Collider2D collision)
    {
        if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
        {
            PickUpImage(true);
        }
    }

    public void OnRobberExitRange(Collider2D collision)
    {
        PickUpImage(false);
    }
}