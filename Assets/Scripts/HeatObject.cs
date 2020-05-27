using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatObject : MonoBehaviour
{
    public float Heat = 1f;
    public float Range = 10f;
    public bool PositionBased = false;

    // Start is called before the first frame update
    void Start()
    {
        HeatManager.AddHeatObject(this);
    }

    private void OnDestroy() {
        HeatManager.HeatObjects.Remove(this);
    }
}
