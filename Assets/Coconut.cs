using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coconut : MonoBehaviour
{
    private AudioSource myAudioSource;

    void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Interactable")) {
            myAudioSource.PlayOneShot(AudioManager.Instance.CoconutBucketSound);
        }
        else {
            myAudioSource.PlayOneShot(AudioManager.Instance.CoconutGroundSound);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Water")) {
            myAudioSource.PlayOneShot(AudioManager.Instance.CoconutWaterSound);
        }
    }
}
