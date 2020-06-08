using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseGrabber : MonoBehaviour
{
    public static ExerciseGrabber Instance;

    public GameObject BreathingPrefab;
    public VicinityMessage MyVicinityMessage;
    public float MinDistance = 5f;
    public float MaxDistance = 50f;

    private bool touching = false;
    private bool grabbed = false;

    private GameObject breathingExercise;
    private LineRenderer lineRenderer;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (MenuManager.Tutorial) {
            gameObject.SetActive(false);
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        MyVicinityMessage = GetComponent<VicinityMessage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed) {
            UpdateExercisePlacement();
        }

        if (!touching)
            return;

        if (!grabbed) {
            CheckForGrab();
        }
    }

    /// <summary>
    /// Update the exercise position based on player right hand
    /// </summary>
    private void UpdateExercisePlacement() {
        RaycastHit hit;

        if (Physics.Raycast(Wings.Instance.RightHand.transform.position, Wings.Instance.RightHand.transform.forward, out hit, MaxDistance) && (Wings.Instance.RightHand.transform.position - hit.point).sqrMagnitude > MinDistance * MinDistance) {
            lineRenderer.SetPositions(new Vector3[] { Wings.Instance.RightHand.transform.position, hit.point });
            breathingExercise.transform.position = hit.point + Vector3.up * 1.5f;
            Vector3 heading = Camera.main.transform.position - breathingExercise.transform.position;
            breathingExercise.transform.rotation = Quaternion.LookRotation(-heading);
        }
        else {
            lineRenderer.SetPositions(new Vector3[] { Wings.Instance.RightHand.transform.position, Wings.Instance.RightHand.transform.position + Wings.Instance.RightHand.transform.forward * MaxDistance });
            lineRenderer.startColor = Color.red;
        }

        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") < 0.5f) {
            grabbed = false;
            lineRenderer.enabled = false;
            breathingExercise.GetComponent<Breathing>().StartBreathing();
        }
    }

    /// <summary>
    /// Check if the player has grabbed the exercise
    /// </summary>
    private void CheckForGrab() {
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.5f) {
            if (MyVicinityMessage != null) {
                MyVicinityMessage.StopMessage();
                Destroy(MyVicinityMessage);
            }

            grabbed = true;
            lineRenderer.enabled = true;

            if (breathingExercise != null) {
                Destroy(breathingExercise);
            }

            touching = false;
            transform.GetChild(0).localScale = new Vector3(0.025f, 0.025f, 0.025f);

            breathingExercise = Instantiate(BreathingPrefab);
            breathingExercise.transform.position = Camera.main.transform.parent.parent.position + Camera.main.transform.parent.parent.forward * 5f + Vector3.up * 1f;
            Vector3 heading = breathingExercise.transform.position - Camera.main.transform.position;
            breathingExercise.transform.rotation = Quaternion.LookRotation(heading);
        }
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name != "GrabVolumeSmall")
            return;

        touching = true;
        transform.GetChild(0).localScale = new Vector3(0.045f, 0.045f, 0.045f);
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.name != "GrabVolumeSmall")
            return;

        touching = false;
        transform.GetChild(0).localScale = new Vector3(0.025f, 0.025f, 0.025f);
    }
}
