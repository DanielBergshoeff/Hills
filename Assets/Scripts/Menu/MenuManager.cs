using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuManager : MonoBehaviour
{
    public GameObject ColorMenuPrefab;
    public GameObject ColorMenu;
    public GameObject TutorialPosition;
    public Wings wings;
    public float MenuDistance = 0.1f;

    private MenuOption[] menuOptions;
    private bool menuEnabled = false;
    private bool painting = true;


    public bool Tutorial = false;
    public VRTK_InteractGrab RightHandGrab;
    public VRTK_Pointer RightHandPointer;
    public VRTK_Pointer LeftHandPointer;

    private int tutorialPart = 0;
    private CommunicationMessage currentMessage;

    private Tool myTool;

    public enum Tool {
        None,
        Paintbrush,
        FireflyCatcher,
        Treasurefinder
    }

    private void Awake() {
        RightHandGrab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
        RightHandGrab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUnGrabObject);

        if (Tutorial) {
            RightHandPointer.enabled = false;
            LeftHandPointer.enabled = false;
            wings.RotationEnabled = false;
        }
    }

    private void Start() {
        StartTutorial();
    }

    private void StartTutorial() {
        wings.RotationEnabled = true;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "To rotate, gently nudge the thumbstick on your right controller into the desired direction. Try it now.", null, 0f, Vector3.up * 1f, 2f, true, 5f);
    }

    private void TeleportTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "To teleport, press the thumbstick on the left controller to make an arch appear first. Aim for a stepping stone on the path, until the arch turns green", null, 0f, Vector3.up * 1f, 2f, true, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (painting)
            PaintingMenu();

        if (!Tutorial)
            return;

        switch (tutorialPart) {
            case 0:
                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch)) {
                    TeleportTutorial();
                }
                break;
            default:
                break;

        }
    }

    private void StartPainting() {
        painting = true;
    }

    void OnGrabObject(object sender, ObjectInteractEventArgs e) {
        if (!e.target.CompareTag("Tool"))
            return;

        Debug.Log("Tool grabbed");

        if (e.target.name == "Paintbrush") {
            myTool = Tool.Paintbrush;
            Paintable.Instance.PaintingEnabled = true;
        }
        else if (e.target.name == "FireflyCatcher") {
            myTool = Tool.FireflyCatcher;
        }
        else if (e.target.name == "TreasureFinder") {
            myTool = Tool.Treasurefinder;
        }
    }

    private void OnUnGrabObject(object sender, ObjectInteractEventArgs e) {
        if (!e.target.CompareTag("Tool"))
            return;

        if(e.target.name == "Paintbrush") {
            Paintable.Instance.PaintingEnabled = false;
        }

        Debug.Log("Tool ungrabbed");

        myTool = Tool.None;
    }

    private void PaintingMenu() {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch)) {
            Debug.Log("Pressed");

            if (myTool == Tool.None)
                return;
            
            if (myTool == Tool.Paintbrush) {
                ColorMenu = Instantiate(ColorMenuPrefab, transform);
                Vector3 forward = new Vector3(Wings.Instance.RightHand.forward.x, 0f, Wings.Instance.RightHand.forward.z).normalized;
                ColorMenu.transform.position = Wings.Instance.RightHand.position + forward * MenuDistance;
                ColorMenu.transform.rotation = Quaternion.LookRotation(forward);
                menuOptions = ColorMenu.GetComponentsInChildren<MenuOption>();
            }

            menuEnabled = true;
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch)) {
            Debug.Log("Released");

            if (!menuEnabled)
                return;

            foreach (MenuOption mo in menuOptions) {
                if (!mo.Selected)
                    continue;

                mo.Select();
            }

            Destroy(ColorMenu);
            menuEnabled = false;
        }
    }
}
