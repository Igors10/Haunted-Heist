using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GhostUI : MonoBehaviour
{
    [SerializeField] GameObject elements_UI;
    [SerializeField] GameObject timer;

    public Image dash_fill;
    public Image stepvision_fill;
    public GameObject ghostVision;
    [SerializeField] Color charged_color;
    [SerializeField] Color cooldown_color;
    [HideInInspector] public bool is_dash_ready = true;
    [HideInInspector] public bool is_stepvision_ready = true;
    public IEnumerator Cooldown(float cooldown_time, bool is_dash)
    {
        Image ability_fill = (is_dash) ? dash_fill : stepvision_fill;

        float cooldown_left = 0;
        if (is_dash) is_dash_ready = false;
        else is_stepvision_ready = false;
        ability_fill.color = cooldown_color;
        
        while (cooldown_left < cooldown_time)
        {
            cooldown_left++;
            ability_fill.fillAmount = cooldown_left / cooldown_time;
            yield return new WaitForSeconds(0.1f);
        }

        ability_fill.color = charged_color;
        if (is_dash) is_dash_ready = true;
        else is_stepvision_ready = true;
        ability_fill.fillAmount = 1;
    }

    public void EnableUI()
    {
        elements_UI.SetActive(true);
        timer.SetActive(true);
        //dash_fill.gameObject.SetActive(true);
        //stepvision_fill.gameObject.SetActive(true);
        ghostVision.SetActive(true);
    }
}
