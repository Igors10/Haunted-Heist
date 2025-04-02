using UnityEngine;

public class FlashlightAura : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Item") return;
        collision.gameObject.GetComponent<Item>().sprite.enabled = true;

        Debug.Log("FLASHLIGHT: ItemFound");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Item") return;
        collision.gameObject.GetComponent<Item>().sprite.enabled = false;
    }

}