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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = changed_position;
        textbox.GetComponent<Text>().text = given_text;
    }

    // Update is called once per frame
    void Update()
    {
        //it ends when their bool is deactivated

        //if (//the exact bool becomes inactive) Delete();
    }

    public void Delete()
    {
        this.gameObject.SetActive(false);
    }
}
