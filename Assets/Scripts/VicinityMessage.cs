using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicinityMessage : MonoBehaviour
{
    public float MaxRange = 10f;
    public string Message = "This is an example text. Use it wisely.";
    public float Size = 0.075f;
    public float TextSize = 10f;
    public float MessageDuration = 10f;
    public float CooldownTime = 90f;
    public Vector3 Position = Vector3.up * 0.2f;

    private CommunicationMessage myMessage;
    private bool displayingMessage = false;
    private bool cooldown = false;
    

    // Update is called once per frame
    void Update()
    {
        if (displayingMessage || cooldown)
            return;

        if ((transform.position - Camera.main.transform.position).sqrMagnitude > MaxRange * MaxRange)
            return;

        Vector3 heading = transform.position - Camera.main.transform.position;
        float angle = Vector3.Angle(heading, Camera.main.transform.forward);

        if(angle < 20f) {
            myMessage = CommunicationManager.Instance.DisplayMessage(this.gameObject, Message, null, MessageDuration, Position, Size, true, TextSize);
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
        myMessage.StartFade();
        displayingMessage = false;
        cooldown = true;
        Invoke("EndCooldown", CooldownTime);
    }

    private void EndCooldown() {
        cooldown = false;
    }
}
