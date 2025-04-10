using UnityEngine;

public class SpawnAntagonists : MonoBehaviour
{
    //objects to spawn
    public GameObject robberObject;
    public GameObject footsteps;
    public GameObject footsteps2;

    //places to spawn robber
    public Transform firstInstance;
    public Transform secondInstance;  
    public Transform thirdInstance;

    bool firstRobber = false;
    bool secondRobber = false;
    bool thirdRobber = false;

    public robberDummy firstRobberInstance;
    public robberDummy secondRobberInstance;
    public robberDummy thirdRobberInstance;

    // Update is called once per frame
    void Update()
    {


        if(TutorialProgress.part == 4 && !secondRobber)
        {

            GameObject robber = Instantiate(robberObject, secondInstance.position, Quaternion.identity);
            secondRobberInstance = robber.GetComponent<robberDummy>(); // Store the reference
            footsteps.SetActive(true);
            secondRobber = true;
        }

        if (TutorialProgress.part == 5)
        {
            footsteps.SetActive(false);
        }

    }

    public void SpawnFirstRobber()
    {

        GameObject robber = Instantiate(robberObject, firstInstance.position, Quaternion.identity);
        firstRobberInstance = robber.GetComponent<robberDummy>(); // Store the reference
        Debug.Log("RF:" + firstRobberInstance);
        firstRobber = true;
    }

    public void SpawnThirdRobber()
    {

        GameObject robber = Instantiate(robberObject, thirdInstance.position, Quaternion.identity);
        thirdRobberInstance = robber.GetComponent<robberDummy>(); // Store the reference
        footsteps2.SetActive(true);
        thirdRobber = true;
    }

}
