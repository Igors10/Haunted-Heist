using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using System.Diagnostics.CodeAnalysis;

public class RuntimeTutorialManager : MonoBehaviour
{
    public bool Robber;
    public bool Ghost;
    public GameObject overlay;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WipeTutorial();
    }

    void WipeTutorial()
    {
        foreach (var key in TutorialProgress.tutorial_bools.Keys.ToList())
        {
            TutorialProgress.tutorial_bools[key] = false;
        }

    }

    void DeactivateBool(string bool_name)
    {
        TutorialProgress.tutorial_bools[bool_name] = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateOverlay(float x_pos, float y_pos, string text, string type, float priority)
    {
        if (overlay.activeSelf)
        {
            if (overlay.GetComponent<RuntimeOverlayScript>().priority > priority)
            {
                overlay.GetComponent<RuntimeOverlayScript>().textbox.GetComponent<TextMeshPro>().text = text;
                overlay.GetComponent<RuntimeOverlayScript>().type = type;
                overlay.GetComponent<RuntimeOverlayScript>().priority = priority;
            }
            else return;
        }
        else
        {
            overlay.GetComponent<RuntimeOverlayScript>().textbox.GetComponent<TextMeshPro>().text = text;
            overlay.GetComponent<RuntimeOverlayScript>().type = type;
            overlay.GetComponent<RuntimeOverlayScript>().priority = priority;
        }
    }
}
