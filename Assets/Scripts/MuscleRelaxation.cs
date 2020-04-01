using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class MuscleRelaxation : MonoBehaviour
{
    public static class PropName {
        public const string PositionMap = "PositionMap";
        public const string PositionOffset = "PositionOffset";
        public const string UVMap = "UVMap";
        public const string ModelMainTex = "ModelMainTex";
        public const string VtxCount = "VtxCount";
        public const string Scale = "Scale";
    }

    public GameObject newModel;
    public GameObject model;

    private int pointCountPerArea = 100000;
    private VisualEffect vfxGraph;
    private MapSet mapSet;
    private Texture modelMainTex;
    private Animator pausedAnim;

    // Start is called before the first frame update
    void Start()
    {
        vfxGraph = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if(newModel != null) {
            model = newModel;
            newModel = null;
            UpdateModel();
        }
    }

    private void UpdateModel() {
        InitEffect();

        vfxGraph.SetTexture(PropName.PositionMap, mapSet.position);
        vfxGraph.SetVector3(PropName.PositionOffset, model.transform.position);
        /*
        vfxGraph.SetTexture(PropName.UVMap, mapSet.uv);
        vfxGraph.SetTexture(PropName.ModelMainTex, modelMainTex);
        vfxGraph.SetInt(PropName.VtxCount, mapSet.vtxCount);*/
        vfxGraph.SetVector3(PropName.Scale, mapSet.scale);
    }

    private void InitEffect() {
        var modelTrans = model.transform;
        var meshAndTexs = GetMeshData();

        foreach (var (mesh, tex) in meshAndTexs) {
            mapSet = MeshToMap.ComputeMap(mesh, pointCountPerArea);
            modelMainTex = tex;
            mapSet.scale = modelTrans.localScale * 1.1f;
        }
    }

    IEnumerable<(Mesh, Texture)> GetMeshData() {
        var smr = model.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null) {
            // stop animation
            pausedAnim = model.GetComponentInParent<Animator>();
            pausedAnim.enabled = false;

            var material = smr.sharedMaterial;

            var mesh = new Mesh();
            smr.BakeMesh(mesh);

            return new[] { (mesh, material.mainTexture) };
        }
        else {
            var mf = model.GetComponent<MeshFilter>();
            var renderer = model.GetComponentInChildren<Renderer>();

            var mesh = mf.sharedMesh;
            var meshCount = mesh.subMeshCount;
            var materials = renderer.sharedMaterials; ;

            return new[] { (mesh, materials.First().mainTexture) };
        }
    }
}
