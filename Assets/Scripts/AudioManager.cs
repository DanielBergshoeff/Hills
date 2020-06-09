using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
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
    public bool AutoPlay = false;
    public AudioClip CoconutBucketSound;
    public AudioClip CoconutGroundSound;
    public AudioClip CoconutWaterSound;
    public AudioClip FireflycatcherCatchSound;
    public AudioClip LeafThrow;
    public AudioClip StartPaintingSound;
    public AudioClip LoopPaintingSound;
    public AudioClip MenuHoverSound;

    public List<AudioClip> WoodGrabClips;

    public static List<AudioClip> BreatheInClips;
    public static List<AudioClip> BreatheOutClips;
    public static List<AudioClip> HoldBreathClips;
    public static List<AudioClip> TutorialClips;

    private void Awake() {
        Instance = this;
        string language = MenuManager.Dutch ? "Dutch" : "English";
        string pathBreatheIn = "TextClips/" + language + "/BreatheIn";
        string pathBreatheOut = "TextClips/" + language + "/BreatheOut";
        string pathHoldBreath = "TextClips/" + language + "/HoldBreath";
        BreatheInClips = new List<AudioClip>(Resources.LoadAll(pathBreatheIn, typeof(AudioClip)).Cast<AudioClip>());
        BreatheOutClips = new List<AudioClip>(Resources.LoadAll(pathBreatheOut, typeof(AudioClip)).Cast<AudioClip>());
        HoldBreathClips = new List<AudioClip>(Resources.LoadAll(pathHoldBreath, typeof(AudioClip)).Cast<AudioClip>());

        string pathTutorial = "Tutorial/" + language;
        TutorialClips = new List<AudioClip>(Resources.LoadAll(pathTutorial, typeof(AudioClip)).Cast<AudioClip>());
    }

    // Start is called before the first frame update
    void Start()
    {
        if (AutoPlay)
            myAudioSource = gameObject.AddComponent<AudioSource>();
        else
            myAudioSource = Seashell.MySongSource;
        AllClips = Clips;
        WaveAudio = WavesAudio;


        if (!AutoPlay)
            return;
        myAudioSource.clip = Clips[0];
        myAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        CalculateAverage();
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

    private void CalculateAverage() {
        float total = 0f;
        for (int i = 0; i < FrequencyBands.Length; i++) {
            total += FrequencyBands[i];
        }
        Average = total / FrequencyBands.Length;
    }
}
