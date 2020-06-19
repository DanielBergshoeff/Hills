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

    /// <summary>
    /// Activates the breathing exercise
    /// </summary>
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

    /// <summary>
    /// Updates every frame during breathing in phase
    /// </summary>
    private void ContinuousBreatheInEffects() {
        if (waterEffect) {
            float waterPoint = 1f - (timer / breatheInTime);
            SetWaterGraph(waterPoint);
        }
    }

    /// <summary>
    /// Updates every frame during holding breath phase
    /// </summary>
    private void ContinuousHoldBreathEffects() {

    }

    /// <summary>
    /// Updates every frame during breathing out phase
    /// </summary>
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

    /// <summary>
    /// Updates water VFX graph
    /// </summary>
    /// <param name="waterPoint"></param>
    private void SetWaterGraph(float waterPoint) {
        waterPoint = waterPoint.Remap(0f, 1f, minWaterDistance, maxWaterDistance);
        WaterShaderGraph.SetFloat("_RipplePosition", waterPoint);
        WaterShaderGraph.SetVector("_CenterPosition", Camera.main.transform.parent.parent.position);
    }

    /// <summary>
    /// Sets once when the breathing in phase starts
    /// </summary>
    private void BreatheInEffects() {
        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, MenuManager.Dutch ? "Adem in" : "Breathe in", AudioManager.BreatheInClips[Random.Range(0, AudioManager.BreatheInClips.Count)], breatheInTime, Vector3.up * 2f, 0.5f, false);
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


    /// <summary>
    /// Sets once when the holding breath phase starts
    /// </summary>
    private void HoldBreathEffects() {
        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, MenuManager.Dutch ? "Houd vast" : "Hold breath", AudioManager.HoldBreathClips[Random.Range(0, AudioManager.HoldBreathClips.Count)], holdBreathTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Speed", 0f);

        if(leavesEffect)
            Tree.UpdateTree(30f, 50f, leafSpawnRateDuringEffect);

        if (flowerRainEffect)
            FlowerRainVfxGraph.SetInt("SpawnRate", 0);

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", 0f);
    }

    /// <summary>
    /// Sets once when the breathing out phase starts
    /// </summary>
    private void BreatheOutEffects() {
        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, MenuManager.Dutch ? "Adem uit" : "Breathe out", AudioManager.BreatheOutClips[Random.Range(0, AudioManager.BreatheOutClips.Count)], breatheOutTime, Vector3.up * 2f, 0.5f, false);
        vfxGraph.SetFloat("Direction", -1f);
        vfxGraph.SetFloat("Speed", 1f / breatheOutTime);

        if(leavesEffect)
            Tree.UpdateTree(0f, 1f, leafSpawnRateNormal);

        if (flowerRainEffect) {
            FlowerRainVfxGraph.SetInt("SpawnRate", 200);
        }

        if (mountainPainting)
            MountainPaintingVFXGraph.SetFloat("SpawnRate", mountainPaintingSpawnRate);
    }


    /// <summary>
    /// Return all the graphs to neutral
    /// </summary>
    private void SetToNeutral() {
        Tree.UpdateTree(0f, 1f, leafSpawnRateNormal);
        FlowerRainVfxGraph.SetInt("SpawnRate", 0);
        MountainPaintingVFXGraph.SetFloat("SpawnRate", 0f);
        WaterShaderGraph.SetFloat("_RipplePosition", 0f);
        WaterShaderGraph.SetVector("_CenterPosition", Vector3.zero);
    }
    
    /// <summary>
    /// Stop the breathing exercise
    /// </summary>
    public void Stop() {
        Debug.Log("Stop");
        if (vfxGraph == null)
            vfxGraph = GetComponent<VisualEffect>();
        vfxGraph.Reinit();
    }

    /// <summary>
    /// Cleans up before destroying self
    /// </summary>
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
