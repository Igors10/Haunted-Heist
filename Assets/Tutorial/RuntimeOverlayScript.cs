using System.Linq.Expressions;
using UnityEngine;

public class RuntimeOverlayScript : MonoBehaviour
{
    public GameObject textbox;

    public string type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Delete()
    {
        this.gameObject.SetActive(false);
    }
}
