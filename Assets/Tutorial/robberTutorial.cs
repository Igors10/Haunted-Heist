using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class robberTutorial : MonoBehaviour
{
    //how many items did the robber get
    public float itemsGathered; 

    //did the player test every direction of movement
    public bool goUp;
    public bool goDown;
    public bool goLeft;
    public bool goRight;

    //did the player use flashlight
    public bool flashlight;

    //did the robber loose a life
    public bool lossOfLife;

    //did the player use vent mechanic
    public bool ventUsed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemsGathered = 0;

        goUp = false;
        goDown = false;
        goLeft = false;
        goRight = false;

        flashlight = false;

        lossOfLife = false;

        ventUsed = false;
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

        float mouseButton = Input.GetAxis("Fire1");

        if (TutorialProgress.part == 2 && mouseButton == 1)
        {
            flashlight = true;
        }



        //part 1
        if (goUp && goDown && goLeft && goRight && TutorialProgress.part == 1)
        {
            TutorialProgress.part = 2;
        }

        //part 2
        if(flashlight && TutorialProgress.part == 2)
        {
            TutorialProgress.part = 3;
        }

        //part 3
        
        if(itemsGathered == 1 && TutorialProgress.part == 3)
        {
            TutorialProgress.part = 4;
        }


        //part 4
        if (itemsGathered == 3 && TutorialProgress.part == 4)
        {
            TutorialProgress.part = 5;
        }

        //part 5
        if (lossOfLife && TutorialProgress.part == 5)
        {
            TutorialProgress.part = 6;
        }

        //part 6
        if(ventUsed && TutorialProgress.part == 6)
        {
            TutorialProgress.part = 7;
        }


    }
}
