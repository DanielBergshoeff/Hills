using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicinityMessage : MonoBehaviour
{
    public float MaxRange = 10f;
    public string Message = "This is an example text. Use it wisely.";
    public string MessageDutch = "Dit is een voorbeeld text. Gebruik het wijslijk.";
    public float Size = 0.075f;
    public float TextSize = 10f;
    public float MessageDuration = 10f;
    public float CooldownTime = 90f;
    public Vector3 Position = Vector3.up * 0.2f;
    public bool FollowObject = true;

    private CommunicationMessage myMessage;
    private bool displayingMessage = false;
    private bool cooldown = false;
    

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

            myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, msg, null, MessageDuration, Position, Size, FollowObject, TextSize);
            displayingMessage = true;
            Invoke("RevertDisplayingMessage", MessageDuration);
        }
    }

    private void RevertDisplayingMessage() {
        displayingMessage = false;
        cooldown = true;
        Invoke("EndCooldown", CooldownTime);
    }

    public void StopMessage() {
        if(myMessage != null)
            myMessage.StartFade();
        displayingMessage = false;
        cooldown = true;
        Invoke("EndCooldown", CooldownTime);
    }

    private void EndCooldown() {
        cooldown = false;
    }
}
