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
        StartCoroutine("SetHeat", 1f);
    }

    public void UpdateHeat() {
        if (Wings.Instance == null)
            return;

        foreach (HeatObject ho in HeatObjects) {
            Vector3 heading = ho.transform.position - Wings.Instance.transform.parent.position;
            heading = new Vector3(heading.x, 0f, heading.z);
            if (heading.sqrMagnitude > ho.Range * ho.Range)
                continue;

            heatFront = 0f;
            heatLeft = 0f;
            heatRight = 0f;

            if (ho.PositionBased) {
                Vector3 player = new Vector3(Wings.Instance.transform.parent.forward.x, 0f, Wings.Instance.transform.parent.forward.z);

                float dot = heading.x * player.x + heading.z * player.z;
                float det = heading.x * player.z - heading.z * player.x;
                float atanAngle = Mathf.Atan2(det, dot);
                atanAngle = atanAngle / Mathf.PI * 180f;

                if (Mathf.Abs(atanAngle) < 45f) {
                    heatFront = ho.Heat;
                    heatLeft = ho.Heat;
                    heatRight = ho.Heat;
                }
                else if (Mathf.Abs(atanAngle) > 135f) {
                    heatLeft = ho.Heat;
                    heatRight = ho.Heat;
                }
                else if (atanAngle < -45f)
                    heatLeft = ho.Heat;
                else if (atanAngle > 45f)
                    heatRight = ho.Heat;

            }
            else {
                heatFront = ho.Heat;
                heatLeft = ho.Heat;
                heatRight = ho.Heat;
            }
            break;
        }
    }

    private IEnumerator SetHeat() {
        SensiksManager.SetHeaterIntensity(HeaterPosition.FRONT, heatFront);
        SensiksManager.SetHeaterIntensity(HeaterPosition.LEFT, heatLeft);
        SensiksManager.SetHeaterIntensity(HeaterPosition.RIGHT, heatRight);


        StartCoroutine(TurnOffHeat(heatFront * UpdateTime * MaxUptimePercentage, HeaterPosition.FRONT));
        StartCoroutine(TurnOffHeat(heatLeft * UpdateTime * MaxUptimePercentage, HeaterPosition.LEFT));
        StartCoroutine(TurnOffHeat(heatRight * UpdateTime * MaxUptimePercentage, HeaterPosition.RIGHT));

        yield return new WaitForSeconds(UpdateTime);

        StartCoroutine("SetHeat");
    }

    private IEnumerator TurnOffHeat(float timeToWait, HeaterPosition position) {
        yield return new WaitForSeconds(timeToWait);

        SensiksManager.SetHeaterIntensity(position, 0f);
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
