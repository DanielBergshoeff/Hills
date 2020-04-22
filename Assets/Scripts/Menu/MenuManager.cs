using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject ColorMenuPrefab;
    public GameObject ColorMenu;
    public float MenuDistance = 0.1f;

    private MenuOption[] menuOptions;
    private bool menuEnabled = false;
    private bool painting = false;

    // Update is called once per frame
    void Update()
    {
        if (painting)
            PaintingMenu();
    }

    private void StartPainting() {
        painting = true;
    }

    private void PaintingMenu() {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch)) {
            Debug.Log("Pressed");
            ColorMenu = Instantiate(ColorMenuPrefab, transform);
            Vector3 forward = new Vector3(Wings.Instance.RightHand.forward.x, 0f, Wings.Instance.RightHand.forward.z).normalized;
            ColorMenu.transform.position = Wings.Instance.RightHand.position + forward * MenuDistance;
            ColorMenu.transform.rotation = Quaternion.LookRotation(forward);
            menuOptions = ColorMenu.GetComponentsInChildren<MenuOption>();
            menuEnabled = true;
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch)) {
            Debug.Log("Released");

            if (!menuEnabled)
                return;

            Destroy(ColorMenu);
            menuEnabled = false;

            foreach (MenuOption mo in menuOptions) {
                if (!mo.Selected)
                    continue;

                mo.Select();
            }
        }
    }
}
