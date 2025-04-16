using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;

public class RobberUI : MonoBehaviour
{
    [SerializeField] GameObject item_coupon;
    [SerializeField] GameObject energy_bar;

    // Maybe have a low energy mode later
    [SerializeField] Color full_energy_color;
    [SerializeField] Color low_energy_color;
    [SerializeField] Image energy_bar_sprite;

    float current_energy;
    bool using_energy = false;
    [SerializeField] float burst_decrease;
    [SerializeField] float energy_decrease;
    [SerializeField] float energy_regeneration;
    //[SerializeField] float use_threshold;
    bool recharging = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_energy = 1f;
        energy_bar_sprite.fillAmount = current_energy;
        energy_bar_sprite.color = full_energy_color;
    }

    public void EnableUI()
    {
        item_coupon.SetActive(true);
        energy_bar.SetActive(true);
    }

    private void FixedUpdate()
    {
        EnergyManagement();
    }

    void RechargeMode(bool is_recharge)
    {
        recharging = is_recharge;

        energy_bar_sprite.color = (recharging) ? low_energy_color : full_energy_color;
    }

    void EnergyManagement()
    {
        if (current_energy >= 1f && recharging) RechargeMode(false);
        if (using_energy)
        {
            current_energy -= energy_decrease;
            if (current_energy < 0)
            {
                current_energy = 0;
                Game.Instance.robber.Value.GetComponent<RobberScript>().NightVision(false); // turning night vision off if energy is at 0
                RechargeMode(true);
            }
            /*
            else if (current_energy < use_threshold && energy_bar_sprite.color == full_energy_color)
            {
                energy_bar_sprite.color = low_energy_color;
                Debug.Log("low energy level");
            }*/
        }
        else
        {
            current_energy += energy_regeneration;
            if (current_energy >= 1f)
            {
                current_energy = 1f;
            }
            //else if (current_energy > use_threshold && energy_bar_sprite.color == low_energy_color) energy_bar_sprite.color = full_energy_color;
        }

        energy_bar_sprite.fillAmount = current_energy;
    }

    public bool UseEnergy(bool is_using)
    {
        if (recharging)
        {
            using_energy = false;
            return false;
        }

        using_energy = is_using;

        if (is_using) current_energy -= burst_decrease;
        
        // it returns false if you cant use night vision;
        return true;
    }

}
