using UnityEngine;

public class ElementsController : MonoBehaviour
{

    [SerializeField] private GameObject createServerButton;
    [SerializeField] private GameObject inputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createServerButton.SetActive(!GameData.disableCreateServerButton);
        inputField.SetActive(!GameData.disableInputField);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameData.disableCreateServerButton = !GameData.disableCreateServerButton;
            createServerButton.SetActive(!GameData.disableCreateServerButton);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            GameData.disableInputField = !GameData.disableInputField;
            inputField.SetActive(!GameData.disableInputField);
        }
    }

}
