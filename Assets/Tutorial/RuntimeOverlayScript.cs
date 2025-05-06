using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeOverlayScript : MonoBehaviour
{
    public GameObject textbox;
    public string given_text;
    public Vector3 changed_position;
    public string type;
    public float priority;

    public void Activate()
    {
        transform.position = changed_position;
        textbox.GetComponent<Text>().text = given_text;
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
    }
}
