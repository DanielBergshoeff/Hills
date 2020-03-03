using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seashell : MonoBehaviour
{
    [SerializeField] private float timeTillSong;
    [SerializeField] private float timeTillFullSong;

    private bool activated = false;
    private bool fullsong = false;
    private AudioSource myAudioSource;
    private AudioSource mySongSource;
    private float timer = 0f;
    

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
    }

    // Update is called once per frame
    void Update()
    {
        if (activated && !fullsong)
            IncreaseWaveSound();
        else if (activated)
            IncreaseSongSound();
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
