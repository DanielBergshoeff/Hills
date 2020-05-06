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


    public static bool Tutorial = false;
    public VRTK_InteractGrab RightHandGrab;
    public VRTK_Pointer RightHandPointer;
    public VRTK_Pointer LeftHandPointer;
    public VRTK_HeightAdjustTeleport Teleporter;

    private int tutorialPart = 0;
    private CommunicationMessage currentMessage;
    private Vector3 startPosition;

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
        Teleporter.Teleported += new TeleportEventHandler(OnTeleport);

        if (Tutorial) {
            RightHandPointer.enabled = false;
            LeftHandPointer.enabled = false;
            wings.RotationEnabled = false;
        }
    }

    private void OnTeleport(object sender, DestinationMarkerEventArgs e) {
        WindManager.Instance.UpdateWind();
        HeatManager.Instance.UpdateHeat();

        if(Breathing.Instance != null) {
            if((Breathing.Instance.transform.position - wings.transform.position).sqrMagnitude > 60f * 60f) {
                Destroy(Breathing.Instance.gameObject);
            }
        }
    }

    private void Start() {
        if(Tutorial)
            StartTutorial();
    }

    private void StartTutorial() {
        wings.RotationEnabled = true;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "To rotate, gently nudge the thumbstick on your right controller into the desired direction. Try it now.", null, 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void PickupTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        RightHandPointer.enabled = true;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "To pick up items from a distance, press the thumbstick on your right controller to make a ray appear from it.", null, 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void PickupTutorialPart2() {
        tutorialPart++;
        currentMessage.StartFade();
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "Aim at the brush on the floor and squeeze the right controller's middle finger to pick it up.", null, 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void TeleportTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        LeftHandPointer.enabled = true;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "To teleport, press the thumbstick on the left controller to make an arch appear first.", null, 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void TeleportPart2Tutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        startPosition = wings.transform.parent.position;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "Aim for a stepping stone on the path, until the arch turns green. Release the button to teleport.", null, 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void EndTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        Tutorial = false;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, "Good job, you are now in control!", null, 10f, Vector3.up * 1f, 2f, true, 4f);
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
                    PickupTutorial();
                }
                break;
            case 1:
                if(OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch)) {
                    PickupTutorialPart2();
                }
                break;
            case 2:
                if(myTool == Tool.Paintbrush) {
                    TeleportTutorial();
                }
                break;
            case 3: 
                if(OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)){
                    TeleportPart2Tutorial();
                }
                break;
            case 4:
                if((startPosition - wings.transform.position).sqrMagnitude > 0.5f) {
                    EndTutorial();
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
        VicinityMessage vm = e.target.GetComponent<VicinityMessage>();
        if(vm != null) {
            vm.StopMessage();
            Destroy(vm);
        }

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
