using UnityEngine;

public class robberDummy : MonoBehaviour
{
    public Transform teleportSpot;

    private void Start()
    {
        teleportSpot = GameObject.FindGameObjectWithTag("spawnPointTutorial").GetComponent<Transform>();
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
                ghost.transform.position = teleportSpot.position;
            }

            Destroy(this.gameObject);
        }
    }
}
