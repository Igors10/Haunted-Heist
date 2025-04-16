using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Button next_button;
    [SerializeField] Button previos_button;
    [SerializeField] GameObject tutorial_main;
    [SerializeField] GameObject[] robber_tutorial_pages = new GameObject[3];
    [SerializeField] GameObject[] ghost_tutorial_pages = new GameObject[3];
    int current_robber_tutorial_page = -1;
    int current_ghost_tutorial_page = -1;

    public void EnableTutorial(bool enable)
    {
        tutorial_main.SetActive(enable);
        next_button.gameObject.SetActive(false);
        previos_button.gameObject.SetActive(false);

        if (current_robber_tutorial_page != -1)
        {
            robber_tutorial_pages[current_robber_tutorial_page].SetActive(false);
            current_robber_tutorial_page = -1;
        }

        if (current_ghost_tutorial_page != -1)
        {
            ghost_tutorial_pages[current_ghost_tutorial_page].SetActive(false);
            current_ghost_tutorial_page = -1;
        }

        this.gameObject.SetActive(enable);
    }

    public void OpenTutorialRobber()
    {
        current_robber_tutorial_page = 0;
        robber_tutorial_pages[0].SetActive(true);
        previos_button.gameObject.SetActive(true);
        next_button.gameObject.SetActive(true);
        tutorial_main.SetActive(false);
    }

    public void OpenTutorialGhost()
    {
        current_ghost_tutorial_page = 0;
        ghost_tutorial_pages[0].SetActive(true);
        previos_button.gameObject.SetActive(true);
        next_button.gameObject.SetActive(true);
        tutorial_main.SetActive(false);
    }

    bool NextPageCheck(int current_page, int total_page_count)
    {
        return (current_page < total_page_count - 1);
    }
    public void NextPage()
    {
        // Next page Robber
        if (current_robber_tutorial_page != -1)
        {
            robber_tutorial_pages[current_robber_tutorial_page].SetActive(false);
            current_robber_tutorial_page++;
            robber_tutorial_pages[current_robber_tutorial_page].SetActive(true);

            // Disabling next button if last page
            next_button.gameObject.SetActive(NextPageCheck(current_robber_tutorial_page, robber_tutorial_pages.Length));
        }
        else // Next page Ghost
        {
            ghost_tutorial_pages[current_ghost_tutorial_page].SetActive(false);
            current_ghost_tutorial_page++;
            ghost_tutorial_pages[current_ghost_tutorial_page].SetActive(true);

            // Disabling next button if last page
            next_button.gameObject.SetActive(NextPageCheck(current_ghost_tutorial_page, ghost_tutorial_pages.Length));
        }
    }

    public void PreviosPage()
    {
        if (current_robber_tutorial_page != -1) // Previous page Robber
        {
            if (current_robber_tutorial_page == 0) // Checking to close "how to play robber if first page"
            {
                next_button.gameObject.SetActive(false);
                previos_button.gameObject.SetActive(false);
                robber_tutorial_pages[current_robber_tutorial_page].SetActive(false);
                current_robber_tutorial_page = -1;

                tutorial_main.SetActive(true);
                return;
            }

            // Flipping to previous page
            robber_tutorial_pages[current_robber_tutorial_page].SetActive(false);
            current_robber_tutorial_page--;
            robber_tutorial_pages[current_robber_tutorial_page].SetActive(true);

            // Enabling next button if not last page
            next_button.gameObject.SetActive(NextPageCheck(current_ghost_tutorial_page, ghost_tutorial_pages.Length));
        }
        else // Previous page Ghost
        {
            if (current_ghost_tutorial_page == 0) // Checking to close "how to play ghost if first page"
            {
                next_button.gameObject.SetActive(false);
                previos_button.gameObject.SetActive(false);
                ghost_tutorial_pages[current_ghost_tutorial_page].SetActive(false);
                current_ghost_tutorial_page = -1;

                tutorial_main.SetActive(true);
                return;
            }

            // Flipping to previous page
            ghost_tutorial_pages[current_ghost_tutorial_page].SetActive(false);
            current_ghost_tutorial_page--;
            ghost_tutorial_pages[current_ghost_tutorial_page].SetActive(true);

            // Enabling next button if not last page
            next_button.gameObject.SetActive(NextPageCheck(current_ghost_tutorial_page, ghost_tutorial_pages.Length));
        }
    }


}
