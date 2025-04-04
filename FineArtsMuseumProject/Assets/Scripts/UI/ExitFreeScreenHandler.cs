using UnityEngine;

public class ExitFreeScreenHandler : MonoBehaviour
{
    public IdleScreenManager manager;

    public void OnFadeOutComplete()
    {
        if (manager != null)
        {
            manager.OnFadeOutComplete();
        }
    }
}