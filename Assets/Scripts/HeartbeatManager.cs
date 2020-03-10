using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensiks.SDK.Shared.SensiksDataTypes;
using Sensiks.SDK.UnityLibrary;

public class HeartbeatManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(SensiksManager.Instance.CurrentPulseSensorIbiValue);
    }
}
