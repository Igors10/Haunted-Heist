using UnityEngine;

public class SpawnGhostAntagonists : MonoBehaviour
{
    //objects to spawn
    public GameObject ghostObject;


    //places to spawn robber
    public Transform firstInstance;
    public Transform secondInstance;

    //endzones spawn
    public GameObject endZones;

    bool ghostSpawned = false;
    bool ghostSpawned2 = false;


    // Update is called once per frame
    void Update()
    {
        if (TutorialProgress.part == 5 && !ghostSpawned)
        {
            Instantiate(ghostObject, firstInstance.position, Quaternion.identity);
            ghostSpawned = true;
        }

        if (TutorialProgress.part == 6 && !ghostSpawned2)
        {
            //Instantiate(ghostObject, secondInstance.position, Quaternion.identity);
            //ghostSpawned2 = true;
            //ghostObject.GetComponent<Collider>().isTrigger = false;
        }

        if (TutorialProgress.part == 7)
        {
            endZones.SetActive(true);
        }

    }
}
