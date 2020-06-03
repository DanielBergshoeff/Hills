using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seashell : MonoBehaviour
{
    [SerializeField] private float timeTillSong;
    [SerializeField] private float timeTillFullSong;
    [SerializeField] private float timeForShake = 2.0f;

    private bool activated = false;
    private bool fullsong = false;
    private bool shakeAllowed = true;
    private AudioSource myAudioSource;
    public static AudioSource MySongSource;
    private float timer = 0f;

    private VRTK.VRTK_InteractableObject vrtkObject;

    private void Awake() {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.spatialBlend = 1f;
        myAudioSource.minDistance = 0.1f;
        myAudioSource.maxDistance = 5f;
        myAudioSource.volume = 0f;

        MySongSource = gameObject.AddComponent<AudioSource>();
        MySongSource.spatialBlend = 1f;
        MySongSource.minDistance = 0.1f;
        MySongSource.maxDistance = 5f;
        MySongSource.volume = 0f;

        vrtkObject = GetComponent<VRTK.VRTK_InteractableObject>();


        if (ShakeDetection.shakeEvent == null)
            ShakeDetection.shakeEvent = new ShakeEvent();

        ShakeDetection.shakeEvent.AddListener(Shaken);
    }

    public void ToggleSong() {
        if (!activated || !fullsong)
            return;

        MySongSource.Stop();
        activated = false;
        fullsong = false;
        timer = 0f;
    }

    private void EnableShaking() {
        shakeAllowed = true;
    }

    private void Shaken(GameObject go) {
        if (!shakeAllowed || !vrtkObject.IsGrabbed(go) || !fullsong)
            return;

        Debug.Log("Shake with correct hand");

        shakeAllowed = false;
        SwitchSong();
        Invoke("EnableShaking", timeForShake);
    }

    // Update is called once per frame
    void Update()
    {
        if (activated && !fullsong)
            IncreaseWaveSound();
        else if (activated)
            IncreaseSongSound();
    }

    private void SwitchSong() {
        if (!activated || !fullsong)
            return;

        List<AudioClip> tempClips = new List<AudioClip>(AudioManager.AllClips);
        tempClips.Remove(MySongSource.clip);
        MySongSource.clip = tempClips[Random.Range(0, tempClips.Count)];
        MySongSource.Play();
    }

    private void IncreaseWaveSound() {
        if (timer >= timeTillSong)
            return;

        timer += Time.deltaTime;
        myAudioSource.volume = timer / timeTillSong;

        if (timer >= timeTillSong) {
            fullsong = true;
            timer = 0f;
            MySongSource.clip = AudioManager.AllClips[0];
            MySongSource.Play();
            MySongSource.loop = true;
        }
    }

    private void IncreaseSongSound() {
        if (timer >= timeTillFullSong)
            return;

        timer += Time.deltaTime;
        MySongSource.volume = timer / timeTillFullSong;
        myAudioSource.volume = 1f - timer / timeTillFullSong;
        MySongSource.spatialBlend = 1f - timer / timeTillFullSong;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("MainCamera"))
            return;

        if (activated)
            return;

        activated = !activated;

        myAudioSource.clip = AudioManager.WaveAudio;
        myAudioSource.loop = true;
        myAudioSource.Play();
    }
}
