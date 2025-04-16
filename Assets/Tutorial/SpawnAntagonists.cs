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


    // Update is called once per frame
    void Update()
    {


        if (TutorialProgress.part == 3 && !firstRobber)
        {

            Instantiate(robberObject, firstInstance.position, Quaternion.identity);
            firstRobber = true;
        }


        if(TutorialProgress.part == 4 && !secondRobber)
        {

            Instantiate(robberObject, secondInstance.position, Quaternion.identity);
            footsteps.SetActive(true);
            secondRobber = true;
        }

        if (TutorialProgress.part == 5 && !thirdRobber)
        {

            Instantiate(robberObject, thirdInstance.position, Quaternion.identity);
            footsteps.SetActive(false);
            footsteps2.SetActive(true);
            thirdRobber = true;
        }

        if(TutorialProgress.part == 6)
        {
            footsteps2.SetActive(false);
            //ghost wins
        }
    }
}
