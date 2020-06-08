using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensiks.SDK.UnityLibrary;
using Sensiks.SDK.Shared.SensiksDataTypes;
using System;

public class ScentManager : MonoBehaviour
{
    public static List<ScentObject> ScentObjects;
    public static ScentManager Instance;
    private Dictionary<Scent, float> scentToStrength;
    private Scent strongestScent;
    private float turnOffTimer = 0f;
    private float currentScentStrength = 0f;

    public float UpdateTime = 15f;
    public float ScentTimeMultiplier = 3f;

    private bool turningOn = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        StartCoroutine("UpdateScent", 1f);
    }

    private void Update() {
        if(currentScentStrength > 0f && !turningOn) {
            currentScentStrength -= Time.deltaTime * ScentTimeMultiplier;

            if(currentScentStrength <= 0f) {
                TurnOffScent();
            }
            else {
                SetScentStrength();
            }
        }
        else if(currentScentStrength < turnOffTimer && turningOn) {
            currentScentStrength += Time.deltaTime * ScentTimeMultiplier;

            if(currentScentStrength >= turnOffTimer) {
                turningOn = false;
            }
            else {
                SetScentStrength();
            }
        }
    }

    private void SetScentStrength() {
        SensiksManager.SetActiveScent(strongestScent, currentScentStrength);
    }


    /// <summary>
    /// Set the strongest scent and its strength
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateScent() {
        yield return new WaitForSeconds(UpdateTime);

        SetScent();
        SensiksManager.SetActiveScent(Scent.SMOKE, 0f);
        Scent strongestScent = Scent.NEWSCENT1;
        float strongestScentStrength = 0f;
        foreach (Scent s in Enum.GetValues(typeof(Scent))) {
            if (scentToStrength.ContainsKey(s)) {
                if (scentToStrength[s] > strongestScentStrength) {
                    strongestScent = s;
                    strongestScentStrength = scentToStrength[s];
                }
            }
        }

        strongestScentStrength = Mathf.Clamp(strongestScentStrength, 0f, 1f);
        SensiksManager.SetActiveScent(strongestScent, strongestScentStrength);

        turnOffTimer = strongestScentStrength / 2f;
        turningOn = true;

        StartCoroutine("UpdateScent");
    }

    private void TurnOffScent() {
        SensiksManager.SetActiveScent(strongestScent, 0f);
    }

    /// <summary>
    /// Update scent and scent strength based on scent objects
    /// </summary>
    private void SetScent() {
        scentToStrength = new Dictionary<Scent, float>();
        foreach(ScentObject so in ScentObjects) {
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
