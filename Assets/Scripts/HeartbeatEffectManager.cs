using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HeartbeatEffectManager : MonoBehaviour
{
    public Material SaplingMaterial;
    public float Ripple = 0f;
    public float Range = 0.1f;
    public float RippleSpeed = 1f;

    public GameObject DecalObjectPrefab;
    public int amountOfRipples = 5;

    private Dictionary<int, string> rippleToName = new Dictionary<int, string>() {
        {0, "Ripple" },
        {1, "Ripple2" },
        {2, "Ripple3" },
        {3, "Ripple4" },
        {4, "Ripple5" },
    };

    private Dictionary<int, float> rippleToStrength;

    private int currentRipple = 0;
    private bool init = false;

    private List<DecalProjector> projectors;
    private List<float> rippleSizes;

    // Start is called before the first frame update
    void Start()
    {
        if (GetIbiRate.heartBeatEvent == null)
            GetIbiRate.heartBeatEvent = new HeartBeatEvent();

        GetIbiRate.heartBeatEvent.AddListener(ResetRipple);

        rippleToStrength = new Dictionary<int, float>();
        for (int i = 0; i < rippleToName.Count; i++) {
            rippleToStrength.Add(i, 0f);
        }

        projectors = new List<DecalProjector>();
        rippleSizes = new List<float>();
        for (int i = 0; i < amountOfRipples; i++) {
            GameObject go = Instantiate(DecalObjectPrefab);
            projectors.Add(go.GetComponent<DecalProjector>());
            rippleSizes.Add(0f);
            projectors[i].size = new Vector3(rippleSizes[i], rippleSizes[i], rippleSizes[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
            Initialize();

        for (int i = 0; i < rippleToName.Count; i++) {
            rippleToStrength[i] += Time.deltaTime * RippleSpeed;
            SaplingMaterial.SetFloat(rippleToName[i], rippleToStrength[i]);
        }
        SaplingMaterial.SetFloat("Vector1_1C651665", Range);

        for (int i = 0; i < projectors.Count; i++) {
            rippleSizes[i] += Time.deltaTime * RippleSpeed;
            projectors[i].size = new Vector3(rippleSizes[i], rippleSizes[i], rippleSizes[i]);
            projectors[i].transform.position = Camera.main.transform.parent.parent.position;
        }
    }

    private void Initialize() {
        SaplingMaterial.SetVector("Vector3_3FA21F24", Camera.main.transform.parent.parent.position);
        init = true;
    }

    private void ResetRipple(int ibi) {
        currentRipple++;
        if (currentRipple > rippleToName.Count - 1)
            currentRipple = 0;

        Debug.Log(currentRipple);


        rippleSizes[currentRipple] = 0f;
        rippleToStrength[currentRipple] = 0f;
        SaplingMaterial.SetFloat(rippleToName[currentRipple], rippleToStrength[currentRipple]);
    }
}
