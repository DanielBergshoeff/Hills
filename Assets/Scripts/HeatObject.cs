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
        if (HeatManager.HeatObjects == null)
            HeatManager.HeatObjects = new List<HeatObject>();

        HeatManager.HeatObjects.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
