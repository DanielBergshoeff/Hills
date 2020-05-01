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

    private void Awake() {
        Instance = this;
    }

    public CommunicationMessage DisplayMessage(GameObject go, string message, AudioClip audio, float time = 0f, Vector3 relativePosition = default, float size = 1f, bool follow = false, float textsize = 10f) {
        GameObject messageObject = Instantiate(MessagePrefab);
        CommunicationMessage msg = messageObject.GetComponent<CommunicationMessage>();

        if (go == null)
            msg.FollowObject = Camera.main.gameObject;
        else {
            msg.FollowObject = go;
        }

        msg.TextSize = textsize;
        msg.Follow = follow;
        msg.Size = size;
        msg.TerrainLayer = TerrainLayer;
        msg.DisappearTime = time;
        msg.Text = message;
        msg.Audio = audio;
        msg.RelativePosition = relativePosition;

        return msg;
    }
}
