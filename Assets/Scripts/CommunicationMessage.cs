﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class CommunicationMessage : MonoBehaviour
{
    public bool FacePlayer = true;
    public GameObject FollowObject;
    public bool Follow = false;
    public Vector3 RelativePosition;
    public float DisappearTime = 0f;
    public string Text;
    public string Title;
    public AudioClip MyAudioClip;
    public float Size;
    public TextMeshProUGUI Tmp;
    public TextMeshProUGUI TitleTmp;
    public float TextSize;
    public float TitleTextSize;

    public float CameraPositioning;

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

        Tmp.text = Text;
        if (Title != "") {
            TitleTmp.text = Title;
            TitleTmp.fontSize = TitleTextSize;
        }
        Tmp.fontSize = TextSize;
        startAlphaText = Tmp.alpha;
        img = GetComponentInChildren<Image>();
        if(img != null)
            startAlphaImage = img.color.a;

        myAudioSource = gameObject.AddComponent<AudioSource>();
        AudioMixer mixer = Resources.Load("MixerGroups/Ambience") as AudioMixer;
        myAudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Instructions")[0];
        if(MyAudioClip != null)
            myAudioSource.PlayOneShot(MyAudioClip);

        fadeTimer = CommunicationManager.Instance.DisappearTime;
        transform.localScale = new Vector3(Size, Size, Size);
        DoFollow();
    }

    // Update is called once per frame
    void Update()
    {
        if (Follow)
            DoFollow();

        if (Camera.main != null) {
            Vector3 targetDirection = transform.position - Camera.main.transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, 1f, 0f));
        }

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

    /// <summary>
    /// Update position based on follow object
    /// </summary>
    private void DoFollow() {
        Vector3 dir = Vector3.zero;
        if (CameraPositioning != 0f) {
            Vector3 heading = Camera.main.transform.position - FollowObject.transform.position;
            dir = Vector3.Cross(heading, Vector3.up).normalized * CameraPositioning;
        }

        transform.position = FollowObject.transform.position + RelativePosition + dir;

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
    }

    public void StartFade() {
        fade = true;
    }
}
