using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Breathing : MonoBehaviour
{
    private VisualEffect vfxGraph;
    public VisualEffect TreeVfxGraph;
    public VisualEffect FlowerRainVfxGraph;
    public VisualEffect MountainPaintingVFXGraph;
    public Material WaterShaderGraph;

    public Transform LeftHand;

    [SerializeField] private float breatheInTime = 3.0f;
    [SerializeField] private float holdBreathTime = 3.0f;
    [SerializeField] private float breatheOutTime = 3.0f;
    [SerializeField] private float waitForNewBreath = 1.0f;

    [SerializeField] private bool leavesEffect = false;
    [SerializeField] private bool flowerRainEffect = false;
    [SerializeField] private bool waterEffect = false;
    [SerializeField] private bool mountainPainting = false;

    [SerializeField] private float leafSpawnRateNormal = 3f;
    [SerializeField] private float leafSpawnRateDuringEffect = 10f;

    [SerializeField] private float minWaterDistance = 3f;
    [SerializeField] private float maxWaterDistance = 15f;

    [SerializeField] private float mountainPaintingSpawnRate = 30f;
    [SerializeField] private float mountainPaintingSpeed = 3f;

    private float timer = 0f;
    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.resetSeedOnPlay = true;
        vfxGraph.Reinit();

        vfxGraph.enabled = false;
        //CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInTime, Vector3.up * 2f, 0.5f, false);
        //Invoke("BreatheIn", 0f);

        Tree.UpdateTree(0f, 1f, leafSpawnRateNormal);
    }

    private void StartBreathing() {
        vfxGraph.transform.position = Camera.main.transform.parent.parent.position + Camera.main.transform.parent.parent.forward * 5f + Vector3.up * 1f;
        vfxGraph.enabled = true;
        active = true;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("Start!");
            StartBreathing();
        }

        if (!active)
            return;

        if (timer < breatheInTime) {
            ContinuousBreatheInEffects();
            if(timer + Time.deltaTime >= breatheInTime)
                HoldBreathEffects();
        }
        else if (timer < breatheInTime + holdBreathTime) {
            ContinuousHoldBreathEffects();
            if(timer + Time.deltaTime >= breatheInTime + holdBreathTime)
                BreatheOutEffects();
        }
        else if(timer < breatheInTime + holdBreathTime + breatheOutTime) {
            ContinuousBreatheOutEffects();
            if(timer + Time.deltaTime >= breatheInTime + holdBreathTime + breatheOutTime)
                BreatheInEffects();
        }

        timer += Time.deltaTime;
        vfxGraph.SetFloat("Point", timer);
    }

    private void ContinuousBreatheInEffects() {
        if (waterEffect) {
            float waterPoint = 1f - (timer / breatheInTime);
            SetWaterGraph(waterPoint);
        }
    }

    private void ContinuousHoldBreathEffects() {

    }

    private void ContinuousBreatheOutEffects() {
        if (waterEffect) {
            float waterPoint = (timer - breatheInTime - holdBreathTime) / breatheOutTime;
            SetWaterGraph(waterPoint);
        }
        if (mountainPainting) {
            MountainPaintingVFXGraph.SetVector3("Velocity", LeftHand.transform.forward * mountainPaintingSpeed);
            MountainPaintingVFXGraph.SetVector3("SpawnPosition", LeftHand.transform.position);
        }
    }

    private void SetWaterGraph(float waterPoint) {
        waterPoint = waterPoint.Remap(0f, 1f, minWaterDistance, maxWaterDistance);
        WaterShaderGraph.SetFloat("_RipplePosition", waterPoint);
        WaterShaderGraph.SetVector("_CenterPosition", Camera.main.transform.parent.parent.position);
    }

    private void BreatheInEffects() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", 1f);
        vfxGraph.SetFloat("Speed", 1f / breatheInTime);
        TreeVfxGraph.SetFloat("Direction", 0f);
        timer = 0f;

        if(leavesEffect)
            Tree.UpdateTree(30f, 50f, leafSpawnRateDuringEffect);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 0);

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", 0f);
    }

    private void HoldBreathEffects() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Hold breath", holdBreathTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Speed", 0f);

        if(leavesEffect)
            Tree.UpdateTree(30f, 50f, leafSpawnRateDuringEffect);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 0);

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", 0f);
    }

    private void BreatheOutEffects() {
        CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe out", breatheOutTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", -1f);
        vfxGraph.SetFloat("Speed", 1f / breatheOutTime);
        TreeVfxGraph.SetFloat("Direction", 1f);

        if(leavesEffect)
            Tree.UpdateTree(0f, 1f, leafSpawnRateNormal);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 16);

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", mountainPaintingSpawnRate);
    }
    

    public void Stop() {
        Debug.Log("Stop");
        if (vfxGraph == null)
            vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.Reinit();
    }
}

public static class ExtensionMethods {

    public static float Remap(this float value, float inputfrom, float inputTo, float outputFrom, float outputTo) {
        return (value - inputfrom) / (inputTo - inputfrom) * (outputTo - outputFrom) + outputFrom;
    }

}
