using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using VRTK;

public class Wings : MonoBehaviour {
    public static Wings Instance { get; private set; }

    [Header("References")]
    public Transform LeftHand;
    public Transform RightHand;
    public Transform Head;

    public GameObject LeftHandNormal;
    public GameObject RightHandNormal;


    public GameObject LeftHandTutorial;
    public GameObject RightHandTutorial;

    public GameObject LeftHandMessagePosition;
    public GameObject RightHandMessagePosition;

    [Header("Tutorial")]
    public Material HighlightMaterial;
    public Material StandardMaterial;

    public Renderer ThumbstickLeftRenderer;
    public Renderer ButtonOneLeftRenderer;

    public Renderer ButtonOneRightRenderer;
    public Renderer IndexRightRenderer;
    public Renderer HandRightRenderer;

    public LayerMask ExplanationMask;

    private Vector3 startPosition;
    
    public bool RotationEnabled = true;
    public float RotationRatchet = 45f;

    private bool ReadyToSnapTurn = true;
    public bool RotationEitherThumbstick = false;

    private List<CommunicationMessage> messagesRight;
    private List<CommunicationMessage> messagesLeft;
    private bool holdingObject = false;
    
    private void Awake() {
        startPosition = transform.parent.position;
        Instance = this;
    }

    public void ResetPosition() {
        transform.parent.position = startPosition;
    }

    private void Start() {
        //HeatManager.Instance.UpdateHeat();
        WindManager.Instance.UpdateWind();
    }

    public void ReverseHighlight(Renderer r) {
        r.material = StandardMaterial;
    }

    public void HighlightButton(Renderer r) {
        r.material = HighlightMaterial;
    }

    public void ToggleTutorialHands(bool enabled) {
        LeftHandTutorial.SetActive(enabled);
        RightHandTutorial.SetActive(enabled);

        LeftHandNormal.SetActive(!enabled);
        RightHandNormal.SetActive(!enabled);
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
        CheckForButtonMessages();
    }

    private void CheckForExplanation() {
        if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
            RaycastHit hit;
            if(Physics.Raycast(RightHand.position, RightHand.forward, out hit, 100f, ExplanationMask)) {
                if(!(hit.transform.CompareTag("SmallObject") || hit.transform.CompareTag("Tool") || hit.transform.CompareTag("Interactable")))
                    return;

                Explanation ex = hit.transform.GetComponent<Explanation>();
                if (ex == null)
                    return;

                ex.ShowMessage();

                VicinityMessage vm = hit.transform.GetComponent<VicinityMessage>();
                if(vm != null)
                    vm.StopMessage();
            }
        }
    }

    private void CheckForButtonMessages() {
        if(OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
            ClearMessagesRight();
            ButtonAMessages();
        }
        else if(OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
            ClearMessagesRight();
            if (holdingObject) {
                CommunicationMessage msg = CommunicationManager.Instance.DisplayButtonMessage(RightHandMessagePosition, "Middle finger", "Release to drop", null, 0f, Vector3.up * 0.04f, 0.03f, true, 9f, 7f, 0.1f);
                messagesRight.Add(msg);
            }
        }

        if(OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch)) {
            ClearMessagesLeft();
            CommunicationMessage msg = CommunicationManager.Instance.DisplayButtonMessage(LeftHandMessagePosition, "X button", "Release to teleport", null, 0f, Vector3.up * 0.1f, 0.03f, true, 9f, 7f, -0.1f);
            messagesLeft.Add(msg);
        }
        else if(OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch)) {
            ClearMessagesLeft();
        }
    }

    public void OnPickup(object sender, ObjectInteractEventArgs e) {
        holdingObject = true;
        ClearMessagesRight();
        CommunicationMessage msg3 = CommunicationManager.Instance.DisplayButtonMessage(RightHandMessagePosition, "A button", "Release to remove line", null, 0f, Vector3.up * 0.1f, 0.03f, true, 9f, 6f, 0.1f);
        CommunicationMessage msg = CommunicationManager.Instance.DisplayButtonMessage(RightHandMessagePosition, "Middle finger", "Release to drop", null, 0f, Vector3.up * 0.04f, 0.03f, true, 9f, 7f, 0.1f);
        messagesRight.Add(msg);
        messagesRight.Add(msg3);
    }

    public void OnDrop(object sender, ObjectInteractEventArgs e) {
        holdingObject = false;
        ClearMessagesRight();

        if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
            ButtonAMessages();
        }
    }

    private void ButtonAMessages() {
        CommunicationMessage msg = CommunicationManager.Instance.DisplayButtonMessage(RightHandMessagePosition, "Index finger", "Show explanation", null, 0f, Vector3.up * 0.1f, 0.03f, true, 9f, 7f, 0.1f);
        CommunicationMessage msg2 = CommunicationManager.Instance.DisplayButtonMessage(RightHandMessagePosition, "Middle finger", "Pickup item", null, 0f, Vector3.up * 0.04f, 0.03f, true, 9f, 7f, 0.1f);
        messagesRight.Add(msg);
        messagesRight.Add(msg2);
    }

    public void OnExplanation(string title) {

    }

    private void ClearMessagesRight() {
        if (messagesRight != null) {
            foreach (CommunicationMessage msg in messagesRight) {
                msg.StartFade();
            }
        }

        messagesRight = new List<CommunicationMessage>();
    }

    private void ClearMessagesLeft() {
        if (messagesLeft != null) {
            foreach (CommunicationMessage msg in messagesLeft) {
                msg.StartFade();
            }
        }

        messagesLeft = new List<CommunicationMessage>();
    }
}
