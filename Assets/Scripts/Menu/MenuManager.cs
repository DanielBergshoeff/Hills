using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public static bool Dutch = false;

    public GameObject ColorMenuPrefab;
    public GameObject ColorMenu;
    public GameObject TutorialPosition;
    public Wings wings;
    public float MenuDistance = 0.1f;
    public GameObject PaintingCanvas;
    public float StandardAmountOfLeaves = 1f;

    public LineRenderer menuLineRenderer;

    private MenuOption[] menuOptions;
    private MenuOption selectedMenu;
    private bool menuEnabled = false;
    private bool painting = true;


    public static bool Tutorial = false;
    public VRTK_InteractGrab RightHandGrab;
    public VRTK_Pointer RightHandPointer;
    public VRTK_Pointer LeftHandPointer;
    public VRTK_HeightAdjustTeleport Teleporter;

    private int tutorialPart = 0;
    private CommunicationMessage currentMessage;
    private Vector3 previousPosition;
    private Vector3 startPosition;

    private Tool myTool;

    private string[] englishTutorial = new string[] {
        "To rotate, gently nudge the thumbstick on your left controller into the desired direction. Try it now.",
        "To pick up items from a distance, press the A button on your right controller to make a ray appear from it.",
        "Aim at the brush on the floor and squeeze the right controller's middle finger to pick it up.",
        "To teleport, press the X button on the left controller to make an arch appear first.",
        "Aim for a stepping stone on the path, until the arch turns green. Release the button to teleport.",
        "Good job, now you are in control!"
    };

    private string[] dutchTutorial = new string[] {
        "Om te roteren, kan je de joystick op je linker controller zachtjes in de gewenste richting duwen. Probeer het nu.",
        "Om objecten van een afstand op te pakken, druk je op de A knop op je rechter controller om een straal te laten verschijnen.",
        "Richt met de straal op de kwast en knijp met je middelvinger de onderste knop in om de kwast op te pakken.",
        "Om te teleporteren kan je de X knop op je linker controller indrukken om een boog te laten verschijnen.",
        "Richt vervolgens op een steen op het pad, tot de boog groen wordt. Laat vervolgens de knop los om te teleporteren.",
        "Goed gedaan, nu heb je zelf de controle!"
    };

    private string[] currentTutorial;

    public enum Tool {
        None,
        Paintbrush,
        FireflyCatcher,
        Treasurefinder
    }

    private void Awake() {
        RightHandGrab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
        RightHandGrab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUnGrabObject);
        Instance = this;

        if (Tutorial) {
            if (Dutch) {
                currentTutorial = dutchTutorial;
            }
            else {
                currentTutorial = englishTutorial;
            }

            RightHandPointer.enabled = false;
            LeftHandPointer.enabled = false;
            wings.RotationEnabled = false;
        }
    }

    private void OnTeleport() {
        WindManager.Instance.UpdateWind();
        HeatManager.Instance.UpdateHeat();

        if(Breathing.Instance != null) {
            if((Breathing.Instance.transform.position - wings.transform.position).sqrMagnitude > 60f * 60f) {
                Destroy(Breathing.Instance.gameObject);
            }
        }
    }

    private void Start() {
        if (Tutorial && !MuscleRelaxationStarter.StartOnAwake) {
            StartTutorial();
        }

        menuLineRenderer.enabled = false;
        PaintingCanvas.SetActive(true);
        Tree.SetAllTrees(0f, 1f, StandardAmountOfLeaves);
    }

    public void StartTutorial() {
        wings.RotationEnabled = true;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[0], AudioManager.TutorialClips[0], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void PickupTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        RightHandPointer.enabled = true;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[1], AudioManager.TutorialClips[1], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void PickupTutorialPart2() {
        tutorialPart++;
        currentMessage.StartFade();
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[2], AudioManager.TutorialClips[2], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void TeleportTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        LeftHandPointer.enabled = true;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[3], AudioManager.TutorialClips[3], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void TeleportPart2Tutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        startPosition = wings.transform.parent.position;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[4], AudioManager.TutorialClips[4], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void EndTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        ExerciseGrabber.Instance.gameObject.SetActive(true);
        Tutorial = false;
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[5], AudioManager.TutorialClips[5], 10f, Vector3.up * 1f, 2f, true, 4f);
    }



    // Update is called once per frame
    void Update()
    {
        if (menuEnabled) {
            MenuPointing();
        }

        if ((previousPosition - wings.transform.position).sqrMagnitude > 0.5f) {
            OnTeleport();
        }
        previousPosition = wings.transform.position;

        if (painting)
            PaintingMenu();

        if (!Tutorial || MuscleRelaxationStarter.StartOnAwake)
            return;

        switch (tutorialPart) {
            case 0:
                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.LTouch) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.LTouch)) {
                    PickupTutorial();
                }
                break;
            case 1:
                if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
                    PickupTutorialPart2();
                }
                break;
            case 2:
                if(myTool == Tool.Paintbrush) {
                    TeleportTutorial();
                }
                break;
            case 3: 
                if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch)){
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

    private void MenuPointing() {
        RaycastHit hit;
        if(Physics.Raycast(RightHandGrab.transform.position, RightHandGrab.transform.forward, out hit, 10f)) {
            menuLineRenderer.SetPositions(new Vector3[] { RightHandGrab.transform.position, hit.point });
            if (hit.transform.CompareTag("MenuOption")) {
                if (selectedMenu == null || selectedMenu.name != hit.transform.name) {
                    if(selectedMenu != null) {
                        selectedMenu.SetDeselected();
                    }
                    MenuOption mo = hit.transform.GetComponent<MenuOption>();
                    selectedMenu = mo;
                    selectedMenu.SetSelected();
                }
            }
            else {
                DeselectMenu();
            }
        }
        else {
            menuLineRenderer.SetPositions(new Vector3[] { RightHandGrab.transform.position, RightHandGrab.transform.position + RightHandGrab.transform.forward * 10f });
            DeselectMenu();
        }
    }

    private void DeselectMenu() {
        if (selectedMenu == null)
            return;

        selectedMenu.SetDeselected();
        selectedMenu = null;
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

            menuLineRenderer.enabled = true;

            menuEnabled = true;
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch)) {
            Debug.Log("Released");

            if (!menuEnabled)
                return;

            if (selectedMenu != null) {
                selectedMenu.Select();
            }
            else {
                foreach (MenuOption mo in menuOptions) {
                    if (!mo.Selected)
                        continue;

                    mo.Select();
                }
            }

            menuLineRenderer.enabled = false;

            Destroy(ColorMenu);
            menuEnabled = false;
        }
    }
}
