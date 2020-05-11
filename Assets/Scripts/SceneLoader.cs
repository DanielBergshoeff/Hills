﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Toggle toggleTutorial;
    public Toggle toggleMR;

    public void LoadExperience() {
        MenuManager.Tutorial = toggleTutorial.isOn;
        MuscleRelaxationStarter.StartOnAwake = toggleMR;
        SceneManager.LoadScene("LizzyTerrain");
    }
}
