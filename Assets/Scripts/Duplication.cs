using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplication : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("SmallObject")) {
            SmallObject otherObject = other.GetComponent<SmallObject>();
            if (otherObject.HeldInHand) {
                otherObject.HeldInHand = false;
                GameObject go = Instantiate(other.gameObject);
                go.transform.position = other.transform.position;
            }
        }
    }
}
