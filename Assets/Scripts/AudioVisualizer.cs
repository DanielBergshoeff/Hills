using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioVisualizer : MonoBehaviour {
    [SerializeField] private GameObject audioVisualizerPrefab;
    [SerializeField] private int amtOfVisualizers = 8;
    [SerializeField] private Gradient colours;

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
            audioVisualizers[i].SetFloat("Strength", AudioManager.FrequencyBands[i] * AudioManager.FrequencyBands[i]);
            audioVisualizers[i].SetFloat("TurbulenceIntensity", AudioManager.FrequencyBands[i] / 3f);
        }
    }

    private void SpawnVisualizers() {
        audioVisualizers = new List<VisualEffect>();
        for (int i = 0; i < amtOfVisualizers; i++) {
            Color color = colours.Evaluate((1f / amtOfVisualizers) * i);
            VisualEffect ve = Instantiate(audioVisualizerPrefab).GetComponent<VisualEffect>();

            ve.transform.parent = transform;
            ve.transform.localPosition = Vector3.right * i;
            ve.SetVector4("Color", color);

            audioVisualizers.Add(ve);
        }
    }

}
