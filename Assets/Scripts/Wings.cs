using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class Wings : MonoBehaviour {
    public static Wings Instance { get; private set; }

    [Header("References")]
    public Transform LeftHand;
    public Transform RightHand;
    public Transform Head;

    public GameObject LeftHandTutorial;
    public GameObject RightHandTutorial;

    public bool Testing;
    public bool Flying = false;
    public bool RotationEnabled = true;
    public float RotationRatchet = 45f;

    private Rigidbody myRigidbody;
    private GameObject FeatherParent;
    private bool ReadyToSnapTurn = true;
    public bool RotationEitherThumbstick = false;
    
    private void Awake() {
        myRigidbody = transform.parent.GetComponent<Rigidbody>();

        Instance = this;
    }

    private void Start() {
        //HeatManager.Instance.UpdateHeat();
        WindManager.Instance.UpdateWind();
    }

    public void ToggleTutorialHands(bool enabled) {
        LeftHandTutorial.SetActive(enabled);
        RightHandTutorial.SetActive(enabled);

        LeftHand.gameObject.SetActive(!enabled);
        RightHand.gameObject.SetActive(!enabled);
    }

    private void UpdateRotations() {
        Vector3 euler = transform.parent.rotation.eulerAngles;
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.LTouch) ||
                    (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))) {
            if (ReadyToSnapTurn) {
                euler.y -= RotationRatchet;
                WindManager.Instance.UpdateWind();
                //HeatManager.Instance.UpdateHeat();
                ReadyToSnapTurn = false;
                Debug.Log("Rotate!");
            }
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.LTouch) ||
            (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))) {
            if (ReadyToSnapTurn) {
                euler.y += RotationRatchet;
                WindManager.Instance.UpdateWind();
                //HeatManager.Instance.UpdateHeat();
                ReadyToSnapTurn = false;
                Debug.Log("Rotate!");
            }
        }
        else {
            ReadyToSnapTurn = true;
        }

        transform.parent.rotation = Quaternion.Euler(euler);
    }

    // Update is called once per frame
    void Update() {
        if(RotationEnabled)
            UpdateRotations();
        CheckForExplanation();
    }

    private void CheckForExplanation() {
        if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
            RaycastHit hit;
            if(Physics.Raycast(RightHand.position, RightHand.forward, out hit, 100f)) {
                if(!(hit.transform.CompareTag("SmallObject") || hit.transform.CompareTag("Tool") || hit.transform.CompareTag("Interactable")))
                    return;

                Explanation ex = hit.transform.GetComponent<Explanation>();
                if (ex == null)
                    return;

                ex.ShowMessage();
            }
        }
    }
}
