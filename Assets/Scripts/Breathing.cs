﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Breathing : MonoBehaviour
{
    private VisualEffect vfxGraph;
    public VisualEffect Tree;

    [SerializeField] private float breatheInTime = 3.0f;
    [SerializeField] private float holdBreathTime = 3.0f;
    [SerializeField] private float breatheOutTime = 3.0f;
    [SerializeField] private float waitForNewBreath = 1.0f;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.resetSeedOnPlay = true;
        vfxGraph.Reinit();

        vfxGraph.SetFloat("Direction", 1f);
        vfxGraph.SetFloat("Speed", 1f / breatheInTime);
        //CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInTime, Vector3.up * 2f, 0.5f, false);
        //Invoke("BreatheIn", 0f);
    }

    private void Update() {
        if (timer < breatheInTime && timer + Time.deltaTime >= breatheInTime) {
            CommunicationManager.Instance.DisplayMessage(this.gameObject, "Hold breath", holdBreathTime, Vector3.up * 2f, 0.5f, false);
            vfxGraph.SetFloat("Speed", 0f);
        }
        else if (timer < breatheInTime + holdBreathTime && timer + Time.deltaTime >= breatheInTime + holdBreathTime) {
            CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe out", breatheOutTime, Vector3.up * 2f, 0.5f, false);
            vfxGraph.SetFloat("Direction", -1f);
            vfxGraph.SetFloat("Speed", 1f / breatheOutTime);
            Tree.SetFloat("Direction", 1f);
        }
        else if(timer < breatheInTime + holdBreathTime + breatheOutTime && timer + Time.deltaTime >= breatheInTime + holdBreathTime + breatheOutTime) {
            CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInTime, Vector3.up * 2f, 0.5f, false);
            vfxGraph.SetFloat("Direction", 1f);
            vfxGraph.SetFloat("Speed", 1f / breatheInTime);
            Tree.SetFloat("Direction", 0f);
            timer = 0f;
        }

        timer += Time.deltaTime;
        vfxGraph.SetFloat("Point", timer);
    }

    private void BreatheIn() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", 1f);
        vfxGraph.SetFloat("Speed", 1f / breatheInTime);
        Invoke("HoldBreath", breatheInTime);
    }

    private void HoldBreath() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Hold breath", holdBreathTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Speed", 0f);
        Invoke("BreatheOut", holdBreathTime);
    }

    private void BreatheOut() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe out", breatheOutTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", -1f);
        vfxGraph.SetFloat("Speed", 1f / breatheOutTime);
        Invoke("WaitForNewBreath", breatheOutTime);
    }

    private void WaitForNewBreath() {
        vfxGraph.SetFloat("Speed", 0f);
        Invoke("BreatheIn", waitForNewBreath);
    }

    public void Stop() {
        Debug.Log("Stop");
        if (vfxGraph == null)
            vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.Reinit();
    }
}
