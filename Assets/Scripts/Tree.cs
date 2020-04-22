using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Tree : MonoBehaviour
{
    public static List<Tree> AllTrees;
    public VisualEffect VfxGraph;

    // Start is called before the first frame update
    void Start()
    {
        if (AllTrees == null)
            AllTrees = new List<Tree>();

        AllTrees.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Camera.main != null)
            VfxGraph.SetVector3("Center", Camera.main.transform.position);
    }

    public static void UpdateTree(float attractionForce, float colorIntensity, float amtOfLeaves) {
        if (AllTrees == null)
            return;

        foreach(Tree tree in AllTrees) {
            Vector3 heading = tree.transform.position - Camera.main.transform.position;
            if (heading.sqrMagnitude < 30f * 30f) {
                tree.VfxGraph.SetFloat("AttractionForce", attractionForce);
                tree.VfxGraph.SetFloat("ColorIntensity", colorIntensity);
                tree.VfxGraph.SetFloat("SpawnRate", amtOfLeaves);
            }
            else {
                tree.VfxGraph.SetFloat("AttractionForce", 0f);
                tree.VfxGraph.SetFloat("ColorIntensity", 1f);
                tree.VfxGraph.SetFloat("SpawnRate", 0);
            }
        }
    }
}
