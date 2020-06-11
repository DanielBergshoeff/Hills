using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Toggle toggleTutorial;
    public Toggle toggleMR;
    public Toggle toggleDutch;

    public void LoadExperience() {
        MenuManager.Tutorial = toggleTutorial.isOn;
        MuscleRelaxationStarter.StartOnAwake = toggleMR.isOn;
        MenuManager.Dutch = !toggleDutch.isOn;
        SceneManager.LoadScene("LizzyTerrain");
    }
}
