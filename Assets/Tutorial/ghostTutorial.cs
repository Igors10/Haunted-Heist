using UnityEngine;
using System.Collections;

public class ghostTutorial : MonoBehaviour
{
    //how many times the ghost has caugth the robber
    public float robberKills;

    //did the player test every direction of movement
    public bool goUp;
    public bool goDown;
    public bool goLeft;
    public bool goRight;

    public bool moveRoom;

    //did the player use step vision
    public bool stepVision;

    //accessed the closed room
    public bool throughWall;
    public bool finishSteps;

    GhostScript ghostScript;
    public GameObject spawner;
    public SpawnAntagonists spawnAntagonists;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // If scene is Lvl_Tilemap then disable the whole script

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Lvl_Tilemap")
        {
            this.enabled = false;
            return;
        }
    }

    void Start()
    {
        robberKills = 0;

        goUp = false;
        goDown = false;
        goLeft = false;
        goRight = false;

        moveRoom = false;

        stepVision = false;

        throughWall = false;
        finishSteps = false;

        spawner = GameObject.FindWithTag("spawnPointTutorial");
        ghostScript = GetComponent<GhostScript>();
        spawnAntagonists = spawner.GetComponent<SpawnAntagonists>();
    }

    IEnumerator DelayChange(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        TutorialProgress.part = 5;

        // Attach the second robberDummy as a target of the flashlight
        spawnAntagonists.SpawnSecondRobber();

        if (spawnAntagonists.secondRobberInstance != null)
        {
            ghostScript.player.indication.target = spawnAntagonists.secondRobberInstance.transform.position;
            ghostScript.player.Indication(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("tutorial part: " + TutorialProgress.part);

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //tracking the progress of the part of the tutorial
        if (!goUp || !goDown || !goLeft || !goRight)
        {
            if (x > 0)
            {
                goRight = true;
            }

            if (y > 0)
            {
                goUp = true;
            }

            if (y < 0)
            {
                goDown = true;
            }

            if (x < 0)
            {
                goLeft = true;
            }
        }


        //part 1
        if (moveRoom && TutorialProgress.part == 1)
        {
            TutorialProgress.part = 2;
        }

        //part 2

        if (throughWall && TutorialProgress.part == 2)
        {
            TutorialProgress.part = 3;

            // Attach the first robberDummy as a target of the flashlight
            spawnAntagonists.SpawnFirstRobber();


            if (spawnAntagonists.firstRobberInstance != null)
            {
                ghostScript.player.indication.target = spawnAntagonists.firstRobberInstance.transform.position;
                ghostScript.player.Indication(true);
            }

        }


        //part 3
        if (robberKills == 1 && TutorialProgress.part == 3)
        {
            TutorialProgress.part = 4;
            ghostScript.player.Indication(false);
        }

        //part 4
        if (finishSteps && TutorialProgress.part == 4)
        {
            DelayChange(3f);
        }

        //part 5
        if (robberKills == 2 && TutorialProgress.part == 5)
        {
            TutorialProgress.part = 6;

            spawnAntagonists.SpawnThirdRobber();

            // Attach the third robberDummy as a target of the flashlight
            if (spawnAntagonists.secondRobberInstance != null)
            {
                ghostScript.player.indication.target = spawnAntagonists.secondRobberInstance.transform.position;
                ghostScript.player.Indication(true);
            }

        }

        //part 6
        if (robberKills == 3 && TutorialProgress.part == 6)
        {
            TutorialProgress.part = 7;
            ghostScript.player.Indication(false);

            this.gameObject.GetComponent<Player>().GameOverServerRpc(true);
        }

    }
}
