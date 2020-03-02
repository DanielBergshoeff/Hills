using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    public static CommunicationManager Instance { get; private set; }
    public GameObject MessagePrefab;
    public GameObject Sapling;
    public float DisappearTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DisplayMessage(Sapling, "Welcome.. Please take a deep breath..", 10f, Vector3.up * 2f);
    }

    public void DisplayMessage(GameObject go, string message, float time = 0f, Vector3 relativePosition = default) {
        GameObject messageObject = Instantiate(MessagePrefab);
        CommunicationMessage msg = messageObject.GetComponent<CommunicationMessage>();
        msg.FollowObject = go;
        msg.DisappearTime = time;
        msg.Text = message;
        msg.RelativePosition = relativePosition;
    }
}
