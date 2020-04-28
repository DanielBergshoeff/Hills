using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BucketBallCounter : MonoBehaviour
{
    private int ballsInBucket = 0;
    private TextMeshProUGUI counterText;

    // Start is called before the first frame update
    void Start()
    {
        counterText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("SmallObject"))
            return;

        ballsInBucket++;
        counterText.text = ballsInBucket.ToString();
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("SmallObject"))
            return;

        ballsInBucket--;
        counterText.text = ballsInBucket.ToString();
    }
}
