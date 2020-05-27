using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    public float MaxRange = 10f;
    public string Message = "This is an example text. Use it wisely.";
    public string MessageDutch = "Dit is een voorbeeld text. Gebruik het wijslijk.";
    public float Size = 0.075f;
    public float TextSize = 10f;
    public float MessageDuration = 10f;
    public Vector3 Position = Vector3.up * 0.2f;
    public bool FollowObject = true;

    protected CommunicationMessage myMessage;
    protected bool displayingMessage;

    public void StopMessage() {
        if (myMessage != null)
            myMessage.StartFade();
        displayingMessage = false;
    }

    public void ShowMessage() {
        string msg = MenuManager.Dutch ? MessageDutch : Message;

        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, msg, null, MessageDuration, Position, Size, FollowObject, TextSize);
        displayingMessage = true;
        Invoke("RevertDisplayingMessage", MessageDuration);
    }

    private void RevertDisplayingMessage() {
        displayingMessage = false;
    }
}
