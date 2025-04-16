using UnityEngine;

public class ghostTutorial : MonoBehaviour
{
    //how many times the ghost has caugth the robber
    public float robberKills;

    //did the player test every direction of movement
    public bool goUp;
    public bool goDown;
    public bool goLeft;
    public bool goRight;

    //did the player use step vision
    public bool stepVision;

    //accessed the closed room
    public bool throughWall;
    public bool finishSteps;

    //teleportation back for the ghost
    public Transform ghostTeleportBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robberKills = 0;

        goUp = false;
        goDown = false;
        goLeft = false;
        goRight = false;

        stepVision = false;

        throughWall = false;
        finishSteps = false;

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
        if (goUp && goDown && goLeft && goRight && TutorialProgress.part == 1)
        {
            TutorialProgress.part = 2;
        }

        //part 2

        if (throughWall && TutorialProgress.part == 2)
        {
            TutorialProgress.part = 3;
        }


        //part 3
        if (robberKills == 1 && TutorialProgress.part == 3)
        {
            TutorialProgress.part = 4;
            this.gameObject.transform.position = ghostTeleportBack.GetChild(0).position;
        }

        //part 4
        if (finishSteps && TutorialProgress.part == 4)
        {
            TutorialProgress.part = 5;
        }

        //part 5
        if (robberKills == 2 && TutorialProgress.part == 5)
        {
            TutorialProgress.part = 6;
            this.gameObject.transform.position = ghostTeleportBack.GetChild(0).position;
        }

        //part 6
        if (robberKills == 3 && TutorialProgress.part == 6)
        {
            TutorialProgress.part = 7;
            this.gameObject.transform.position = ghostTeleportBack.GetChild(0).position;
        }

    }
}
