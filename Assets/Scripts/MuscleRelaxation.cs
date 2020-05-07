using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class MuscleRelaxation : MonoBehaviour {
    public static class PropName {
        public const string PositionMap = "PositionMap";
        public const string PositionOffset = "PositionOffset";
        public const string Scale = "Scale";
        public const string Speed = "Speed";
        public const string TurbulenceIntensity = "TurbulenceIntensity";
        public const string FlexStrength = "FlexStrength";
        public const string FlexBlend = "FlexBlend";
    }

    private int pointCountPerArea = 100;
    private VisualEffect vfxGraph;
    private Animator pausedAnim;

    private int currentMuscle = -1;
    private List<GameObject> MuscleGroups;
    public float timePerMuscle = 10f;
    public float switchTime = 3f;
    public float turbulenceIntensity = 0f;
    public float maxTurbulenceIntensity = 3f;
    public float timeBetweenMuscles = 2.0f;
    public bool SendMessages = false;
    public GameObject ParentObject;
    private List<MapSet> mapSets;
    private List<CMObject> cMObjects;
    private bool instructionPhase = false;

    private CommunicationMessage currentMessage;


    // Start is called before the first frame update
    void Start() {
        vfxGraph = GetComponent<VisualEffect>();
        mapSets = new List<MapSet>();
        cMObjects = new List<CMObject>();
        MuscleGroups = new List<GameObject>();

        for (int i = 0; i < ParentObject.transform.childCount; i++) {
            MuscleGroups.Add(ParentObject.transform.GetChild(i).gameObject);
        }

        foreach (GameObject go in MuscleGroups) {
            CMObject cmo = go.GetComponent<CMObject>();
            cMObjects.Add(cmo);

            if (go.name == "Pause") {
                mapSets.Add(null);
                continue;
            }

            MapSet ms = InitEffect(go);
            mapSets.Add(ms);
        }
        
        vfxGraph.SetFloat(PropName.Speed, 1f / switchTime);
        SwitchMuscleGroup();
    }

    // Update is called once per frame
    void Update()
    {
        if (turbulenceIntensity > 0f && instructionPhase) {
            turbulenceIntensity -= (Time.deltaTime / switchTime) * maxTurbulenceIntensity;

            if (turbulenceIntensity < 0f) {
                turbulenceIntensity = 0f;
                vfxGraph.SetFloat(PropName.FlexStrength, 1f);
            }
            else {
                vfxGraph.SetFloat(PropName.FlexBlend, 1f - (turbulenceIntensity / maxTurbulenceIntensity));
            }

            vfxGraph.SetFloat(PropName.TurbulenceIntensity, turbulenceIntensity);
        }
    }

    private void SwitchMuscleGroup() {
        if (currentMuscle + 1 >= MuscleGroups.Count)
            return;

        currentMuscle++;

        if(mapSets[currentMuscle] != null)
            UpdateModel();

        if (SendMessages) {
            if (currentMessage != null)
                currentMessage.StartFade();

            if (cMObjects[currentMuscle] == null)
                return;

            currentMessage = CommunicationManager.Instance.DisplayMessage(gameObject, cMObjects[currentMuscle].MyMessageInstructions, cMObjects[currentMuscle].MyAudioClipInstructions, 0f, Vector3.up * 12.5f, 3f, false, 3f);
            switchTime = cMObjects[currentMuscle].MyAudioClipInstructions.length;
        }


        if (MuscleGroups[currentMuscle].name != "Pause") {
            Invoke("FlexMuscleGroup", switchTime);
            instructionPhase = true;
        }
        else {
            Invoke("SwitchMuscleGroup", switchTime);
        }
    }

    private void FlexMuscleGroup() {
        if (SendMessages) {
            if (currentMessage != null)
                currentMessage.StartFade();
            currentMessage = CommunicationManager.Instance.DisplayMessage(gameObject, cMObjects[currentMuscle].MyMessageHolding, cMObjects[currentMuscle].MyAudioClipHolding, 0f, Vector3.up * 12.5f, 3f, false, 3f);
        }

        instructionPhase = false;
        Invoke("ReleaseMuscleGroup", cMObjects[currentMuscle].MyAudioClipHolding.length);
    }

    private void ReleaseMuscleGroup() {
        currentMessage.StartFade();

        if (SendMessages)
            currentMessage = CommunicationManager.Instance.DisplayMessage(gameObject, cMObjects[currentMuscle].MyMessageRelease, cMObjects[currentMuscle].MyAudioClipRelease, 0f, Vector3.up * 12.5f, 3f, false, 3f);

        turbulenceIntensity = maxTurbulenceIntensity;
        vfxGraph.SetFloat(PropName.TurbulenceIntensity, turbulenceIntensity);
        vfxGraph.SetFloat(PropName.FlexStrength, 0f);
        vfxGraph.SetFloat(PropName.FlexBlend, 0f);

        Invoke("SwitchMuscleGroup", cMObjects[currentMuscle].MyAudioClipRelease.length + timeBetweenMuscles);
    }

    private void UpdateModel() {
        vfxGraph.SetTexture(PropName.PositionMap, mapSets[currentMuscle].position);
        vfxGraph.SetVector3(PropName.PositionOffset, MuscleGroups[currentMuscle].transform.position - transform.position);
        vfxGraph.SetVector3(PropName.Scale, mapSets[currentMuscle].scale);
    }

    private MapSet InitEffect(GameObject go) {
        MapSet mapSet = new MapSet();
        var modelTrans = go.transform;
        var meshAndTexs = GetMeshData(go);

        foreach (var (mesh, tex) in meshAndTexs) {
            mapSet = MeshToMap.ComputeMap(mesh, pointCountPerArea);
            mapSet.modelMainTex = tex;
            mapSet.scale = modelTrans.parent.localScale * 1.1f;
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
