using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Breathing : MonoBehaviour
{
    private VisualEffect vfxGraph;

    [SerializeField] private float breatheInTime = 3.0f;
    [SerializeField] private float holdBreathTime = 3.0f;
    [SerializeField] private float breatheOutTime = 3.0f;
    [SerializeField] private float waitForNewBreath = 1.0f;
      
    private float timer = 0f;
    private bool breatheIn = false;

    // Start is called before the first frame update
    void Start()
    {
        vfxGraph = GetComponent<VisualEffect>();
        Invoke("BreatheOut", 0f);
    }

    private void BreatheIn() {
        vfxGraph.SetFloat("Direction", 1f);
        vfxGraph.SetFloat("Speed", 1f / breatheInTime);
        Invoke("HoldBreath", breatheInTime);
    }

    private void HoldBreath() {
        vfxGraph.SetFloat("Speed", 0f);
        Invoke("BreatheOut", holdBreathTime);
    }

    private void BreatheOut() {
        vfxGraph.SetFloat("Direction", 0f);
        vfxGraph.SetFloat("Speed", 1f / breatheOutTime);
        Invoke("WaitForNewBreath", breatheOutTime);
    }

    private void WaitForNewBreath() {
        vfxGraph.SetFloat("Speed", 0f);
        Invoke("BreatheIn", waitForNewBreath);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < breatheOutTime && breatheIn) {
            timer += Time.deltaTime;
            if (timer >= 0f) {
                vfxGraph.SetFloat("Speed", 0f);
            }
        }
    }
}
