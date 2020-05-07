using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyCatcher : MonoBehaviour
{
    public Material GlassMat;
    public Color startColor;
    public int maxFireFlies = 10;
    public float maxIntensity = 15f;

    private GlobalFlock myFlock;

    private int currentFireflies = 0;

    // Start is called before the first frame update
    void Start()
    {
        myFlock = GetComponent<GlobalFlock>();
        GlassMat = GetComponent<Renderer>().material;
        GlassMat.SetColor("_EmissiveColor", startColor);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Firefly"))
            return;

        Flock flock = other.GetComponent<Flock>();
        flock.MyFlock.AllFish.Remove(flock);

        myFlock.AllFish.Add(flock);
        flock.MyFlock = myFlock;
        flock.RotationSpeed = 1000f;
        flock.SpeedMultiplier = 0f;
        flock.Speed = 0f;

        flock.transform.position = myFlock.transform.position;
        flock.transform.parent = this.transform;

        Destroy(flock);
        
        currentFireflies++;
        GlassMat.SetColor("_EmissiveColor", startColor * (1f + (maxIntensity * (Mathf.Clamp(currentFireflies, 0f, maxFireFlies)) / maxFireFlies)));
    }
}
