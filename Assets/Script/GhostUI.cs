using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GhostUI : MonoBehaviour
{
    [SerializeField] GameObject elements_UI;
    [SerializeField] GameObject timer;
    public HealthBar souls; 

    public Image dash_fill;
    public Image stepvision_fill;
    [SerializeField] Image[] dash_icons;
    public Image[] stepvision_icons;
    public GameObject ghostVision;
    [SerializeField] Color charged_color_dash;
    [SerializeField] Color charged_color_stepvision;
    [SerializeField] Color cooldown_color;
    [HideInInspector] public bool is_dash_ready = true;
    [HideInInspector] public bool is_stepvision_ready = true;
    public IEnumerator Cooldown(float cooldown_time, bool is_dash)
    {
        Image ability_fill = (is_dash) ? dash_fill : stepvision_fill;
        Image[] icons_to_dim = (is_dash) ? dash_icons : stepvision_icons;

        float cooldown_left = 0;
        if (is_dash) is_dash_ready = false;
        else is_stepvision_ready = false;
        ability_fill.color = cooldown_color;

        DimAbilityIcons(icons_to_dim, true);
        
        while (cooldown_left < cooldown_time)
        {
            cooldown_left++;
            ability_fill.fillAmount = cooldown_left / cooldown_time;
            yield return new WaitForSeconds(0.1f);
        }

        ability_fill.color = (is_dash) ? charged_color_dash : charged_color_stepvision;
        if (is_dash) is_dash_ready = true;
        else is_stepvision_ready = true;
        ability_fill.fillAmount = 1;

        DimAbilityIcons(icons_to_dim, false);
    }

    void DimAbilityIcons(Image[] icons_to_dim, bool to_dim)
    {
        float alpha = (to_dim) ? 0.2f : 1f;


        for (int a = 0; a < icons_to_dim.Length; a++)
        {
            // make it become less saturated when on 
            icons_to_dim[a].color = new Color(1f, 1f, 1f, alpha);
        }

    }

    public void EnableUI()
    {
        elements_UI.SetActive(true);
        timer.SetActive(true);
        ghostVision.SetActive(true);
    }
}
