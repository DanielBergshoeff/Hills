using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    public static CommunicationManager Instance { get; private set; }
    public GameObject MessagePrefab;
    public GameObject MessageWithTitlePrefab;
    public GameObject ButtonMesssage;
    public GameObject Sapling;
    public float DisappearTime = 0f;
    public LayerMask TerrainLayer;

    private void Awake() {
        Instance = this;
    }

    /// <summary>
    /// Displays a message in the world
    /// </summary>
    public CommunicationMessage DisplayMessage(GameObject go, string message, AudioClip audio, float time = 0f, Vector3 relativePosition = default, float size = 1f, bool follow = false, float textsize = 10f) {
        GameObject messageObject = Instantiate(MessagePrefab);
        CommunicationMessage msg = messageObject.GetComponent<CommunicationMessage>();

        if (go == null)
            msg.FollowObject = Camera.main.gameObject;
        else {
            msg.FollowObject = go;
        }

        SetMessageValues(msg, message, audio, time, relativePosition, size, follow, textsize);

        return msg;
    }

    /// <summary>
    /// Displays a message with a title in the world
    /// </summary>
    public CommunicationMessage DisplayMessage(GameObject go, string title, string message, AudioClip audio, float time = 0f, Vector3 relativePosition = default, float size = 1f, bool follow = false, float textsize = 10f, float titleSize = 12f) {
        GameObject messageObject = Instantiate(MessageWithTitlePrefab);
        CommunicationMessage msg = messageObject.GetComponent<CommunicationMessage>();

        if (go == null)
            msg.FollowObject = Camera.main.gameObject;
        else {
            msg.FollowObject = go;
        }

        SetMessageValues(msg, message, audio, time, relativePosition, size, follow, textsize);

        msg.Title = title;
        msg.TitleTextSize = titleSize;

        return msg;
    }

    /// <summary>
    /// Displays a message with a title in the world
    /// </summary>
    public CommunicationMessage DisplayButtonMessage(GameObject go, string title, string message, AudioClip audio, float time = 0f, Vector3 relativePosition = default, float size = 1f, bool follow = false, float textsize = 10f, float titleSize = 12f, float cameraPositioning = 0f) {
        GameObject messageObject = Instantiate(ButtonMesssage);
        CommunicationMessage msg = messageObject.GetComponent<CommunicationMessage>();

        if (go == null)
            msg.FollowObject = Camera.main.gameObject;
        else {
            msg.FollowObject = go;
        }

        SetMessageValues(msg, message, audio, time, relativePosition, size, follow, textsize);

        msg.Title = title;
        msg.TitleTextSize = titleSize;
        msg.CameraPositioning = cameraPositioning;

        return msg;
    }

    /// <summary>
    /// Set the values of a message
    /// </summary>
    private void SetMessageValues(CommunicationMessage msg, string message, AudioClip audio, float time = 0f, Vector3 relativePosition = default, float size = 1f, bool follow = false, float textsize = 10f) {
        msg.TextSize = textsize;
        msg.Follow = follow;
        msg.Size = size;
        msg.TerrainLayer = TerrainLayer;
        msg.DisappearTime = time;
        msg.Text = message;
        msg.MyAudioClip = audio;
        msg.RelativePosition = relativePosition;
    }
}
