using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindObject : MonoBehaviour
{
    public float Strength = 1f;
    public float MaxDistance = 15f;

    private void Start() {
        if (WindManager.WindObjects == null)
            WindManager.WindObjects = new List<WindObject>();

        WindManager.WindObjects.Add(this);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
    }
}
