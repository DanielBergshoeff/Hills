﻿using System.Collections;
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
        public const string Speed = "Speed";
        public const string TurbulenceIntensity = "TurbulenceIntensity";
    }

    private int pointCountPerArea = 100000;
    private VisualEffect vfxGraph;
    private Animator pausedAnim;

    private int currentMuscle = 0;
    public List<GameObject> MuscleGroups;
    public float timePerMuscle = 10f;
    public float switchTime = 3f;
    public float turbulenceIntensity = 0f;
    public float maxTurbulenceIntensity = 3f;
    private List<MapSet> mapSets;


    // Start is called before the first frame update
    void Start()
    {
        vfxGraph = GetComponent<VisualEffect>();
        mapSets = new List<MapSet>();

        foreach(GameObject go in MuscleGroups) {
            mapSets.Add(InitEffect(go));
        }

        UpdateModel();
        vfxGraph.SetFloat(PropName.Speed, 1f / switchTime);

        Invoke("SwitchMuscleGroup", timePerMuscle);
    }

    // Update is called once per frame
    void Update()
    {
        if(turbulenceIntensity > 0f) {
            turbulenceIntensity -= Time.deltaTime / switchTime;

            if (turbulenceIntensity < 0f)
                turbulenceIntensity = 0f;

            vfxGraph.SetFloat(PropName.TurbulenceIntensity, turbulenceIntensity);
        }
    }

    private void SwitchMuscleGroup() {
        if (currentMuscle + 1 >= MuscleGroups.Count)
            return;

        currentMuscle++;
        UpdateModel();
        turbulenceIntensity = maxTurbulenceIntensity;
        Invoke("SwitchMuscleGroup", timePerMuscle);
    }

    private void UpdateModel() {
        vfxGraph.SetTexture(PropName.PositionMap, mapSets[currentMuscle].position);
        vfxGraph.SetVector3(PropName.PositionOffset, MuscleGroups[currentMuscle].transform.position);
        vfxGraph.SetVector3(PropName.Scale, mapSets[currentMuscle].scale);
    }

    private MapSet InitEffect(GameObject go) {
        MapSet mapSet = new MapSet();
        var modelTrans = go.transform;
        var meshAndTexs = GetMeshData(go);

        foreach (var (mesh, tex) in meshAndTexs) {
            mapSet = MeshToMap.ComputeMap(mesh, pointCountPerArea);
            mapSet.modelMainTex = tex;
            mapSet.scale = modelTrans.localScale * 1.1f;
        }

        return mapSet;
    }

    IEnumerable<(Mesh, Texture)> GetMeshData(GameObject model) {
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
