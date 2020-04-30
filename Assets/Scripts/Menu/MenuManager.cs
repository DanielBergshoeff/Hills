using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuManager : MonoBehaviour
{
    public GameObject ColorMenuPrefab;
    public GameObject ColorMenu;
    public Wings wings;
    public float MenuDistance = 0.1f;

    private MenuOption[] menuOptions;
    private bool menuEnabled = false;
    private bool painting = true;


    public bool Tutorial = false;
    public VRTK_InteractGrab RightHandGrab;
    public VRTK_Pointer RightHandPointer;
    public VRTK_Pointer LeftHandPointer;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (painting)
            PaintingMenu();
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
