using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatEffectManager : MonoBehaviour
{
    public Material SaplingMaterial;
    public float Ripple = 0f;
    public float Range = 0.1f;
    public float RippleSpeed = 1f;

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

        rippleToStrength[currentRipple] = 0f;
        SaplingMaterial.SetFloat(rippleToName[currentRipple], rippleToStrength[currentRipple]);
    }
}
