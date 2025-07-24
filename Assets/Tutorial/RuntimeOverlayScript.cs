using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static System.Net.Mime.MediaTypeNames;

public class RuntimeOverlayScript : MonoBehaviour
{
    public TextMeshProUGUI textbox;
    public AnimationScript smoke_animation;
    public string given_text;
    public string approrpiate_text; //text that switches the {key} to the appropriate key
    public Vector3 changed_position;
    public string type;
    public float priority = 10f;

    public float lastActivatedTime = -999f;
    public float minIntervalBetweenActivations = 0.5f; // half a second

    public bool CanActivate()
    {
        return Time.time - lastActivatedTime > minIntervalBetweenActivations;
    }

    public void Activate(string new_text, string new_type, float new_priority)
    {
        type = new_type;
        priority = new_priority;

        if (type == "robber_movement" || type == "ghost_movement")
        {
            if (GameData.is_gamepad_used)
            {
                approrpiate_text = "Left Stick";
            }
            else
            {
                approrpiate_text = "W, A, S, D";
            }
        }

        if(type == "robber_lantern" || type == "ghost_dash")
        {
            if (GameData.is_gamepad_used)
            {
                approrpiate_text = "(A)";
            }
            else
            {
                approrpiate_text = "Left Mouse Button";
            }
        }

        if (type == "robber_radar" || type == "ghost_stepvision")
        {
            if (GameData.is_gamepad_used)
            {
                approrpiate_text = "(B)";
            }
            else
            {
                approrpiate_text = "Righ Mouse Button";
            }
        }

        if (type == "robber_vent")
        {
            if (GameData.is_gamepad_used)
            {
                approrpiate_text = "LB <color=white>or</color> RB";
            }
            else
            {
                approrpiate_text = "Q <color=white>or</color> E";
            }
        }

        if (type == "robber_pickup")
        {
            if (GameData.is_gamepad_used)
            {
                approrpiate_text = "RT";
            }
            else
            {
                approrpiate_text = "spacebar";
            }
        }

        //fill the text with the appropriate control
        new_text = new_text.Replace("{key}", approrpiate_text);


        //transform.position = changed_position;   It appears in the center of the screen
        textbox.text = new_text;

        

        Debug.Log($"Overlay activated: {type}, priority: {priority}");
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialProgress.tutorial_bools[type])
        {
            Delete();
            Debug.Log($"Overlay deleted: {type}");
        }
    }

    public void Delete()
    {
        this.gameObject.SetActive(false);         
        priority = 10f;
    }
}
