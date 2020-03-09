using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensiks.SDK.UnityLibrary;
using Sensiks.SDK.Shared.SensiksDataTypes;
using System;

public class ScentManager : MonoBehaviour
{
    public static List<ScentObject> scentObjects;
    private Dictionary<Scent, float> scentToStrength;
    public float refreshTime = 1.0f;
    public float onTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowScent());
    }

    // Update is called once per frame
    void Update()
    {
        SetScent();
    }

    private IEnumerator ShowScent() {
        yield return new WaitForSeconds(onTime);
        SensiksManager.SetActiveScent(Scent.SMOKE, 0f);

        yield return new WaitForSeconds(refreshTime);
        Scent strongestScent = Scent.NEWSCENT1;
        float strongestScentStrength = 0f;
        foreach (Scent s in Enum.GetValues(typeof(Scent))) {
            if (scentToStrength.ContainsKey(s)) {
                Debug.Log(s + ": " + scentToStrength[s]);
                if (scentToStrength[s] > strongestScentStrength) {
                    strongestScent = s;
                    strongestScentStrength = scentToStrength[s];
                }
            }
        }
        SensiksManager.SetActiveScent(strongestScent, strongestScentStrength);
        StartCoroutine(ShowScent());
    }

    private void SetScent() {
        scentToStrength = new Dictionary<Scent, float>();
        foreach(ScentObject so in scentObjects) {
            float sqrDistance = (so.transform.position - Camera.main.transform.position).sqrMagnitude;
            if (sqrDistance > so.MaxDistance * so.MaxDistance)
                continue;
            float scentStrength = 1f - sqrDistance / (so.MaxDistance * so.MaxDistance);
            scentStrength *= so.Strength;

            if (scentToStrength.ContainsKey(so.ObjectScent))
                scentToStrength[so.ObjectScent] += scentStrength;
            else
                scentToStrength.Add(so.ObjectScent, scentStrength);
        }
    }
}
