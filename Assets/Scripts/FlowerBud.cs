using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBud : MonoBehaviour
{
    [SerializeField] private GameObject flowerPrefab;

    private void OnCollisionEnter(Collision collision) {
        if (!(collision.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")))
            return;

        GameObject go = Instantiate(flowerPrefab);
        go.transform.position = transform.position;
        Destroy(gameObject);
    }
}
