using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class SaveCameraInfo : MonoBehaviour
{
    public Camera DepthCamera;
    public VisualEffect vfxGraph;
    RenderTexture rt;

    // Start is called before the first frame update
    void Start()
    {
        rt = new RenderTexture(Screen.width, Screen.height, 16, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
        rt.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
        rt.volumeDepth = 4;
        DepthCamera.targetTexture = rt;
    }

    // Update is called once per frame
    void Update()
    {
        vfxGraph.SetTexture("DepthTexture", rt);
    }
}
