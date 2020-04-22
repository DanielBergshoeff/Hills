using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommunicationMessage : MonoBehaviour
{
    public bool FacePlayer = true;
    public GameObject FollowObject;
    public bool Follow = false;
    public Vector3 RelativePosition;
    public float DisappearTime = 0f;
    public string Text;
    public AudioClip Audio;
    public float Size;
    public TextMeshProUGUI Tmp;

    private Image img;
    public LayerMask TerrainLayer; 

    private bool fade;
    private float startAlphaText;
    private float startAlphaImage;
    private float fadeTimer = 0f;
    private AudioSource myAudioSource;

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

        myAudioSource = gameObject.AddComponent<AudioSource>();
        if(Audio != null)
            myAudioSource.PlayOneShot(Audio);

        fadeTimer = CommunicationManager.Instance.DisappearTime;
        transform.localScale = new Vector3(Size, Size, Size);
        DoFollow();
    }

    // Update is called once per frame
    void Update()
    {
        if (Follow)
            DoFollow();

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

    private void DoFollow() {
        transform.position = FollowObject.transform.position + RelativePosition;
        RaycastHit hit;
        float height = -100f;
        for (int i = -1; i < 2; i++) {
            if (Physics.Raycast(transform.position + Vector3.up * 100f + transform.forward * i * Size * 2.5f, Vector3.down, out hit, 200f, TerrainLayer)) {
                if (hit.point.y > height)
                    height = hit.point.y;
            }
        }
        if (height > transform.position.y - Size) {
            transform.position = new Vector3(transform.position.x, height + Size, transform.position.z);
        }

        /*
        Vector3 heading = transform.position - Camera.main.transform.position;
        if(Physics.SphereCast(Camera.main.transform.position, 0.5f, heading.normalized, out hit, 1000f)) {
            if (!hit.collider.CompareTag("Message")) {
                Vector3 hd = Camera.main.transform.position - hit.point;
                transform.position = hit.point + hd.normalized;
            }
        }*/
    }

    private void StartFade() {
        fade = true;
    }
}
