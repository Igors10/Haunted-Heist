using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class OverlayScript : MonoBehaviour
{
    public bool tutorial_part_1;
    public bool tutorial_part_2;
    public bool tutorial_part_3;
    public bool tutorial_part_4;
    public bool tutorial_part_5;
    public bool tutorial_part_6;
    public bool tutorial_part_7;

    public bool show = true;

    void Start()
    {

    }

    void Update()
    {
        
        Deinitialise();
    }

    void Deinitialise()
    {
        if (tutorial_part_1 && TutorialProgress.part == 2) { show = false; }
        else if (tutorial_part_2 && TutorialProgress.part == 3) { show = false; }
        else if (tutorial_part_3 && TutorialProgress.part == 4) { show = false; }
        else if (tutorial_part_4 && TutorialProgress.part == 5) { show = false; }
        else if (tutorial_part_5 && TutorialProgress.part == 6) { show = false; }
        else if (tutorial_part_6 && TutorialProgress.part == 7) { show = false; }
        else if (tutorial_part_7 && TutorialProgress.part == 8) { show = false; }


        if (!show) { gameObject.SetActive(false); }
    }


}
