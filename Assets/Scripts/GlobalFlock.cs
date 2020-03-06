using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject flockPrefab;
    public Vector3 TankSize;
    public int NumFish = 10;

    public static GameObject[] AllFish;
    public static Vector3 TankSizeStatic;
    public static Vector3 GoalPosition = Vector3.zero;
    public static Vector3 GlobalFlockPosition;


    // Start is called before the first frame update
    void Start()
    {
        AllFish = new GameObject[NumFish];
        for (int i = 0; i < NumFish; i++)
        {
            Vector3 pos = transform.position + new Vector3(Random.Range(-TankSize.x / 2f, TankSize.x / 2f),
                Random.Range(-TankSize.y / 2f, TankSize.y / 2f),
                Random.Range(-TankSize.z / 2f, TankSize.z / 2f));
            AllFish[i] = (GameObject)Instantiate(flockPrefab, pos, Quaternion.identity);
        }

        TankSizeStatic = TankSize;
        GlobalFlockPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    { 
        if(Random.Range(0, 10000) < 50)
        {
            GoalPosition = transform.position + new Vector3(Random.Range(-TankSize.x / 2f, TankSize.x / 2f),
                Random.Range(-TankSize.y / 2f, TankSize.y / 2f),
                Random.Range(-TankSize.z / 2f, TankSize.z / 2f));
        }
    }
}
