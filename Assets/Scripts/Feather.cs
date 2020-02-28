using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour {
    public int AmountOfPositions;
    public Vector3 AverageForce;

    private Vector3[] positions;
    private Quaternion[] rotations;
    private int currentPosition = 0;

    public bool ShowDebug = false;

    // Start is called before the first frame update
    void Start() {
        positions = new Vector3[AmountOfPositions];
        rotations = new Quaternion[AmountOfPositions];

        for (int i = 0; i < positions.Length; i++) {
            positions[i] = transform.localPosition;
            rotations[i] = transform.localRotation;
        }
    }

    // Update is called once per frame
    void Update() {
        positions[currentPosition] = transform.localPosition;
        rotations[currentPosition] = transform.localRotation;
        currentPosition++;
        if (currentPosition >= AmountOfPositions)
            currentPosition = 0;

        Vector3 positionMovement = Vector3.zero;

        for (int i = 0; i < AmountOfPositions - 1; i++) {
            float angle = Vector3.Angle(rotations[i].eulerAngles, positions[i + 1] - positions[i]) / 90f;
            if ((int)angle % 2 == 1) {
                angle = 1f - angle % 2;
            }
            if (ShowDebug) {
                Debug.Log(rotations[i].eulerAngles);
                Debug.Log(positions[i + 1] - positions[i]);
                Debug.Log(angle);
            }
            positionMovement += (positions[i + 1] - positions[i]) * angle;
        }

        AverageForce = positionMovement / AmountOfPositions;
        if (ShowDebug)
            Debug.Log(AverageForce);
    }
}
