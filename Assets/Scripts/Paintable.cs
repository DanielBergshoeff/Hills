using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Paintable : MonoBehaviour
{
    public GameObject PaintPrefabBlue;
    public GameObject PaintPrefabGreen;
    public GameObject PaintPrefabYellow;
    public GameObject PaintPrefabPink;
    public GameObject SpawnPosition;
    public VisualEffect VFXGraph;

    public bool VREnabled = false;
    public float BrushSize = 0.1f;
    public LayerMask PaintLayer;

    private GameObject currentPrefab;

    private float distance = 0f;

    // Start is called before the first frame update
    void Start()
    {
        PaintLayer = ~PaintLayer;
        currentPrefab = PaintPrefabBlue;
    }

    // Update is called once per frame
    void Update()
    {
        if (!VREnabled) {
            TryPaint();
            TrySelectColor();
        }
        else {
            TryPaintVR();
        }

    }

    private void TryPaintVR() {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.1f) {
            RaycastHit hit;
            if(Physics.Raycast(Wings.Instance.RightHand.transform.position, Wings.Instance.RightHand.transform.forward, out hit, 100f, PaintLayer)) {
                if (!hit.transform.CompareTag("Painting"))
                    return;

                /*var go = Instantiate(currentPrefab, hit.point, transform.rotation, transform);
                go.transform.localScale = Vector3.one * BrushSize;
                go.transform.position = go.transform.position + transform.up * distance;*/

                distance += Time.deltaTime * 0.03f;
                SpawnPosition.transform.position = hit.point + transform.up * distance;
            }
        }
    }

    private void TryPaint() {
        if (Input.GetMouseButton(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, PaintLayer)) {
                if (!hit.transform.CompareTag("Painting"))
                    return;

                /*
                var go = Instantiate(currentPrefab, hit.point, transform.rotation, transform);
                go.transform.localScale = Vector3.one * BrushSize;
                go.transform.position = go.transform.position + transform.up * distance;*/

                distance += Time.deltaTime * 0.03f;
                SpawnPosition.transform.position = hit.point + transform.up * distance;
                VFXGraph.SetFloat("SpawnRate", 60f);
            }
        }
        else {
            VFXGraph.SetFloat("SpawnRate", 0f);
        }
    }

    private void TrySelectColor() {
        if (Input.GetMouseButtonDown(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, PaintLayer)) {
                if (!hit.transform.CompareTag("Color"))
                    return;

                if (hit.transform.name == "Blue")
                    currentPrefab = PaintPrefabBlue;
                else if (hit.transform.name == "Green")
                    currentPrefab = PaintPrefabGreen;
                else if (hit.transform.name == "Pink")
                    currentPrefab = PaintPrefabPink;
                else if(hit.transform.name == "Yellow")
                    currentPrefab = PaintPrefabYellow;
            }
        }
    }
}
