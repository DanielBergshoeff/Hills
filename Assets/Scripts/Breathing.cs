using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Breathing : MonoBehaviour
{
    private VisualEffect vfxGraph;
    public VisualEffect TreeVfxGraph;
    public VisualEffect FlowerRainVfxGraph;

    [SerializeField] private float breatheInTime = 3.0f;
    [SerializeField] private float holdBreathTime = 3.0f;
    [SerializeField] private float breatheOutTime = 3.0f;
    [SerializeField] private float waitForNewBreath = 1.0f;

    [SerializeField] private bool leavesEffect = false;
    [SerializeField] private bool flowerRainEffect = false;

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
            HoldBreathEffects();
        }
        else if (timer < breatheInTime + holdBreathTime && timer + Time.deltaTime >= breatheInTime + holdBreathTime) {
            BreatheOutEffects();
        }
        else if(timer < breatheInTime + holdBreathTime + breatheOutTime && timer + Time.deltaTime >= breatheInTime + holdBreathTime + breatheOutTime) {
            BreatheInEffects();
        }

        timer += Time.deltaTime;
        vfxGraph.SetFloat("Point", timer);
    }

    private void BreatheInEffects() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", 1f);
        vfxGraph.SetFloat("Speed", 1f / breatheInTime);
        TreeVfxGraph.SetFloat("Direction", 0f);
        timer = 0f;

        if(leavesEffect)
            Tree.UpdateTree(30f, 50f);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 0);
    }

    private void HoldBreathEffects() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Hold breath", holdBreathTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Speed", 0f);

        if(leavesEffect)
            Tree.UpdateTree(30f, 50f);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 0);
    }

    private void BreatheOutEffects() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe out", breatheOutTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", -1f);
        vfxGraph.SetFloat("Speed", 1f / breatheOutTime);
        TreeVfxGraph.SetFloat("Direction", 1f);

        if(leavesEffect)
            Tree.UpdateTree(0f, 1f);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 16);
    }
    

    public void Stop() {
        Debug.Log("Stop");
        if (vfxGraph == null)
            vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.Reinit();
    }
}
