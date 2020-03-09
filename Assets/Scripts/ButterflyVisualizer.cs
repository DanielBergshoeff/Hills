using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ButterflyVisualizer : MonoBehaviour {
    [SerializeField] private GameObject butterflyVisualizerPrefab;
    [SerializeField] private int amtOfVisualizers = 8;
    [SerializeField] private Gradient colours;

    [SerializeField] private int amtOfButterflies = 100;
    [SerializeField] private float distanceMultiplier = 5.0f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float speedMultiplier = 2.0f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float colorMultiplier = 3.0f;

    private List<VisualEffect> audioVisualizers;

    // Start is called before the first frame update
    void Start() {
        SpawnVisualizers();
    }

    // Update is called once per frame
    void Update() {
        ChangeColorStrength();
    }

    private void ChangeColorStrength() {
        for (int i = 0; i < amtOfVisualizers; i++) {
            audioVisualizers[i].SetFloat("MaxDistance", Mathf.Clamp(AudioManager.FrequencyBands[i] * distanceMultiplier, minDistance, 500f));
            audioVisualizers[i].SetFloat("Speed", Mathf.Clamp(AudioManager.FrequencyBands[i] * speedMultiplier, minSpeed, 500f));
        }
    }

    private void SpawnVisualizers() {
        audioVisualizers = new List<VisualEffect>();
        for (int i = 0; i < amtOfVisualizers; i++) {
            Color color = colours.Evaluate((1f / amtOfVisualizers) * i);
            VisualEffect ve = Instantiate(butterflyVisualizerPrefab).GetComponent<VisualEffect>();
            ve.transform.parent = transform;
            //ve.transform.localPosition = Vector3.right * i;
            ve.SetVector4("Color", color * colorMultiplier);
            ve.SetInt("AmtOfButterflies", amtOfButterflies);
            audioVisualizers.Add(ve);
        }
    }

}
