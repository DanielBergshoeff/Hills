using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensiks.SDK.Shared.SensiksDataTypes;
using Sensiks.SDK.UnityLibrary;

public class HeatManager : MonoBehaviour
{
    public static List<HeatObject> HeatObjects;
    public static HeatManager Instance;

    public float UpdateTime = 15f;
    public float MaxUptimePercentage = 0.5f;

    private float heatFront = 0f;
    private float heatLeft = 0f;
    private float heatRight = 0f;

    void Awake()
    {
        Instance = this;
        Invoke("UpdateHeat", UpdateTime);
    }

    public static void AddHeatObject(HeatObject ho) {
        if (HeatManager.HeatObjects == null)
            HeatManager.HeatObjects = new List<HeatObject>();

        HeatObjects.Add(ho);
        Instance.ProcessHeatObject(ho);
    }

    private void Update() {
        if (heatFront > 0f) 
            heatFront -= Time.deltaTime / UpdateTime * MaxUptimePercentage;
        if (heatLeft > 0f) 
            heatLeft -= Time.deltaTime / UpdateTime * MaxUptimePercentage;
        if (heatRight > 0f) 
            heatRight -= Time.deltaTime / UpdateTime * MaxUptimePercentage;

        SetHeat();
    }

    public void UpdateHeat() {
        foreach (HeatObject ho in HeatObjects) {
            ProcessHeatObject(ho);
        }

        heatFront = Mathf.Clamp(heatFront, 0f, 1f);
        heatLeft = Mathf.Clamp(heatLeft, 0f, 1f);
        heatRight = Mathf.Clamp(heatRight, 0f, 1f);

        Invoke("UpdateHeat", UpdateTime);
    }

    private void ProcessHeatObject(HeatObject ho) {
        if (Wings.Instance == null)
            return;

        Vector3 heading = ho.transform.position - Wings.Instance.transform.parent.position;
        heading = new Vector3(heading.x, 0f, heading.z);
        if (heading.sqrMagnitude > ho.Range * ho.Range)
            return;

        if (ho.PositionBased) {
            Vector3 player = new Vector3(Wings.Instance.transform.parent.forward.x, 0f, Wings.Instance.transform.parent.forward.z);

            float dot = heading.x * player.x + heading.z * player.z;
            float det = heading.x * player.z - heading.z * player.x;
            float atanAngle = Mathf.Atan2(det, dot);
            atanAngle = atanAngle / Mathf.PI * 180f;

            if (Mathf.Abs(atanAngle) < 45f) {
                heatFront += ho.Heat;
                heatLeft += ho.Heat;
                heatRight += ho.Heat;
            }
            else if (Mathf.Abs(atanAngle) > 135f) {
                heatLeft += ho.Heat;
                heatRight += ho.Heat;
            }
            else if (atanAngle < -45f)
                heatLeft += ho.Heat;
            else if (atanAngle > 45f)
                heatRight += ho.Heat;

        }
        else {
            heatFront += ho.Heat;
            heatLeft += ho.Heat;
            heatRight += ho.Heat;
        }
    }

    private void SetHeat() {
        SensiksManager.SetHeaterIntensity(HeaterPosition.FRONT, Mathf.Clamp(heatFront, 0f, 1f));
        SensiksManager.SetHeaterIntensity(HeaterPosition.LEFT, Mathf.Clamp(heatLeft, 0f, 1f));
        SensiksManager.SetHeaterIntensity(HeaterPosition.RIGHT, Mathf.Clamp(heatRight, 0f, 1f));
    }

    private void OnDrawGizmosSelected() {
        if (Camera.main == null)
            return;

        Vector3 posFrontLeft = Camera.main.transform.parent.parent.position + Camera.main.transform.parent.parent.forward;
        Gizmos.DrawSphere(posFrontLeft, 0.1f + heatFront);
        Vector3 posFrontRight = Camera.main.transform.parent.parent.position + Camera.main.transform.parent.parent.right;
        Gizmos.DrawSphere(posFrontRight, 0.1f + heatRight);
        Vector3 posBehindLeft = Camera.main.transform.parent.parent.position - Camera.main.transform.parent.parent.right;
        Gizmos.DrawSphere(posBehindLeft, 0.1f + heatLeft);
    }
}
