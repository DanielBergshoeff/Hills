using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensiks.SDK.Shared.SensiksDataTypes;

public class ScentObject : MonoBehaviour
{
    public float Strength;
    public Scent ObjectScent;
    public float MaxDistance;

    // Start is called before the first frame update
    void Start()
    {
        if (ScentManager.scentObjects == null)
            ScentManager.scentObjects = new List<ScentObject>();

        ScentManager.scentObjects.Add(this);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
    }
}
