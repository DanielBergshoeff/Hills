using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        this.transform.parent.GetComponent<Rigidbody>().SendMessage("OnCollisionEnter", collision);
    }
}
