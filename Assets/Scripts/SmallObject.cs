using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallObject : MonoBehaviour {
    public bool HeldInHand = false;

    public void LetGo() {
        HeldInHand = true;
        Invoke("SetHeldInHand", 3f);
    }

    public void SetHeldInHand() {
        HeldInHand = false;
    }
}
