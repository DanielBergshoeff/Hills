using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommunicationMessage : MonoBehaviour
{
    public bool FacePlayer = true;
    public GameObject FollowObject;
    public Vector3 RelativePosition;
    public float DisappearTime = 0f;
    public string Text;
    public TextMeshProUGUI Tmp;
    public Image img;

    private bool fade;
    private float startAlphaText;
    private float startAlphaImage;
    private float fadeTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (DisappearTime > 0f)
            Invoke("StartFade", DisappearTime);

        Tmp = GetComponentInChildren<TextMeshProUGUI>();
        Tmp.text = Text;
        startAlphaText = Tmp.alpha;
        img = GetComponentInChildren<Image>();
        startAlphaImage = img.color.a;

        fadeTimer = CommunicationManager.Instance.DisappearTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = FollowObject.transform.position + RelativePosition;
        Vector3 targetDirection = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, 1f, 0f));

        if (!fade)
            return;

        fadeTimer -= Time.deltaTime;

        Tmp.alpha = (fadeTimer / CommunicationManager.Instance.DisappearTime) * startAlphaText;
        Color temp = img.color;
        temp.a = (fadeTimer / CommunicationManager.Instance.DisappearTime) * startAlphaImage;
        img.color = temp;

        if (Tmp.alpha <= 0f && img.color.a <= 0f)
            Destroy(gameObject);
    }

    private void StartFade() {
        fade = true;
    }
}
