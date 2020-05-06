using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensiks.SDK.Shared.SensiksDataTypes;
using Sensiks.SDK.UnityLibrary;

public class WindManager : MonoBehaviour {
    public static List<WindObject> WindObjects;
    public static WindManager Instance;

    private float strengthFrontLeft = 0f;
    private float strengthFrontRight = 0f;
    private float strengthBehindLeft = 0f;
    private float strengthBehindRight = 0f;

    // Start is called before the first frame update
    void Start() {
        Instance = this;
    }

    public void UpdateWind() {
        strengthFrontLeft = 0f;
        strengthFrontRight = 0f;
        strengthBehindLeft = 0f;
        strengthBehindRight = 0f;

        foreach (WindObject wo in WindObjects) {
            Vector3 heading = wo.transform.position - Wings.Instance.transform.parent.position;
            heading = new Vector3(heading.x, 0f, heading.z);

            if (heading.sqrMagnitude > wo.MaxDistance * wo.MaxDistance)
                continue;

            Vector3 player = new Vector3(Wings.Instance.transform.parent.forward.x, 0f, Wings.Instance.transform.parent.forward.z);
            float angle = Vector3.Angle(heading.normalized, player);

            float dot = heading.x * player.x + heading.z * player.z;
            float det = heading.x * player.z - heading.z * player.x;
            float atanAngle = Mathf.Atan2(det, dot);
            atanAngle = atanAngle / Mathf.PI * 180f;

            //Debug.Log(atanAngle);
            if (atanAngle >= -45f && atanAngle <= 45) {
                //Front left and front right
                //Debug.Log("Front left and front right");

                //Front left
                strengthFrontLeft += (1f - Mathf.Abs(atanAngle + 45f) / 90f) * wo.Strength;

                //Front right
                strengthFrontRight += (1f - Mathf.Abs(atanAngle - 45f) / 90f) * wo.Strength;
            }
            else if (atanAngle >= 45f && atanAngle <= 135f) {
                //Front right and behind right
                //Debug.Log("Front right and behind right");

                //Front right
                strengthFrontRight += (1f - Mathf.Abs(atanAngle - 45f) / 90f) * wo.Strength;

                //Behind right
                strengthBehindRight += (1f - Mathf.Abs(atanAngle - 135f) / 90f) * wo.Strength;

            }
            else if (atanAngle <= -45f && atanAngle >= -135f) {
                //Front left and behind left
                //Debug.Log("Front left and behind left");

                //Front left
                strengthFrontLeft += (1f - Mathf.Abs(atanAngle + 45f) / 90f) * wo.Strength;

                //Behind left
                strengthBehindLeft += (1f - Mathf.Abs(atanAngle + 135f) / 90f) * wo.Strength;
            }
            else {
                //Behind left and behind right
                //Debug.Log("Behind left and behind right");

                if (atanAngle > 0f) {
                    //Behind left
                    strengthBehindLeft += (1f - (90 - (atanAngle - 135f)) / 90f) * wo.Strength;

                    //Behind right
                    strengthBehindRight += (1f - (atanAngle - 135f) / 90f) * wo.Strength;
                }
                else {
                    //Behind left
                    strengthBehindLeft += (1f - (-135f - atanAngle) / 90f) * wo.Strength;

                    //Behind right
                    strengthBehindRight += (1f - (90 - (-135f - atanAngle)) / 90f) * wo.Strength;
                }

            }

            /*
            Debug.Log("Strength front left: " + strengthFrontLeft);
            Debug.Log("Strength front right: " + strengthFrontRight);
            Debug.Log("Strength behind left: " + strengthBehindLeft);
            Debug.Log("Strength behind right: " + strengthBehindRight); */

            SensiksManager.SetFanIntensity(FanPosition.FRONT_LEFT, Mathf.Clamp(strengthFrontLeft, 0f, 1f));
            SensiksManager.SetFanIntensity(FanPosition.FRONT_RIGHT, Mathf.Clamp(strengthFrontRight, 0f, 1f));
            SensiksManager.SetFanIntensity(FanPosition.REAR_LEFT, Mathf.Clamp(strengthBehindLeft, 0f, 1f));
            SensiksManager.SetFanIntensity(FanPosition.REAR_RIGHT, Mathf.Clamp(strengthBehindRight, 0f, 1f));
        }
    }

    private void OnDrawGizmos() {
        /*
        Vector3 posFrontLeft = Camera.main.transform.parent.parent.position + Camera.main.transform.parent.parent.forward - Camera.main.transform.parent.parent.right;
        Gizmos.DrawSphere(posFrontLeft, 0.1f + strengthFrontLeft);
        Vector3 posFrontRight = Camera.main.transform.parent.parent.position + Camera.main.transform.parent.parent.forward + Camera.main.transform.parent.parent.right;
        Gizmos.DrawSphere(posFrontRight, 0.1f + strengthFrontRight);
        Vector3 posBehindLeft = Camera.main.transform.parent.parent.position - Camera.main.transform.parent.parent.forward - Camera.main.transform.parent.parent.right;
        Gizmos.DrawSphere(posBehindLeft, 0.1f + strengthBehindLeft);
        Vector3 posBehindRight = Camera.main.transform.parent.parent.position - Camera.main.transform.parent.parent.forward + Camera.main.transform.parent.parent.right;
        Gizmos.DrawSphere(posBehindRight, 0.1f + strengthBehindRight);*/
    }
}
