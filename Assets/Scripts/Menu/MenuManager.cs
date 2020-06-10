﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.VFX;
using VRTK;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public static bool Dutch = false;
    public static StandardMenuEvent standardMenuEvent;

    public GameObject ColorMenuPrefab;
    public GameObject StandardMenuPrefab;
    public GameObject ColorMenu;
    public GameObject TutorialPosition;
    public Wings wings;
    public float MenuDistance = 0.1f;
    public GameObject PaintingCanvas;
    public float StandardAmountOfLeaves = 1f;

    public VisualEffect FireflyCatcherVFX;
    public float MaxFireflyCatcherTime = 10f;

    public LineRenderer menuLineRenderer;

    private MenuOption[] menuOptions;
    private MenuOption selectedMenu;
    private bool menuEnabled = false;
    private bool painting = true;
    private bool menuRightHand = false;


    public static bool Tutorial = false;
    public VRTK_InteractGrab RightHandGrab;
    public VRTK_Pointer RightHandPointer;
    public VRTK_Pointer LeftHandPointer;
    public VRTK_HeightAdjustTeleport Teleporter;

    private int tutorialPart = 0;
    private CommunicationMessage currentMessage;
    private Vector3 previousPosition;
    private Vector3 startPosition;

    public GameObject WarmthPrefab;
    private float fireflyCatcherCooldown = 0f;

    private Tool myTool;
    private GameObject heldObject;
    private FireflyCatcher fireflyCatcher;

    private AudioSource grabAudioSource;
    private AudioSource menuAudioSource;

    private string[] englishTutorial = new string[] {
        "To rotate, gently nudge the thumbstick on your left controller into the desired direction. Try it now.",
        "To pick up items from a distance, press the A button on your right controller to make a ray appear from it.",
        "Aim at the brush on the floor and squeeze the right controller's middle finger to pick it up.",
        "To teleport, press the X button on the left controller to make an arch appear first.",
        "Aim for a stepping stone on the path, until the arch turns green. Release the button to teleport.",
        "Good job, now you are in control!",
        "If you dont know how to use an object, hold the A button on your right controller and aim for the object.",
        "Then click the button by your index finger to show an info screen. Try it with the paint brush.",
        "Well done. Now that you know how to use it, try painting on the canvas."
    };

    private string[] dutchTutorial = new string[] {
        "Om te roteren, kan je de joystick op je linker controller zachtjes in de gewenste richting duwen. Probeer het nu.",
        "Om objecten van een afstand op te pakken, druk je op de A knop op je rechter controller om een straal te laten verschijnen.",
        "Richt met de straal op de kwast en knijp met je middelvinger de onderste knop in om de kwast op te pakken.",
        "Om te teleporteren kan je de X knop op je linker controller indrukken om een boog te laten verschijnen.",
        "Richt vervolgens op een steen op het pad, tot de boog groen wordt. Laat vervolgens de knop los om te teleporteren.",
        "Goed gedaan, nu heb jij de controle!",
        "Als je niet weet hoe je een object kan gebruiken, houd dan de A knop ingedrukt op je rechter controller, en richt op het object.",
        "Klik vervolgens op de knop bij je wijsvinger om een informatiescherm te laten zien. Probeer het eens met de kwast.",
        "Goed gedaan. Nu je weet hoe je het moet gebruiken,probeer eens op het canvas te schilderen."
    };

    private string[] currentTutorial;

    public enum Tool {
        None,
        Paintbrush,
        FireflyCatcher,
        Treasurefinder,
        Seashell
    }

    private void Awake() {
        RightHandGrab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
        RightHandGrab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUnGrabObject);
        Instance = this;
        standardMenuEvent = new StandardMenuEvent();
        standardMenuEvent.AddListener(SelectMenu);

        grabAudioSource = gameObject.AddComponent<AudioSource>();
        AudioMixer mixer = Resources.Load("MixerGroups/Ambience") as AudioMixer;
        grabAudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("GrabSound")[0];


        menuAudioSource = gameObject.AddComponent<AudioSource>();

        if (Tutorial) {
            if (Dutch) {
                currentTutorial = dutchTutorial;
            }
            else {
                currentTutorial = englishTutorial;
            }

            wings.ToggleTutorialHands(true);

            RightHandPointer.enabled = false;
            LeftHandPointer.enabled = false;
            wings.RotationEnabled = false;
        }
    }

    private void SelectMenu(StandardMenuOption smo) {
        if(smo == StandardMenuOption.Exit) {
            Application.Quit();
        }
        else if(smo == StandardMenuOption.ReturnToStart) {
            wings.ResetPosition();
        }
    }

    private void OnTeleport() {
        WindManager.Instance.UpdateWind();
        //HeatManager.Instance.UpdateHeat();

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
        wings.HighlightButton(wings.ThumbstickLeftRenderer);
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[0], AudioManager.TutorialClips[0], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void ExplanationTutorial() {
        RightHandPointer.enabled = true;
        tutorialPart++;
        currentMessage.StartFade();
        wings.ReverseHighlight(wings.ThumbstickLeftRenderer);
        wings.HighlightButton(wings.ButtonOneRightRenderer);
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[6], AudioManager.TutorialClips[6], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void ExplanationTutorialPart2() {
        tutorialPart++;
        currentMessage.StartFade();
        wings.HighlightButton(wings.IndexRightRenderer);
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[7], AudioManager.TutorialClips[7], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void PickupTutorial() {
        wings.ReverseHighlight(wings.IndexRightRenderer);
        tutorialPart++;
        currentMessage.StartFade();
        wings.HighlightButton(wings.ButtonOneRightRenderer);
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[1], AudioManager.TutorialClips[1], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void PickupTutorialPart2() {
        tutorialPart++;
        currentMessage.StartFade();
        wings.HighlightButton(wings.HandRightRenderer);
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[2], AudioManager.TutorialClips[2], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void PaintTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[8], AudioManager.TutorialClips[8], 0f, Vector3.up * 1f, 2f, true, 4f);
    }

    private void TeleportTutorial() {
        tutorialPart++;
        currentMessage.StartFade();
        LeftHandPointer.enabled = true;
        wings.ReverseHighlight(wings.ButtonOneRightRenderer);
        wings.ReverseHighlight(wings.HandRightRenderer);
        wings.HighlightButton(wings.ButtonOneLeftRenderer);
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
        wings.ReverseHighlight(wings.ButtonOneLeftRenderer);

        Tutorial = false;

        wings.ToggleTutorialHands(false);

        currentMessage = CommunicationManager.Instance.DisplayMessage(TutorialPosition, currentTutorial[5], AudioManager.TutorialClips[5], AudioManager.TutorialClips[5].length, Vector3.up * 1f, 2f, true, 4f);
    }

    private void FireflyCatcherUpdate() {
        if (fireflyCatcherCooldown > 0f) {
            FireflyCatcherVFX.SetVector3("Velocity", fireflyCatcher.transform.up * 2f);
            FireflyCatcherVFX.SetVector3("SpawnPosition", fireflyCatcher.transform.position);
            fireflyCatcherCooldown -= Time.deltaTime;
            if(fireflyCatcherCooldown <= 0f) {
                FireflyCatcherVFX.SetFloat("SpawnRate", 0f);
            }
        }

        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
            if(fireflyCatcher.currentFireflies > 0 && fireflyCatcherCooldown <= 0f) {
                Debug.Log("Firefly use!");
                GameObject go = Instantiate(WarmthPrefab);
                go.transform.parent = fireflyCatcher.transform;
                go.transform.localPosition = Vector3.zero;
                fireflyCatcher.RemoveFirefly();
                fireflyCatcherCooldown = MaxFireflyCatcherTime;
                FireflyCatcherVFX.SetFloat("SpawnRate", 3f);
            }
        }
    }

    private void SeashellUpdate() {
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
            heldObject.GetComponent<Seashell>().ToggleSong();
        }
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

        if(myTool == Tool.None)
            StandardMenu();

        if (painting)
            PaintingMenu();

        if(myTool == Tool.FireflyCatcher || fireflyCatcherCooldown > 0f) 
            FireflyCatcherUpdate();

        if (myTool == Tool.Seashell)
            SeashellUpdate();

        if (!Tutorial || MuscleRelaxationStarter.StartOnAwake)
            return;

        switch (tutorialPart) {
            case 0:
                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.LTouch) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.LTouch)) {
                    ExplanationTutorial();
                }
                break;
            case 1:
                if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
                    ExplanationTutorialPart2();
                }
                break;
            case 2:
                if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
                    PickupTutorial();
                }
                break;
            case 3:
                if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
                    PickupTutorialPart2();
                }
                break;
            case 4:
                if(myTool == Tool.Paintbrush) {
                    PaintTutorial();
                }
                break;
            case 5:
                if(myTool == Tool.Paintbrush && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
                    TeleportTutorial();
                }
                break;
            case 6: 
                if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch)){
                    TeleportPart2Tutorial();
                }
                break;
            case 7:
                if((startPosition - wings.transform.position).sqrMagnitude > 0.5f) {
                    EndTutorial();
                }
                break;
            default:
                break;

        }
    }

    private void StandardMenu() {
        if(OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
            ColorMenu = Instantiate(StandardMenuPrefab, transform);
            Vector3 forward = new Vector3(Wings.Instance.LeftHand.forward.x, 0f, Wings.Instance.LeftHand.forward.z).normalized;
            ColorMenu.transform.position = Wings.Instance.LeftHand.position + forward * MenuDistance;
            ColorMenu.transform.rotation = Quaternion.LookRotation(forward);
            menuOptions = ColorMenu.GetComponentsInChildren<MenuOption>();

            menuLineRenderer.enabled = true;
            menuEnabled = true;
            menuRightHand = false;
        }

        if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
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

    private void MenuPointing() {
        RaycastHit hit;
        Transform hand = RightHandGrab.transform;
        if (!menuRightHand)
            hand = wings.LeftHand;

        if(Physics.Raycast(hand.position, hand.forward, out hit, 10f)) {
            menuLineRenderer.SetPositions(new Vector3[] { hand.position, hit.point });
            if (hit.transform.CompareTag("MenuOption")) {
                if (selectedMenu == null || selectedMenu.name != hit.transform.name) {
                    if(selectedMenu != null) {
                        selectedMenu.SetDeselected();
                    }
                    MenuOption mo = hit.transform.GetComponent<MenuOption>();
                    selectedMenu = mo;
                    selectedMenu.SetSelected();
                    menuAudioSource.PlayOneShot(AudioManager.Instance.MenuHoverSound);
                }
            }
            else {
                DeselectMenu();
            }
        }
        else {
            menuLineRenderer.SetPositions(new Vector3[] { hand.position, hand.position + hand.forward * 10f });
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

        Explanation ex = e.target.GetComponent<Explanation>();
        if (ex != null) {
            ex.StopMessage();
        }

        heldObject = e.target.gameObject;

        if (!e.target.CompareTag("Tool"))
            return;

        Debug.Log("Tool grabbed");

        if (e.target.name == "Seashell") {
            myTool = Tool.Seashell;
        }
        else {
            grabAudioSource.PlayOneShot(AudioManager.Instance.WoodGrabClips[Random.Range(0, AudioManager.Instance.WoodGrabClips.Count)]);

            if (e.target.name == "Paintbrush") {
                myTool = Tool.Paintbrush;
                Paintable.Instance.PaintingEnabled = true;
            }
            else if (e.target.name == "FireflyCatcher") {
                myTool = Tool.FireflyCatcher;
                fireflyCatcher = e.target.GetComponentInChildren<FireflyCatcher>();
            }
            else if (e.target.name == "TreasureFinder") {
                myTool = Tool.Treasurefinder;
            }
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
            menuRightHand = true;
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

public class StandardMenuEvent : UnityEvent<StandardMenuOption> { }
