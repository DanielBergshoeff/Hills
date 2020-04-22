using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeShow : MonoBehaviour
{
    public Vector3 Size;

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(transform.position, Size);
    }
}
