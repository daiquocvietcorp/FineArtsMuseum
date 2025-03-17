using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    public SubtitleManager subtitleManager;

    public void SwitchToEnglish()
    {
        subtitleManager.SetLanguage("en");
    }

    public void SwitchToVietnamese()
    {
        subtitleManager.SetLanguage("vi");
    }
}

