using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    public static CommunicationManager Instance { get; private set; }
    public GameObject MessagePrefab;
    public GameObject Sapling;
    public float DisappearTime = 0f;
    public LayerMask TerrainLayer;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //DisplayMessage(Sapling, "Welcome.. Please take a deep breath..", 0f, Vector3.up * 2f, 1f);
        //DisplayMessage(null, "Try to squeeze your hands..", 30f, Camera.main.transform.forward * 10f, 0.3f);
    }

    public void DisplayMessage(GameObject go, string message, float time = 0f, Vector3 relativePosition = default, float size = 1f, bool follow = false) {
        GameObject messageObject = Instantiate(MessagePrefab);
        CommunicationMessage msg = messageObject.GetComponent<CommunicationMessage>();

        if (go == null)
            msg.FollowObject = Camera.main.gameObject;
        else {
            msg.FollowObject = go;
        }

        msg.Follow = follow;
        msg.Size = size;
        msg.TerrainLayer = TerrainLayer;
        msg.DisappearTime = time;
        msg.Text = message;
        msg.RelativePosition = relativePosition;
    }
}
