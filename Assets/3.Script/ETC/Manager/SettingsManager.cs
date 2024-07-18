using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager
{
    public void LoadSettings()
    {
        AudioManager.instance.LoadSettings();
        // Load other settings here if needed
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}
