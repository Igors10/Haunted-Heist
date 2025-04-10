using FishNet.Demo.AdditiveScenes;
using System.Globalization;
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

    public bool moveRoom;

    //did the player use flashlight
    public bool flashlight;

    //did the robber sees a Ghost
    public bool seenAGhost;

    //did the player use vent mechanic
    public bool ventUsed;

    //ghost radar functionality
    GameObject ghost;
    RobberScript robberScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemsGathered = 0;

        goUp = false;
        goDown = false;
        goLeft = false;
        goRight = false;

        moveRoom = false;

        flashlight = false;

        seenAGhost = false;

        ventUsed = false;

        
        robberScript = GetComponent<RobberScript>();
    }

    //reworked ghostradar for the tutorial
    void GhostNear()
    {
          ghost = GameObject.FindGameObjectsWithTag("GhostDummy")[0];

          float distance_to_ghost = Vector2.Distance(ghost.transform.position, transform.position);

          // Shaking effect
          Vector2 shake_vector = ShakeEffect(distance_to_ghost);
          robberScript.natural_light.transform.localPosition = new Vector3(shake_vector.x, shake_vector.y, robberScript.natural_light.transform.position.z);
          robberScript.flashlight.transform.localPosition = new Vector3(shake_vector.x, shake_vector.y, robberScript.flashlight.transform.position.z);
          //level_light.transform.localPosition = new Vector3(shake_vector.x, shake_vector.y, level_light.transform.position.z);

          // Growing Audio effect
          AudioSource white_noise = GetComponent<AudioSource>();
          if (distance_to_ghost > robberScript.white_noise_range) white_noise.volume = 0;
          else white_noise.volume = (robberScript.white_noise_range - distance_to_ghost) * robberScript.white_noise_volume / robberScript.white_noise_range;
    }

    Vector2 ShakeEffect(float distance_to_ghost)
    {
        // No shake at all if ghost is too far away
        if (distance_to_ghost > robberScript.radar_range) return Vector2.zero;

        float distance_modifier = (robberScript.radar_range - distance_to_ghost);
        float magnitude = (distance_modifier * robberScript.shake_intensity / 50f);

        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;

        return new Vector2(x, y);

    }

    // Update is called once per frame
    void Update()
    {
        AudioSource white_noise = GetComponent<AudioSource>();

        //checking if the ghost is near
        if (TutorialProgress.part == 5 || TutorialProgress.part == 6) GhostNear();
        else white_noise.volume = 0;

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

        float mouseLeftButton = Input.GetAxis("Fire1");
        float mouseRightButton = Input.GetAxis("Fire2");

        if (TutorialProgress.part == 2 && mouseLeftButton == 1)
        {
            flashlight = true;
        }

        //part 1
        if (moveRoom && TutorialProgress.part == 1)
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
        if (itemsGathered >= 3 && TutorialProgress.part == 4)
        {
            TutorialProgress.part = 5;
        }

        //part 5
        if (seenAGhost && mouseRightButton == 1 && TutorialProgress.part == 5)
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
