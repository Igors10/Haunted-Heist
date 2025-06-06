using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] GameObject[] miasma;
    [SerializeField] GameObject[] miasma_border;
    [SerializeField] float miasma_speed;
   
    private void FixedUpdate()
    {
        MiasmaUpdate();
    }
    void MiasmaUpdate() // Background miasma moving
    {
        if (miasma[0] == null) return;

        for (int a = 0; a < miasma.Length; a++)
        {
            if (miasma[a].transform.position.x > miasma_border[1].transform.position.x) miasma[a].transform.position = miasma_border[0].transform.position;
            miasma[a].transform.Translate(miasma_speed, 0, 0);

        }
    }
}
