using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class Wings : MonoBehaviour {
    public static Wings Instance { get; private set; }

    [Header("References")]
    public Transform LeftHand;
    public Transform RightHand;
    public Transform Head;

    public GameObject LeftEstimatedElbow;
    public GameObject RightEstimatedElbow;

    [Header("Wing information")]
    public bool RenderWings = false;
    public float WingSpan;
    public float rotationOffset = 0.01f;
    public int AmtOfFeathers;
    public float Force;

    public bool Testing;
    public bool Flying = false;
    public bool RotationEnabled = true;

    public GameObject FeatherPrefab;

    private List<GameObject> FeathersLeftWing;
    private List<GameObject> FeathersRightWing;
    private List<Feather> Feathers;

    public int AmountOfPositions;
    [SerializeField] private bool verticalMovement = false;
    [SerializeField] private float maxSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 1f;
    public float RotationRatchet = 45f;

    private int currentPosition = 0;
    private int filledPositions = 0;

    private Vector3[] handLeftPositions;
    private Vector3[] handRightPositions;
    private Vector3[] handLeftRotations;
    private Vector3[] handRightRotations;

    private Rigidbody myRigidbody;
    private GameObject FeatherParent;
    private bool ReadyToSnapTurn = true;
    public bool RotationEitherThumbstick = false;
    
    private void Awake() {
        Debug.Log("Awake");
        myRigidbody = transform.parent.GetComponent<Rigidbody>();
        if (RenderWings)
            SpawnFeathers();
        ResetPositions();

        Instance = this;
    }

    private void Start() {
        HeatManager.Instance.UpdateHeat();
        WindManager.Instance.UpdateWind();
    }

    private void UpdateRotations() {
        Vector3 euler = transform.parent.rotation.eulerAngles;
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch) ||
                    (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))) {
            if (ReadyToSnapTurn) {
                euler.y -= RotationRatchet;
                ReadyToSnapTurn = false;
            }
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch) ||
            (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))) {
            if (ReadyToSnapTurn) {
                euler.y += RotationRatchet;
                ReadyToSnapTurn = false;
            }
        }
        else {
            ReadyToSnapTurn = true;
        }

        if (!ReadyToSnapTurn) {
            WindManager.Instance.UpdateWind();
            HeatManager.Instance.UpdateHeat();
        }

        transform.parent.rotation = Quaternion.Euler(euler);
    }

    private void SpawnFeathers() {
        Feathers = new List<Feather>();
        FeathersLeftWing = new List<GameObject>();
        FeathersRightWing = new List<GameObject>();
        FeatherParent = new GameObject();
        FeatherParent.transform.parent = this.transform;
        FeatherParent.transform.localPosition = Vector3.zero;
        for (int i = 0; i < AmtOfFeathers; i++) {
            GameObject featherLeft = Instantiate(FeatherPrefab, FeatherParent.transform);
            featherLeft.name = "Feather left " + i.ToString();
            FeathersLeftWing.Add(featherLeft);
            Feathers.Add(featherLeft.GetComponent<Feather>());

            GameObject featherRight = Instantiate(FeatherPrefab, FeatherParent.transform);
            featherRight.name = "Feather right " + i.ToString();
            FeathersRightWing.Add(featherRight);
            Feathers.Add(featherRight.GetComponent<Feather>());
        }
    }

    private void ResetPositions() {
        handLeftPositions = new Vector3[AmountOfPositions];
        handRightPositions = new Vector3[AmountOfPositions];
        handLeftRotations = new Vector3[AmountOfPositions];
        handRightRotations = new Vector3[AmountOfPositions];

        for (int i = 0; i < AmountOfPositions; i++) {
            handLeftPositions[i] = LeftHand.transform.localPosition;
            handRightPositions[i] = RightHand.transform.localPosition;
            handLeftRotations[i] = LeftHand.transform.forward;
            handRightRotations[i] = RightHand.transform.forward;
        }
    }

    // Update is called once per frame
    void Update() {
        if (RenderWings)
            SetWingPositionAndRotation();
        
        if(RotationEnabled)
            UpdateRotations();

        if (!Flying)
            return;

        if ((Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.5f && Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.5f) || Testing) {
            WingBasedMovement();
        }
        else {
            ResetPositions();
            currentPosition = 0;
            filledPositions = 0;
        }
    }


    /// <summary>
    /// Calculate movement body based on feather positions
    /// </summary>
    private void WingBasedMovement() {
        float speed = myRigidbody.velocity.magnitude;

        handLeftPositions[currentPosition] = LeftHand.transform.localPosition;
        handRightPositions[currentPosition] = RightHand.transform.localPosition;
        handLeftRotations[currentPosition] = LeftHand.transform.forward;
        handRightRotations[currentPosition] = RightHand.transform.forward;

        currentPosition += 1;
        if (filledPositions < AmountOfPositions - 1)
            filledPositions += 1;
        if (currentPosition >= AmountOfPositions)
            currentPosition = 0;

        Vector3 forceToAdd = Vector3.zero;

        for (int i = 0; i < filledPositions; i++) {
            if (i == currentPosition - 1)
                continue;

            float angleLeft = Vector3.Angle(handLeftRotations[i], (handLeftPositions[i] - handLeftPositions[i + 1]).normalized * 360f) / 90f;
            float angleRight = Vector3.Angle(handRightRotations[i], (handRightPositions[i] - handRightPositions[i + 1]).normalized * 360f) / 90f;

            angleLeft = 1f;
            angleRight = 1f;

            forceToAdd += (handLeftPositions[i] - handLeftPositions[i + 1]) * angleLeft;
            forceToAdd += (handRightPositions[i] - handRightPositions[i + 1]) * angleRight;
        }

        if (!verticalMovement)
            forceToAdd = new Vector3(forceToAdd.x, 0f, forceToAdd.z);

        /*
        transform.parent.Rotate(0f, forceToAdd.x * rotateSpeed, 0f);
        forceToAdd = new Vector3(0f, 0f, forceToAdd.z);
        */

        transform.parent.GetComponent<Rigidbody>().AddRelativeForce((forceToAdd / AmountOfPositions) * Force);

        if (transform.parent.GetComponent<Rigidbody>().velocity.sqrMagnitude > maxSpeed * maxSpeed) {
            transform.parent.GetComponent<Rigidbody>().velocity = transform.parent.GetComponent<Rigidbody>().velocity.normalized * maxSpeed;
        }
    }

    /// <summary>
    /// Calculate and alter feather positions and rotations
    /// </summary>
    private void SetWingPositionAndRotation() {
        //Calculate elbow height left and right
        float LeftElbowHeight = Mathf.Clamp(LeftHand.localPosition.y - (1f - Mathf.Abs(LeftHand.localPosition.x) / WingSpan) * WingSpan / 2f, -WingSpan / 2f, WingSpan / 2f);
        float RightElbowHeight = Mathf.Clamp(RightHand.localPosition.y - (1f - Mathf.Abs(RightHand.localPosition.x) / WingSpan) * WingSpan / 2f, -WingSpan / 2f, WingSpan / 2f);

        //Calculate the heading of left and right hand relative to head
        Vector3 headingLeft = LeftHand.transform.position - Head.transform.position;
        Vector3 headingRight = RightHand.transform.position - Head.transform.position;

        //Set the estimated elbow positions
        LeftEstimatedElbow.transform.position = new Vector3(Head.transform.position.x + headingLeft.x / 2f, Head.transform.position.y + LeftElbowHeight, Head.transform.position.z + headingLeft.z / 2f);
        RightEstimatedElbow.transform.position = new Vector3(Head.transform.position.x + headingRight.x / 2f, Head.transform.position.y + RightElbowHeight, Head.transform.position.z + headingRight.z / 2f);

        //Calculate the heading from head to elbows
        Vector3 HeadToLeftElbow = LeftEstimatedElbow.transform.position - Head.transform.position;
        float DistanceElbowLeft = HeadToLeftElbow.magnitude;
        Vector3 HeadToRightElbow = RightEstimatedElbow.transform.position - Head.transform.position;
        float DistanceElbowRight = HeadToRightElbow.magnitude;

        //Calculate the heading from elbows to hands
        Vector3 LeftElbowToHand = (LeftHand.transform.position - LeftEstimatedElbow.transform.position).normalized;
        Vector3 RightElbowToHand = (RightHand.transform.position - RightEstimatedElbow.transform.position).normalized;

        //Apply position of the feathers
        for (int i = 0; i < AmtOfFeathers; i++) {
            if (i < AmtOfFeathers / 2) {
                FeathersLeftWing[i].transform.position = Head.transform.position + (HeadToLeftElbow.normalized * ((float)i / AmtOfFeathers) * 2f) * DistanceElbowLeft;
                FeathersRightWing[i].transform.position = Head.transform.position + (HeadToRightElbow.normalized * ((float)i / AmtOfFeathers) * 2f) * DistanceElbowRight;
            }
            else {
                FeathersLeftWing[i].transform.position = LeftEstimatedElbow.transform.position + (LeftElbowToHand * ((float)(i - (AmtOfFeathers / 2)) / (AmtOfFeathers / 2f)) / 2f);
                FeathersRightWing[i].transform.position = RightEstimatedElbow.transform.position + (RightElbowToHand * ((float)(i - (AmtOfFeathers / 2)) / (AmtOfFeathers / 2f)) / 2f);
            }

            //Edit rotation of the wings based on rotation of hands
            FeathersLeftWing[i].transform.rotation = Quaternion.Lerp(LeftHand.transform.rotation, Quaternion.identity, Mathf.Clamp(1.0f - (i * rotationOffset / AmtOfFeathers), 0f, 1f));
            FeathersRightWing[i].transform.rotation = Quaternion.Lerp(RightHand.transform.rotation, Quaternion.identity, Mathf.Clamp(1.0f - (i * rotationOffset / AmtOfFeathers), 0f, 1f));
        }
    }
}
