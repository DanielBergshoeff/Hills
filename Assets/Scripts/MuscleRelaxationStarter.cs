using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleRelaxationStarter : MonoBehaviour
{
    public static bool StartOnAwake = false;

    public GameObject MuscleRelaxation;
    public GameObject DutchMuscleRelaxation;

    private void Awake() {
        if (!StartOnAwake) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        GetComponent<VicinityMessage>().StopMessage();
        Destroy(GetComponent<VicinityMessage>());

        if (MenuManager.Dutch)
            DutchMuscleRelaxation.SetActive(true);
        else
            MuscleRelaxation.SetActive(true);

        Destroy(gameObject);
    }
}
