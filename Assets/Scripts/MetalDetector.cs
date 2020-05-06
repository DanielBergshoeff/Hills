using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MetalDetector : MonoBehaviour
{
    public Material DetectionMaterial;
    public float Range = 5f;

    public VisualEffect DetectionEffect;
    public float DetectionTime = 3f;
    public Transform detectorPosition;

    private GameObject detectedObject;
    private DetectedItem detectedObjectItem;
    private float detectedTimer = 0f;

    private int detectedItems = 0;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Range)) {
            DetectionMaterial.SetVector("Vector3_E8D52375", hit.point);
            DetectionEffect.SetVector3("ScanPosition", hit.point);
            DetectedItem.UpdateDetectedPosition(hit.point);

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.5f && hit.transform.CompareTag("DetectedItem")) {
                if (detectedObject != hit.transform.gameObject) {
                    detectedObject = hit.transform.gameObject;
                    detectedObjectItem = detectedObject.GetComponent<DetectedItem>();
                    detectedTimer = 0f;
                }
                else {
                    detectedTimer += Time.deltaTime;
                    detectedObjectItem.UpdateVisualEffect(transform.position, detectedTimer / DetectionTime);
                    if (detectedTimer >= DetectionTime) {
                        DetectedItem.VisualEffects.Remove(detectedObjectItem.GetVisualEffect());
                        Destroy(detectedObject);
                        detectedItems++;
                    }
                }
            }
            else {
                detectedTimer = 0f;
                if (detectedObjectItem != null)
                    detectedObjectItem.UpdateVisualEffect(Vector3.zero, 0f);
            }
        }
        else {
            DetectionMaterial.SetVector("Vector3_E8D52375", transform.position + transform.forward * Range);
            DetectionEffect.SetVector3("ScanPosition", transform.position + transform.forward * Range);
            DetectedItem.UpdateDetectedPosition(transform.position + transform.forward * Range);
            detectedTimer = 0f;
        }

        DetectionEffect.SetVector3("ScannerPosition", transform.position);
    }
}
