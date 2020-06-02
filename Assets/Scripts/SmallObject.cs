using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallObject : MonoBehaviour {
    public bool HeldInHand = false;
    public bool RandomSize = false;
    public bool Leaf = false;
    private Rigidbody myRigidbody;

    private AudioSource myAudioSource;

    private void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        if (RandomSize) {
            transform.localScale = transform.localScale * Random.Range(0.8f, 1.2f);
        }

        myAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void StartHolding() {
        if (myRigidbody != null) {
            myRigidbody.constraints = RigidbodyConstraints.None;
        }

        Explanation ex = GetComponent<Explanation>();
        if (ex != null) {
            ex.StopMessage();
        }
    }

    public void LetGo() {
        //CheckForEating();
        HeldInHand = true;
        Invoke("SetHeldInHand", 3f);

        if (Leaf) {
            myAudioSource.PlayOneShot(AudioManager.Instance.LeafThrow);
        }
    }

    public void SetHeldInHand() {
        HeldInHand = false;
    }

    private void CheckForEating() {
        if(Vector3.Distance(transform.position, Camera.main.transform.position) < 0.2f) {
            Destroy(gameObject);
        }
    }
}
