using UnityEngine;

public class robberDummy : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    //collision 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ghost"))
        {
            GameObject ghost = collision.gameObject.transform.parent.gameObject;

            if (ghost.TryGetComponent(out ghostTutorial component))
            {
                component.robberKills += 1;
            }

            Destroy(this.gameObject);
        }
    }
}
