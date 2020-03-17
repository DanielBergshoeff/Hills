using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour
{
    public Material GrassMaterial;
    public Transform CenterPosition;
    public float Distance;
    public Terrain MyTerrain;

    public float Ripple;
    public float RippleSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (GetIbiRate.heartBeatEvent == null)
            GetIbiRate.heartBeatEvent = new HeartBeatEvent();

        GetIbiRate.heartBeatEvent.AddListener(ResetRipple);
    }

    private void ResetRipple(int ibi) {
        Ripple = 0f;
        GrassMaterial.SetFloat("Ripple", Ripple);

        MyTerrain.terrainData.RefreshPrototypes();
        MyTerrain.Flush();
    }

    // Update is called once per frame
    void Update()
    {
        Ripple += Time.deltaTime * RippleSpeed;
        GrassMaterial.SetFloat("Ripple", Ripple);
        GrassMaterial.SetVector("CenterPosition", CenterPosition.position);
        GrassMaterial.SetFloat("Distance", Distance);
        MyTerrain.terrainData.RefreshPrototypes();
        MyTerrain.Flush();
    }
}
