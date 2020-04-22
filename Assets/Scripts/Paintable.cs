using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Paintable : MonoBehaviour
{
    public static ColorEvent colorEvent;
    public static SizeEvent sizeEvent;
    public static VisualEffect sharedVfxGraph;
    public GameObject SpawnPosition;
    public VisualEffect VFXGraph;
    public VisualEffect Aim;

    public bool VREnabled = false;
    public float BrushSize = 0.1f;
    public LayerMask PaintLayer;

    private float distance = 0f;

    // Start is called before the first frame update
    void Start()
    {
        PaintLayer = ~PaintLayer;
        colorEvent = new ColorEvent();
        colorEvent.AddListener(ChangeColor);

        sizeEvent = new SizeEvent();
        sizeEvent.AddListener(ChangeSize);


        if (sharedVfxGraph == null)
            sharedVfxGraph = VFXGraph;
    }

    // Update is called once per frame
    void Update()
    {
        if (!VREnabled) {
            TryPaint();
        }
        else {
            TryPaintVR();
        }

    }

    public static void ChangeColor(Color color) {
        sharedVfxGraph.SetVector4("New Color", color);
    }

    public static void ChangeSize(float size) {
        sharedVfxGraph.SetFloat("Size", size);
    }

    private void TryPaintVR() {
        RaycastHit hit;
        if (Physics.Raycast(Wings.Instance.RightHand.transform.position + Wings.Instance.RightHand.transform.forward * 0.25f, Wings.Instance.RightHand.transform.forward, out hit, 100f, PaintLayer)) {
            if (!hit.transform.CompareTag("Painting"))
                return;

            Aim.SetVector3("SpawnPosition", hit.point + transform.up * 0.05f);
            SpawnPosition.transform.position = hit.point + transform.up * distance;

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.1f) {

                distance += Time.deltaTime * 0.1f;
                VFXGraph.SetFloat("SpawnRate", 60f);
            }
            else {
                VFXGraph.SetFloat("SpawnRate", 0f);
            }
        }
        else {
            VFXGraph.SetFloat("SpawnRate", 0f);
        }
    }

    private void TryPaint() {
        if (Input.GetMouseButton(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, PaintLayer)) {
                if (!hit.transform.CompareTag("Painting"))
                    return;
                
                distance += Time.deltaTime * 0.1f;
                SpawnPosition.transform.position = hit.point + transform.up * distance;
                VFXGraph.SetFloat("SpawnRate", 60f);
            }
            else {
                VFXGraph.SetFloat("SpawnRate", 0f);
            }
        }
        else {
            VFXGraph.SetFloat("SpawnRate", 0f);
        }
    }
}

public class ColorEvent : UnityEvent<Color> { }

public class SizeEvent : UnityEvent<float> { }
