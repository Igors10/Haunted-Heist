using UnityEngine;

public class RoomCheckScript : MonoBehaviour
{
    public bool tutorial_part_1;
    public bool tutorial_part_2;
    public bool tutorial_part_3;
    public bool tutorial_part_4;
    public bool tutorial_part_5;
    public bool tutorial_part_6;
    public bool tutorial_part_7;

    public bool ghostRoom;
    public bool ghostVent;
    public bool robberVent;
    public bool robberSeeGhost;

    bool kill = false;

    // Update is called once per frame
    void Update()
    {
        Deinitialise();
    }

    void Deinitialise()
    {
        if (tutorial_part_1 && TutorialProgress.part == 2) { kill = true; }
        else if (tutorial_part_2 && TutorialProgress.part == 3) { kill = true; }
        else if (tutorial_part_3 && TutorialProgress.part == 4) { kill = true; }
        else if (tutorial_part_4 && TutorialProgress.part == 5) { kill = true; }
        else if (tutorial_part_5 && TutorialProgress.part == 6) { kill = true; }
        else if (tutorial_part_6 && TutorialProgress.part == 7) { kill = true; }


        if (kill) { gameObject.SetActive(false); }
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Ghost") && ghostRoom)
        {
            GameObject ghost = collider.gameObject.transform.parent.gameObject;

            //checking the position of the ghost in the tutorial
            if (ghost.TryGetComponent(out ghostTutorial component))
            {
                component.throughWall = true;
            }
        }

        if (collider.gameObject.CompareTag("Ghost") && ghostVent)
        {
            GameObject ghost = collider.gameObject.transform.parent.gameObject;

            //checking the position of the ghost in the tutorial
            if (ghost.TryGetComponent(out ghostTutorial component))
            {
                component.finishSteps = true;
            }
        }

        if (collider.gameObject.CompareTag("Robber") && robberVent)
        {
            //checking the position of the robber in the tutorial
            if (collider.gameObject.TryGetComponent(out robberTutorial component))
            {
                component.ventUsed = true;
            }
        }

        if (collider.gameObject.CompareTag("Robber") && robberSeeGhost)
        {
            //checking the position of the robber in the tutorial
            if (collider.gameObject.TryGetComponent(out robberTutorial component))
            {
                component.seenAGhost = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Robber") && robberSeeGhost)
        {
            //checking the position of the robber in the tutorial
            if (collider.gameObject.TryGetComponent(out robberTutorial component))
            {
                component.seenAGhost = false;
            }
        }
    }

}
