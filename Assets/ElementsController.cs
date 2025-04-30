using UnityEngine;

public class ElementsController : MonoBehaviour
{

    [SerializeField] private GameObject createServerButton;
    [SerializeField] private GameObject inputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If T is presssed disable the create server button, enable it if pressed again

        if (Input.GetKeyDown(KeyCode.T))
        {
            createServerButton.SetActive(!createServerButton.activeSelf);
        }

        // If Y is pressed disable the input field

        if (Input.GetKeyDown(KeyCode.Y))
        {
            inputField.SetActive(!inputField.activeSelf);
        }
    }
}
