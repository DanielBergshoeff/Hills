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
    }

    public void DisplayMessage(GameObject go, string message, AudioClip audio, float time = 0f, Vector3 relativePosition = default, float size = 1f, bool follow = false) {
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
        msg.Audio = audio;
        msg.RelativePosition = relativePosition;
    }
}
