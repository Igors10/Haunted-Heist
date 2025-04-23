using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    //ui elements hooked from the scene manualy
    public GameObject LeftButtonAction;
    public GameObject RightButtonAction;
    public GameObject Life;
    public GameObject ItemCoupon;
    public GameObject Timer;

    public bool ghost;
    public bool robber;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeftButtonAction.SetActive(false);
        RightButtonAction.SetActive(false);
        Life.SetActive(false);
        ItemCoupon.SetActive(false);
        Timer.SetActive(false);
}

    // Update is called once per frame
    void Update()
    {
        if(TutorialProgress.part == 1)
        {
            Life.SetActive(true);
            //Timer.SetActive(true);
        }

        if (TutorialProgress.part == 2)
        {
            LeftButtonAction.SetActive(true);

            if(robber) ItemCoupon.SetActive(true);
        }

        if (TutorialProgress.part == 3)
        {
            ItemCoupon.SetActive(true);
        }

        if (TutorialProgress.part == 4)
        {
            if(ghost) RightButtonAction.SetActive(true);
        }

        if (TutorialProgress.part == 5)
        {
            if (ghost) ItemCoupon.SetActive(true);
            if (robber) RightButtonAction.SetActive(true);
        }

        if (TutorialProgress.part == 6)
        {

        }
    }
}
