using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Breathing))]
[CanEditMultipleObjects]
public class BreathingEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Stop")) {
            ((Breathing)target).Stop();
        }
    }
}
