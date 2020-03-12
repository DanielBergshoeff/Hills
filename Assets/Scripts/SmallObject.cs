﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallObject : MonoBehaviour {
    public bool HeldInHand = false;
    private Rigidbody myRigidbody;

    private void Start() {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void LetGo() {
        CheckForEating();
        HeldInHand = true;
        Debug.Log("Let go");

        if (myRigidbody != null) {
            Debug.Log("Allow rotation");
            myRigidbody.constraints = RigidbodyConstraints.None;
        }

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
