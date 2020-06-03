using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBud : MonoBehaviour
{
    [SerializeField] private GameObject flowerPrefab;

    public Color MyColor;

    private void OnCollisionEnter(Collision collision) {
        if (!(collision.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")))
            return;

        GameObject go = Instantiate(flowerPrefab);
        Flower f = go.GetComponent<Flower>();
        f.MyColor = MyColor;
        go.transform.position = transform.position;
        Destroy(gameObject);
    }
}
