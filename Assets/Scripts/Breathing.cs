using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Breathing : MonoBehaviour
{
    public static Breathing Instance;

    private VisualEffect vfxGraph;
    private VisualEffect FlowerRainVfxGraph;
    private VisualEffect MountainPaintingVFXGraph;

    [Header("Materials")]
    public Material WaterShaderGraph;

    [Header("Breathing cycle")]
    [SerializeField] private float breatheInTime = 3.0f;
    [SerializeField] private float holdBreathTime = 3.0f;
    [SerializeField] private float breatheOutTime = 3.0f;
    [SerializeField] private int amtOfCycles = 10;

    [Header("Audio")]
    [SerializeField] private AudioClip breatheInAudio;
    [SerializeField] private AudioClip holdbreathAudio;
    [SerializeField] private AudioClip breatheOutAudio;

    [Header("Select effects")]
    [SerializeField] private bool leavesEffect = false;
    [SerializeField] private bool flowerRainEffect = false;
    [SerializeField] private bool waterEffect = false;
    [SerializeField] private bool mountainPainting = false;

    [Header("Leaves effect")]
    [SerializeField] private float leafSpawnRateNormal = 3f;
    [SerializeField] private float leafSpawnRateDuringEffect = 10f;

    [Header("Water effect")]
    [SerializeField] private float minWaterDistance = 3f;
    [SerializeField] private float maxWaterDistance = 15f;

    [Header("Mountain painting effect")]
    [SerializeField] private float mountainPaintingSpawnRate = 30f;
    [SerializeField] private float mountainPaintingSpeed = 3f;

    private float timer = 0f;
    private bool active = false;
    private int currentCycle = 0;
    private CommunicationMessage myMessage;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.resetSeedOnPlay = true;
        vfxGraph.Reinit();

        //CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInTime, Vector3.up * 2f, 0.5f, false);
        //Invoke("BreatheIn", 0f);

        FlowerRainVfxGraph = GameObject.Find("FlowerRain").GetComponent<VisualEffect>();
        MountainPaintingVFXGraph = GameObject.Find("MountainPaint").GetComponent<VisualEffect>();

        Tree.UpdateTree(0f, 1f, leafSpawnRateNormal);
    }

    public void StartBreathing() {
        active = true;
        BreatheInEffects();
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
            if (timer + Time.deltaTime >= breatheInTime + holdBreathTime + breatheOutTime) {
                currentCycle++;
                if (currentCycle == amtOfCycles) {
                    Destroy(gameObject);
                }
                else {
                    BreatheInEffects();
                }
            }
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
            MountainPaintingVFXGraph.SetVector3("Velocity", Wings.Instance.LeftHand.transform.forward * mountainPaintingSpeed);
            MountainPaintingVFXGraph.SetVector3("SpawnPosition", Wings.Instance.LeftHand.transform.position);
        }
    }

    private void SetWaterGraph(float waterPoint) {
        waterPoint = waterPoint.Remap(0f, 1f, minWaterDistance, maxWaterDistance);
        WaterShaderGraph.SetFloat("_RipplePosition", waterPoint);
        WaterShaderGraph.SetVector("_CenterPosition", Camera.main.transform.parent.parent.position);
    }

    private void BreatheInEffects() {
        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe in", breatheInAudio, breatheInTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", 1f);
        vfxGraph.SetFloat("Speed", 1f / breatheInTime);
        timer = 0f;

        if(leavesEffect)
            Tree.UpdateTree(30f, 50f, leafSpawnRateDuringEffect);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 0);

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", 0f);
    }

    private void HoldBreathEffects() {
        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, "Hold breath", holdbreathAudio, holdBreathTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Speed", 0f);

        if(leavesEffect)
            Tree.UpdateTree(30f, 50f, leafSpawnRateDuringEffect);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 0);

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", 0f);
    }

    private void BreatheOutEffects() {
        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, "Breathe out", breatheOutAudio, breatheOutTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", -1f);
        vfxGraph.SetFloat("Speed", 1f / breatheOutTime);

        if(leavesEffect)
            Tree.UpdateTree(0f, 1f, leafSpawnRateNormal);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 100);

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", mountainPaintingSpawnRate);
    }

    private void SetToNeutral() {
        Tree.UpdateTree(0f, 1f, leafSpawnRateNormal);
        FlowerRainVfxGraph.SetInt("SpawnRate", 0);
        MountainPaintingVFXGraph.SetFloat("SpawnRate", 0f);
        WaterShaderGraph.SetFloat("_RipplePosition", 0f);
        WaterShaderGraph.SetVector("_CenterPosition", Vector3.zero);
    }
    

    public void Stop() {
        Debug.Log("Stop");
        if (vfxGraph == null)
            vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.Reinit();
    }

    private void OnDestroy() {
        SetToNeutral();
        if (myMessage != null)
            myMessage.StartFade();
    }
}

public static class ExtensionMethods {

    public static float Remap(this float value, float inputfrom, float inputTo, float outputFrom, float outputTo) {
        return (value - inputfrom) / (inputTo - inputfrom) * (outputTo - outputFrom) + outputFrom;
    }

}
