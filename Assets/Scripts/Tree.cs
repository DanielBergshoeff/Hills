﻿using System.Collections;
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
        VfxGraph.SetVector3("Center", Camera.main.transform.position);
    }

    public static void UpdateTree(float attractionForce, float colorIntensity, float amtOfLeaves) {
        foreach(Tree tree in AllTrees) {
            tree.VfxGraph.SetFloat("AttractionForce", attractionForce);
            tree.VfxGraph.SetFloat("ColorIntensity", colorIntensity);
            tree.VfxGraph.SetFloat("SpawnRate", amtOfLeaves);
        }
    }
}
