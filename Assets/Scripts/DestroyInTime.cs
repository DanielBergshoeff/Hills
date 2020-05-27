using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public float TimeTillDestroy = 3f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroySelf", TimeTillDestroy);
    }

    private void DestroySelf() {
        Destroy(gameObject);
    }
}
