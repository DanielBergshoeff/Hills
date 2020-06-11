using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleRelaxationStarter : MonoBehaviour
{
    public static bool StartOnAwake = false;
    public static MuscleRelaxationStarter Instance;

    public GameObject MuscleRelaxation;
    public GameObject DutchMuscleRelaxation;
    public GameObject TutorialObject;

    private void Awake() {
        Instance = this;

        if (!StartOnAwake) {
            //Destroy(gameObject);
        }
    }

    private void Update() {
        CheckForPMR();
    }

    private void CheckForPMR() {
        if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch)) {
            RaycastHit hit;
            if(Physics.Raycast(Wings.Instance.RightHand.transform.position, Wings.Instance.RightHand.forward, out hit, 100f)) {
                if (hit.collider.name == gameObject.name) {
                    GameObject currentMuscleRelaxation = null;

                    if (MenuManager.Dutch)
                        currentMuscleRelaxation = DutchMuscleRelaxation;
                    else
                        currentMuscleRelaxation = MuscleRelaxation;

                    currentMuscleRelaxation.SetActive(true);
                    foreach (MuscleRelaxation mr in currentMuscleRelaxation.GetComponentsInChildren<MuscleRelaxation>()) {
                        mr.StartMuscleRelaxation();
                    }

                    DisableObjects();
                }

                else if(hit.collider.name == TutorialObject.name) {
                    MenuManager.Instance.StartTutorial();
                    DisableObjects();
                }
            }
        }
    }

    public void DisableObjects() {
        TutorialObject.GetComponent<VicinityMessage>().StopMessage();
        GetComponent<VicinityMessage>().StopMessage();
        TutorialObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void EnableObjects() {
        TutorialObject.SetActive(true);
        gameObject.SetActive(true);
    }
}
