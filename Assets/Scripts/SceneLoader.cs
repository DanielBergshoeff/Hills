using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SceneLoader : MonoBehaviour
{
    public List<GameObject> EnglishMenus;
    public List<GameObject> DutchMenus;

    public GameObject NextScreenButton;
    public GameObject PreviousScreenButton;

    private int currentScreen = 0;

    public void LoadExperience() {
        SceneManager.LoadScene("LizzyTerrain");
    }

    public void SwitchLanguage() {
        MenuManager.Dutch = !MenuManager.Dutch;

        List<GameObject> menusToClear = MenuManager.Dutch ? EnglishMenus : DutchMenus;
        foreach (GameObject go in menusToClear) {
            go.SetActive(false);
        }

        LoadScreen();
    }

    public void NextScreen() {
        currentScreen++;
        LoadScreen();
    }

    public void PreviousScreen() {
        currentScreen--;
        LoadScreen();
    }

    public void LoadScreen() {
        List<GameObject> tempList = MenuManager.Dutch ? DutchMenus : EnglishMenus;

        if(currentScreen >= tempList.Count) {
            LoadExperience();
            return;
        }

        NextScreenButton.SetActive(currentScreen < tempList.Count && currentScreen != 0);
        PreviousScreenButton.SetActive(currentScreen > 0);

        for (int i = 0; i < tempList.Count; i++) {
            if(i == currentScreen) {
                tempList[i].SetActive(true);
            }
            else {
                tempList[i].SetActive(false);
            }
        }
    }
}
