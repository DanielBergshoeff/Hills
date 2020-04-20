using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOption : MonoBehaviour
{
    public bool Selected;
    
    private void OnTriggerEnter(Collider other) {
        if (other.name != "LeftHandAnchor" && other.name != "RightHandAnchor")
            return;

        gameObject.transform.localScale = Vector3.one * 1.25f;
        Selected = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.name != "LeftHandAnchor" && other.name != "RightHandAnchor")
            return;

        gameObject.transform.localScale = Vector3.one;
        Selected = false;
    }

    public virtual void Select() { }
}
