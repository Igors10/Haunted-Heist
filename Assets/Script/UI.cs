using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject[] elements_to_enable = new GameObject[0];

    public void EnableUI()
    {
        for (int i = 0; i < elements_to_enable.Length; i++)
        {
            elements_to_enable[i].SetActive(true);
        }
    }
}
