using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flower : MonoBehaviour
{
    [SerializeField] private float timeForShake;
    [SerializeField] private GameObject flowerBudPrefab;

    [ColorUsageAttribute(true, true)]
    public Color MyColor;


    private VRTK.VRTK_InteractableObject vrtkObject;
    private bool shakeAllowed = true;

    private VisualEffect myGraph;

    // Start is called before the first frame update
    void Start()
    {
        vrtkObject = GetComponent<VRTK.VRTK_InteractableObject>();
        myGraph = GetComponentInChildren<VisualEffect>();
        myGraph.SetVector4("Color", MyColor);

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
        FlowerBud fb = flowerBud.GetComponent<FlowerBud>();
        fb.MyColor = MyColor;
        flowerBud.transform.position = transform.position;
    }
}
