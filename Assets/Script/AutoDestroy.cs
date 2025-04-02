using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Called by the animation event
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
