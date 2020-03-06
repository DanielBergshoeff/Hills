using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioSource myAudioSource;
    public static float[] Samples = new float[512];
    public static float[] FrequencyBands = new float[8];
    public static float[] BandBuffers = new float[8];
    public static float Average = 0f;
    float[] bufferDecrease = new float[8];

    public static AudioClip[] AllClips;
    public static AudioClip WaveAudio;
    public AudioClip[] Clips;
    public AudioClip WavesAudio;

    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        AllClips = Clips;
        WaveAudio = WavesAudio;
        /*myAudioSource.clip = Clips[0];
        myAudioSource.Play();*/
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        //BandBuffer();
        //CalculateAverage();
    }

    private void GetSpectrumAudioSource() {
        myAudioSource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
    }

    private void MakeFrequencyBands() {
        int count = 0;

        for (int i = 0; i < FrequencyBands.Length; i++) {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i == FrequencyBands.Length - 1) 
                sampleCount += 2;
            for (int j = 0; j < sampleCount; j++) {
                average += Samples[count] * (count + 1);
                count++;
            }

            average /= count;
            FrequencyBands[i] = average * 10;
        }
    }

    private void BandBuffer() {
        for (int i = 0; i < FrequencyBands.Length; i++) {
            if(FrequencyBands[i] > BandBuffers[i]) {
                BandBuffers[i] = FrequencyBands[i];
                bufferDecrease[i] = 0.005f;
            }
            if(FrequencyBands[i] < BandBuffers[i]) {
                BandBuffers[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }
        }
    }

    private void CalculateAverage() {
        float total = 0f;
        for (int i = 0; i < FrequencyBands.Length; i++) {
            total += FrequencyBands[i];
        }
        Average = total / FrequencyBands.Length;
    }
}
