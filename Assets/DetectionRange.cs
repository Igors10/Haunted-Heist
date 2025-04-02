using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    public Item parentItem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            parentItem.OnRobberEnterRange(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            parentItem.OnRobberStayInRange(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Robber")
        {
            parentItem.OnRobberExitRange(collision);
        }
    }
}