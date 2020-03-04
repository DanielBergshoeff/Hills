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
    private AudioSource mySongSource;
    private float timer = 0f;

    private VRTK.VRTK_InteractableObject vrtkObject;

    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.spatialBlend = 1f;
        myAudioSource.minDistance = 0.1f;
        myAudioSource.maxDistance = 5f;
        myAudioSource.volume = 0f;

        mySongSource = gameObject.AddComponent<AudioSource>();
        mySongSource.spatialBlend = 1f;
        mySongSource.minDistance = 0.1f;
        mySongSource.maxDistance = 5f;
        mySongSource.volume = 0f;

        vrtkObject = GetComponent<VRTK.VRTK_InteractableObject>();


        if (ShakeDetection.shakeEvent == null)
            ShakeDetection.shakeEvent = new ShakeEvent();

        ShakeDetection.shakeEvent.AddListener(Shaken);
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
        List<AudioClip> tempClips = new List<AudioClip>(AudioManager.AllClips);
        tempClips.Remove(mySongSource.clip);
        mySongSource.clip = tempClips[Random.Range(0, tempClips.Count)];
        mySongSource.Play();
    }

    private void IncreaseWaveSound() {
        if (timer >= timeTillSong)
            return;

        timer += Time.deltaTime;
        myAudioSource.volume = timer / timeTillSong;

        if (timer >= timeTillSong) {
            fullsong = true;
            timer = 0f;
            mySongSource.clip = AudioManager.AllClips[0];
            mySongSource.Play();
        }
    }

    private void IncreaseSongSound() {
        if (timer >= timeTillFullSong)
            return;

        timer += Time.deltaTime;
        mySongSource.volume = timer / timeTillFullSong;
        myAudioSource.volume = 1f - timer / timeTillFullSong;
        mySongSource.spatialBlend = 1f - timer / timeTillFullSong;
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
