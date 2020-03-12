using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField] private float timeForShake;
    [SerializeField] private GameObject flowerBudPrefab;

    private VRTK.VRTK_InteractableObject vrtkObject;
    private bool shakeAllowed = true;

    // Start is called before the first frame update
    void Start()
    {
        vrtkObject = GetComponent<VRTK.VRTK_InteractableObject>();

        if (ShakeDetection.shakeEvent == null)
            ShakeDetection.shakeEvent = new ShakeEvent();

        ShakeDetection.shakeEvent.AddListener(Shaken);
    }

    private void EnableShaking() {
        shakeAllowed = true;
    }

    private void Shaken(GameObject go) {
        if (!shakeAllowed || !vrtkObject.IsGrabbed(go))
            return;

        shakeAllowed = false;


        SpawnFlowerBud();
        Invoke("EnableShaking", timeForShake);
    }

    private void SpawnFlowerBud() {
        GameObject flowerBud = Instantiate(flowerBudPrefab);
        flowerBud.transform.position = transform.position;
    }
}
