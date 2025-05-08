using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuntimeOverlayScript : MonoBehaviour
{
    public TextMeshProUGUI textbox;
    public AnimationScript smoke_animation;
    public string given_text;
    public Vector3 changed_position;
    public string type;
    public float priority = 10f;

    public void Activate(string new_text)
    {
        //transform.position = changed_position;   It appears in the center of the screen
        textbox.text = new_text;
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialProgress.tutorial_bools[type])
        {
            Delete();
        }
    }

    public void Delete()
    {
        this.gameObject.SetActive(false);         
        priority = 10f;
    }
}
