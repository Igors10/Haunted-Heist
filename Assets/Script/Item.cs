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

    public GameObject pickUp_image;

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
            pickUp_image.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
            {
                // if the robber is in range of the item, show the pick up image
                pickUp_image.SetActive(true);
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
                pickUp_image.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            // if the robber is out of range of the item, hide the pick up image
            pickUp_image.SetActive(false);
        }
    }

    public void OnRobberEnterRange(Collider2D collision)
    {
        if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
        {
            pickUp_image.SetActive(true);
        }
    }

    public void OnRobberStayInRange(Collider2D collision)
    {
        if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
        {
            pickUp_image.SetActive(true);
        }
    }

    public void OnRobberExitRange(Collider2D collision)
    {
        pickUp_image.SetActive(false);
    }
}