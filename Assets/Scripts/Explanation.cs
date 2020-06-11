using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explanation : MessageObject
{
    public string Title;
    public string TitleDutch;
    public float TitleTextSize = 12f;

    public new void ShowMessage() {
        if (myMessage != null)
            return;

        string msg = MenuManager.Dutch ? MessageDutch : Message;
        string title = MenuManager.Dutch ? TitleDutch : Title;

        myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, title, msg, null, MessageDuration, Position, Size, FollowObject, TextSize, TitleTextSize);
        displayingMessage = true;
        Invoke("RevertDisplayingMessage", MessageDuration);
    }
}
