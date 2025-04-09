using UnityEngine;

public class robberDummy : MonoBehaviour
{
    
    private void Start()
    {
        
    }

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
                //increase the killcount and teleport the ghost back
                component.robberKills += 1;

                if(ghost.TryGetComponent(out GhostScript script))
                {
                    StartCoroutine(script.Catch());
                }
                
            }

            Destroy(this.gameObject);
        }
    }
}
