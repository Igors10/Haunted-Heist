using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUp : MonoBehaviour
{
    public GameObject closest_item;
    GameObject SelectionAura;
    [SerializeField] float pick_up_speed_default;
    [SerializeField] GameObject pickupVFXPrefab; // Pickup VFX prefab
    bool is_picking;
    float picking_progress;
    GameObject item_lottery;

    private void Awake()
    {
        SelectionAura = GameObject.Find("WorldCanvas/SelectionAura");
        item_lottery = GameObject.Find("ItemManager");
    }

    void Start()
    {
        if (SelectionAura != null) SelectionAura.SetActive(false);
    }

    void FixedUpdate()
    {
        ItemPicking();
    }

    void ItemPicking()
    {
        if (is_picking && Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == true)
        {
            float pick_up_speed = pick_up_speed_default;
            switch (closest_item.GetComponent<Item>().level)
            {
                case item_level.Medium:
                    pick_up_speed = pick_up_speed_default / 2;
                    break;
                case item_level.Hard:
                    pick_up_speed = pick_up_speed_default / 4;
                    break;
            }

            picking_progress -= pick_up_speed;
            if (picking_progress < 0) FinishPicking();
            else SelectionAura.GetComponent<Image>().fillAmount = picking_progress;
        }
        else if (Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf == false) 
        {
            if (closest_item != null && closest_item.tag != "Vent")
            {
                if (SelectionAura != null) SelectionAura.SetActive(false);
                closest_item = null;
            }
            is_picking = false;
        }
    }

    public void StartPicking(bool picking)
    {
        if (closest_item == null) return;

        if (closest_item.tag == "Vent")
        {
            closest_item.GetComponent<Vent>().OpenVent(picking);
            // use went in some way
            

            return;
        }

        Debug.Log("StartedPicking");
        picking_progress = 1;
        SelectionAura.GetComponent<Image>().fillAmount = 1f;
        is_picking = picking;
    }

    void LockOnItem(GameObject item)
    {
        if (item == null) SelectionAura.SetActive(false);
        else
        {
            closest_item = item;
            SelectionAura.SetActive(true);
            SelectionAura.transform.position = closest_item.transform.position;
        }

    }

    void FinishPicking()
    {
        // Play sound effect
        AudioManager.instance.PlaySFX("ItemPickup");

        // Reset SelectionAura fill amount
        SelectionAura.GetComponent<Image>().fillAmount = 1;

        // Instantiate pickup VFX at the item's position
        if (pickupVFXPrefab != null && closest_item != null)
        {
            Instantiate(pickupVFXPrefab, closest_item.transform.position, Quaternion.identity);
        }

        if (item_lottery == null)
        {
            Debug.Log("item lottery isn't assigned");
        }
        else if (closest_item == null)
        {
            Debug.Log("closest_item isn't assigned");
        }
        else
        {
            item_lottery.GetComponent<ItemLottery>().ItemPicked(closest_item.GetComponent<SpriteRenderer>().sprite);

            if (Game.Instance.robber.Value.TryGetComponent(out robberTutorial component))
            {
                component.itemsGathered = component.itemsGathered + 1;
            }
        }

        if (closest_item != null)
        {
            closest_item.SetActive(false);
            closest_item = null;
            is_picking = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vents
        if (collision.gameObject.tag == "Vent")
        {
            // Experimental code where you press Q and E to use vents
            collision.GetComponent<Vent>().OpenVent(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Picking Items
        if (collision.gameObject.tag != "Item" || Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.activeSelf != true) return;

        if (closest_item == null)
        {
            LockOnItem(collision.gameObject);
        }
        else if (Vector2.Distance(transform.position, closest_item.transform.position) > Vector2.Distance(transform.position, collision.transform.position))
        {
            LockOnItem(collision.gameObject);
        }
    }

   

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.tag != "Item") return;

        if (closest_item != null && closest_item == collision.gameObject)
        {
            LockOnItem(null);
            closest_item = null;
        }
        if (collision.gameObject.tag == "Vent")
        {
            // Experimental code where you press Q and E to use vents
            collision.GetComponent<Vent>().OpenVent(false);
        }
    }
}
