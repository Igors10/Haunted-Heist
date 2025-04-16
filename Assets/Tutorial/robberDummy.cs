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
            collision.gameObject.transform.parent.TryGetComponent(out GhostScript ghostScript);

            if (ghostScript.is_dashing)
            {

                GameObject ghost = collision.gameObject.transform.parent.gameObject;

                if (ghost.TryGetComponent(out ghostTutorial component))
                {
                    //increase the killcount and teleport the ghost back
                    component.robberKills += 1;
                    StartCoroutine(ghostScript.Catch());
                    

                }

                Destroy(this.gameObject);
            }
        }
    }
}
