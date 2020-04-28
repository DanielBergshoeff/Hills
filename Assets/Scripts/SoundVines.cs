using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVines : MonoBehaviour
{
    public AudioClip MySound;
    public float WaitTime = 0.1f;

    public AudioSource MyAudioSource;

    private bool readyForSound = true;

    private void ReadyForSound() {
        readyForSound = true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!readyForSound || !collision.transform.CompareTag("SmallObject"))
            return;

        MyAudioSource.PlayOneShot(MySound);
        readyForSound = false;

        Invoke("ReadyForSound", WaitTime);
    }
}
