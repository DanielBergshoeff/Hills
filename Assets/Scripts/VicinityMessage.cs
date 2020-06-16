using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicinityMessage : MessageObject
{
    private bool cooldown = false;
    public float CooldownTime = 90f;
    public string Title = "";
    public string TitleDutch = "";
    public float TitleSize = 10f;

    // Update is called once per frame
    void Update()
    {
        if (displayingMessage || cooldown)
            return;

        if (Camera.main == null)
            return;

        if ((transform.position - Camera.main.transform.position).sqrMagnitude > MaxRange * MaxRange)
            return;

        Vector3 heading = transform.position - Camera.main.transform.position;
        float angle = Vector3.Angle(heading, Camera.main.transform.forward);

        if(angle < 20f) {
            string msg = MenuManager.Dutch ? MessageDutch : Message;
            string title = MenuManager.Dutch ? TitleDutch : Title;

            if (Title == "")
                myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, msg, null, MessageDuration, Position, Size, FollowObject, TextSize);
            else
                myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, title, msg, null, MessageDuration, Position, Size, FollowObject, TextSize, TitleSize);
            displayingMessage = true;
            Invoke("RevertDisplayingMessage", MessageDuration);
        }
    }

    private void RevertDisplayingMessage() {
        displayingMessage = false;
        cooldown = true;
        Invoke("EndCooldown", CooldownTime);
    }

    public new void StopMessage() {
        base.StopMessage();
        cooldown = true;
        Invoke("EndCooldown", CooldownTime);
    }

    private void EndCooldown() {
        cooldown = false;
    }
}
