using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplication : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("SmallObject")) {
            if (other.GetComponent<SmallObject>().HeldInHand) {
                GameObject go = Instantiate(other.gameObject);
                go.transform.position = other.transform.position;
                go.GetComponent<SmallObject>().HeldInHand = false;
            }
        }
    }
}
