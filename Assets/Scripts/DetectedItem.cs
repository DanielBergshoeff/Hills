using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DetectedItem : MonoBehaviour
{
    public static List<VisualEffect> VisualEffects;

    private VisualEffect myVisualEffect;

    // Start is called before the first frame update
    void Awake()
    {
        if (VisualEffects == null)
            VisualEffects = new List<VisualEffect>();

        myVisualEffect = GetComponentInChildren<VisualEffect>();
        if (myVisualEffect != null)
            VisualEffects.Add(myVisualEffect);
    }

    public static void UpdateDetectedPosition(Vector3 pos) {
        foreach(VisualEffect ve in VisualEffects) {
            ve.SetVector3("DetecterPosition", pos);
        }
    }

    public void UpdateVisualEffect(Vector3 pos, float blend) {
        foreach(VisualEffect ve in VisualEffects) {
            ve.SetVector3("TargetPosition", pos);
            ve.SetFloat("Blend", blend);
        }
    }

    public void UpdateMyVisualEffect(Vector3 pos, float blend) {
        myVisualEffect.SetVector3("TargetPosition", pos);
        myVisualEffect.SetFloat("Blend", blend);
    }

    public VisualEffect GetVisualEffect() {
        return myVisualEffect;
    }
}
