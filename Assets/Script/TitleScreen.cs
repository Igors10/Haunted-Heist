using UnityEngine;

public class TitleScreen : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
