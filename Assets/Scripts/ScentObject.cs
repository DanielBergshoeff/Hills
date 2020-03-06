using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensiks.SDK.Shared.SensiksDataTypes;

public class ScentObject : MonoBehaviour
{
    public float Strength;
    public Scent ObjectScent;
    public float MaxDistance;

    private bool scentOn = false;

    // Start is called before the first frame update
    void Start()
    {
        if (ScentManager.scentObjects == null)
            ScentManager.scentObjects = new List<ScentObject>();

        ScentManager.scentObjects.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player"))
            return;

        scentOn = true;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player"))
            return;

        scentOn = false;
    }
}
