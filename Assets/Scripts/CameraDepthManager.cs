using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDepthManager : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
