using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnableTroubleshooting : MonoBehaviour
{
    public Light2D TroubleshootingFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TroubleshootingFilter.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
