using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FishNet.Serializing.Helping;
using System.Linq;

public class ItemLottery : MonoBehaviour
{
    [SerializeField] List<Sprite> item_list = new List<Sprite>();
    List<GameObject> item_locations = new List<GameObject>();
    [SerializeField] int total_item_number;
    int[] item_coupon_ids = new int[6];
    [SerializeField] Image[] item_coupon_sprites = new Image[6];
    [SerializeField] Image[] item_coupon_frames = new Image[6];
    [SerializeField] Sprite green_frame;
    //[SerializeField] Color dimmed_frame_color;
    [SerializeField] GameObject items_collected_message;
    [SerializeField] GameObject[] escape_zone = new GameObject[2];
    public bool all_items_collected = false;
    public bool item_picked = false;

    private void Awake()
    {
        GameObject[] item_locations_array = GameObject.FindGameObjectsWithTag("Item");
        item_locations = new List<GameObject>(item_locations_array);

        // A check just to make sure there are enough items on the level for the coupon to be full
        if (total_item_number < item_coupon_ids.Length) total_item_number = item_coupon_ids.Length;

        for (int i = 0; i < total_item_number; i++)
        {
            if (item_list.Count < 1 || item_locations.Count < 1)
            {
                Debug.Log("No more items available");
                break;
            }

            int random_location_id = Random.Range(0, item_locations.Count);
            int random_item_id = Random.Range(0, item_list.Count);

            item_locations[random_location_id].GetComponent<SpriteRenderer>().sprite = item_list[random_item_id];
            //item_locations[random_location_id].GetComponent<ItemScript>().item_id = random_item_id;

            // Putting items into item coupon
            if (i < item_coupon_sprites.Length)
            {
                item_coupon_sprites[i].sprite = item_list[random_item_id];
                item_coupon_ids[i] = random_item_id;
            }

            item_list.Remove(item_list[random_item_id]);
            item_locations.Remove(item_locations[random_location_id]);
        }
    }

    private void Start()
    {
        Game.Instance.item_lottery = this;        
    }

    public void ClearLocations()
    {
        GameObject[] all_locations = GameObject.FindGameObjectsWithTag("Item");
        
        for (int i = 0; i < all_locations.Length; i++)
        {
            all_locations[i].SetActive(false);
        }
    }
    public bool AllItemsCollectedCheck()
    {
        for (int i = 0; i < item_coupon_ids.Length; i++)
        {
            if (item_coupon_sprites[i].color.r == 0) return false;
        }

        for (int a = 0; a < escape_zone.Count(); a++)
        {
            escape_zone[a].SetActive(true);
            Game.Instance.robber.Value.GetComponent<RobberScript>().exit_pointer[a].gameObject.SetActive(true);
            GameObject.Find("ObjectPointer").transform.position = escape_zone[1].transform.position;
        }

        all_items_collected = true;
        return true;
    }

    public GameObject GetRandomItem()
    {
        GameObject random_item;
        float while_stop = 0;

        GameObject[] all_items = GameObject.FindGameObjectsWithTag("Item");

        do
        {
            random_item = all_items[Random.Range(0, all_items.Length)];
            while_stop++;


        } while (random_item.GetComponent<SpriteRenderer>().color.r == 1f && while_stop < 5);


        return random_item;
    }

    public void ItemPicked(Sprite item_sprite)
    {
        for (int i = 0; i < item_coupon_ids.Length; i++)
        {
            if (item_coupon_sprites[i].sprite == null) continue;

            else if (item_coupon_sprites[i].sprite == item_sprite)
            {
                item_coupon_sprites[i].color = new Color(1f, 1f, 1f, 1f);
                //item_coupon_frames[i].sprite = green_frame;
                item_coupon_frames[i].color = new Color(1f, 1f, 1f, 1f);
                Game.Instance.robber.Value.GetComponent<RobberScript>().ResetItemRadar();
                // if all the items are collected, it will set correcsponding robber's boolean to true.
                Game.Instance.robber.Value.GetComponent<RobberScript>().items_collected = AllItemsCollectedCheck();
                break;
            }

        }

        item_picked = true;

    }

}