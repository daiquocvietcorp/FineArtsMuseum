using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public static class XRHelper
{
    /// <summary>Ngắt Select + Hover giữa interactor và interactable (nếu đang có)</summary>
    public static void ForceClear(XRBaseInteractor interactor,
        IXRInteractable interactable,
        XRInteractionManager manager)
    {
        if (interactor == null || interactable == null || manager == null) return;

        /* --- CLEAR SELECT --- */
        var selInteractor   = interactor   as IXRSelectInteractor;
        var selInteractable = interactable as IXRSelectInteractable;

        if (selInteractor != null && selInteractable != null &&
            selInteractor.IsSelecting(selInteractable))
        {
            manager.SelectExit(selInteractor, selInteractable);
        }

        /* --- CLEAR HOVER --- */
        var hovInteractor   = interactor   as IXRHoverInteractor;
        var hovInteractable = interactable as IXRHoverInteractable;

        if (hovInteractor != null && hovInteractable != null &&
            hovInteractor.IsHovering(hovInteractable))
        {
            manager.HoverExit(hovInteractor, hovInteractable);
        }
    }
}