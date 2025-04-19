using UnityEngine;
using TMPro;

public class HiddenMessages : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] hidden_message;
    [SerializeField] int amount_of_messages;
    [SerializeField] string[] message_texts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitMessages();
    }

    void InitMessages()
    {
        // Capping in case bigger then messages in total
        amount_of_messages = (amount_of_messages > hidden_message.Length) ? hidden_message.Length : amount_of_messages;

        // Choosing two random messages and texts
        
        for (int a = 0; a < amount_of_messages; a++)
        {
            int random_message;

            // Picking a random one and making sure it havent been chosen
            do
            {
                random_message = Random.Range(0, hidden_message.Length);
                
            } while (hidden_message[random_message].text != "");

            // Assigning random texts to those random messages
            hidden_message[random_message].text = message_texts[Random.Range(0, message_texts.Length)];
        }
    }
}
