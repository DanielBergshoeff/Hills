using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaAudio : MonoBehaviour
{
    public static SeaAudio Instance;
    public float RefreshTime = 5f;
    public float TimeTillFullVolume = 5f;

    private AudioSource myAudioSource;
    private bool inRange = false;
    private float volumeMultiplier = 0f;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if(inRange && volumeMultiplier < 1f) {
            volumeMultiplier += Time.deltaTime / TimeTillFullVolume;
        }
        else if(!inRange && volumeMultiplier > 0f) {
            volumeMultiplier -= Time.deltaTime / TimeTillFullVolume;
        }

    }

    public void RefreshAudio() {
        if (Camera.main == null || !inRange)
            return;

        float distance = (transform.position - Camera.main.transform.position).magnitude;
        distance = Mathf.Clamp(distance, 80f, 180f);
        distance = distance.Remap(80f, 180f, 0f, 1f);
        distance = 1f - distance;
        myAudioSource.volume = volumeMultiplier * distance;
    }

    public void CheckForPlayer() {
        if (Camera.main != null) {
            if (GetComponent<BoxCollider>().bounds.Contains(Camera.main.transform.position)) {
                inRange = true;
            }
            else {
                inRange = false;
            }
        }
    }
}
