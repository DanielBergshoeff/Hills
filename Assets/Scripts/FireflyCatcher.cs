using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyCatcher : MonoBehaviour
{
    public Light CatcherLight;
    public Color startColor;
    public int maxFireFlies = 10;
    public float maxIntensity = 15f;

    private GlobalFlock myFlock;
    private List<Flock> myFireflies;

    public int currentFireflies = 0;

    private AudioSource myAudioSource;

    // Start is called before the first frame update
    void Start() {
        myFlock = GetComponent<GlobalFlock>();
        myFireflies = new List<Flock>();
        myAudioSource = GetComponent<AudioSource>();
        CatcherLight.intensity = 0f;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Firefly"))
            return;

        Flock flock = other.GetComponent<Flock>();
        if (flock == null || flock.catchCooldown > 0f)
            return;

        flock.MyFlock.AllFish.Remove(flock);

        myAudioSource.PlayOneShot(AudioManager.Instance.FireflycatcherCatchSound);

        flock.transform.position = myFlock.transform.position;
        flock.transform.parent = this.transform;
        flock.enabled = false;
        myFireflies.Add(flock);

        currentFireflies++;
        CatcherLight.intensity = maxIntensity * (Mathf.Clamp(currentFireflies, 0f, maxFireFlies)) / maxFireFlies;
    }

    public void RemoveFirefly() {
        if (myFireflies.Count <= 0)
            return;

        currentFireflies--;
        CatcherLight.intensity = maxIntensity * (Mathf.Clamp(currentFireflies, 0f, maxFireFlies)) / maxFireFlies;

        myFireflies[0].enabled = true;
        myFireflies[0].CatchCooldown();
        myFireflies[0].MyFlock.AllFish.Add(myFireflies[0]);
        myFireflies[0].transform.parent = null;
        myFireflies.RemoveAt(0);
    }
}
