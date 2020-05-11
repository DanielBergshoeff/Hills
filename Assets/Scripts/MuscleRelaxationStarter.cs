using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleRelaxationStarter : MonoBehaviour
{
    public static bool StartOnAwake = true;

    public GameObject MuscleRelaxation;

    private void Awake() {
        if (!StartOnAwake) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        GetComponent<VicinityMessage>().StopMessage();
        Destroy(GetComponent<VicinityMessage>());
        MuscleRelaxation.SetActive(true);
        Destroy(gameObject);
    }
}
