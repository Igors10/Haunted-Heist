using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectPointer : MonoBehaviour
{
    [SerializeField] Image[] pointers; // 0- left, 1- top, 2- right, 3- bottom
    [SerializeField] Vector3[] pointers_pos_1;
    [SerializeField] Vector3[] pointers_pos_2;
    [SerializeField] float animation_speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    IEnumerator PointerAnim()
    {
        while (pointers[0].enabled)
        {
            yield return new WaitForSeconds(animation_speed);

            for (int a = 0; a < pointers.Length; a++)
            {
                pointers[a].transform.localPosition = (pointers[a].transform.localPosition == pointers_pos_1[a]) 
                    ? pointers[a].transform.localPosition = pointers_pos_2[a] : pointers[a].transform.localPosition = pointers_pos_1[a];
            }
        }
    }

    public void ActivatePointer(Vector3 new_location)
    {
        // Do not trigger the function if playing as ghost
        if (Game.Instance.player == Game.Instance.ghost.Value) return;

        transform.position = new_location;

        for (int a = 0; a < pointers.Length; a++)
        {
            pointers[a].enabled = true;
        }

        StartCoroutine(PointerAnim());
    }

    public void DeactivatePointer()
    {
        for (int a = 0; a < pointers.Length; a++)
        {
            pointers[a].enabled = false;
        }
    }
}
