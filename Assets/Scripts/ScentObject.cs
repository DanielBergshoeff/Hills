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
        if (ScentManager.ScentObjects == null)
            ScentManager.ScentObjects = new List<ScentObject>();

        ScentManager.ScentObjects.Add(this);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
    }
}
