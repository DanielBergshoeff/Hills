using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShakeDetection : MonoBehaviour
{
    [Header("References")]
    public Transform LeftHand;
    public Transform RightHand;

    [SerializeField] private float timeTillShake = 1f;

    private Vector3 previousLeftHandPosition;
    private Vector3 previousRightHandPosition;

    private float shakeTimerRight = 0f;
    private float shakeTimerLeft = 0f;

    public static ShakeEvent shakeEvent;

    private GameObject leftHandGrabObject;
    private GameObject rightHandGrabObject;

    private void Start() {
        leftHandGrabObject = LeftHand.GetComponentInChildren<VRTK.VRTK_InteractGrab>().gameObject;
        rightHandGrabObject = RightHand.GetComponentInChildren<VRTK.VRTK_InteractGrab>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositions();
    }

    /// <summary>
    /// Calculate movement body based on feather positions
    /// </summary>
    private void UpdatePositions() {
        Vector3 velocityLeftHand = LeftHand.transform.position - previousLeftHandPosition;
        Vector3 velocityRightHand = RightHand.transform.position - previousRightHandPosition;

        if (Mathf.Abs(velocityRightHand.y) > 0.01f) {
            if (shakeTimerRight < timeTillShake + 0.01f)
                shakeTimerRight += Time.deltaTime;
        }
        else {
            if (shakeTimerRight > 0f)
                shakeTimerRight -= Time.deltaTime;
        }

        if (shakeTimerRight > timeTillShake) {
            shakeEvent.Invoke(rightHandGrabObject);
        }

        if (Mathf.Abs(velocityLeftHand.y) > 0.01f) {
            if (shakeTimerLeft < timeTillShake + 0.01f)
                shakeTimerLeft += Time.deltaTime;
        }
        else {
            if (shakeTimerLeft > 0f)
                shakeTimerLeft -= Time.deltaTime;
        }

        if (shakeTimerLeft > timeTillShake) {
            shakeEvent.Invoke(leftHandGrabObject);
        }

        previousLeftHandPosition = LeftHand.transform.localPosition;
        previousRightHandPosition = RightHand.transform.localPosition;
    }
}

public class ShakeEvent : UnityEvent<GameObject> { }
