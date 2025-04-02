using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ClosedDoorTutorialScript : MonoBehaviour
{
    public bool tutorial_part_1;
    public bool tutorial_part_2;
    public bool tutorial_part_3;
    public bool tutorial_part_4;
    public bool tutorial_part_5;
    public bool tutorial_part_6;
    public bool tutorial_part_7;

    private bool open = false;

    void Update()
    {
        
        Deinitialise();
    }

    void Deinitialise()
    {
        if (tutorial_part_1 && TutorialProgress.part == 1)  { open = true; }
        else if (tutorial_part_2 && TutorialProgress.part == 2) { open = true; }
        else if (tutorial_part_3 && TutorialProgress.part == 3) { open = true; }
        else if (tutorial_part_4 && TutorialProgress.part == 4) { open = true; }
        else if (tutorial_part_5 && TutorialProgress.part == 5) { open = true; }
        else if (tutorial_part_6 && TutorialProgress.part == 6) { open = true; }
        else if (tutorial_part_7 && TutorialProgress.part == 7) { open = true; }


        if (open)
        {
            Destroy(this.gameObject);
        }
    }


}
