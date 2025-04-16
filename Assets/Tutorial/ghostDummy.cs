using System.Collections;
using UnityEngine;

public class ghostDummy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Despawn(int time)
    {

        yield return new WaitForSeconds(time);

        this.gameObject.SetActive(false);
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Robber"))
        {
            if (collision.gameObject.TryGetComponent(out robberTutorial component))
            {
                component.lossOfLife = true;
            }

            this.gameObject.SetActive(false);
            Despawn(2);
        }
    }
}
