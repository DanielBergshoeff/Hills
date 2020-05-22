﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallObject : MonoBehaviour {
    public bool HeldInHand = false;
    public bool RandomSize = false;
    private Rigidbody myRigidbody;

    private void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        if (RandomSize) {
            transform.localScale = transform.localScale * Random.Range(0.8f, 1.2f);
        }
    }

    public void StartHolding() {
        if (myRigidbody != null) {
            myRigidbody.constraints = RigidbodyConstraints.None;
        }
    }

    public void LetGo() {
        //CheckForEating();
        HeldInHand = true;
        Invoke("SetHeldInHand", 3f);
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
