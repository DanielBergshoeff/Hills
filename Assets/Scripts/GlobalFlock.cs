using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject FishPrefab;
    public Vector3 TankSize = new Vector3(10f, 10f, 10f);

    [SerializeField] private int numFish = 10;
    public List<Flock> AllFish;
    public Vector3 GoalPosition = Vector3.zero;
    public bool UpdatePosition = true;

    public Collider MyCollider;

    // Start is called before the first frame update
    void Start()
    {
        AllFish = new List<Flock>();
        for (int i = 0; i < numFish; i++) {
            Vector3 pos = GetRandomPositionInTank();
            AllFish.Add(Instantiate(FishPrefab, pos, Quaternion.identity).GetComponent<Flock>());
            AllFish[i].MyFlock = this;
            AllFish[i].transform.parent = transform;
        }

        MyCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UpdatePosition) {
            GoalPosition = transform.position;
            return;
        }

        if(Random.Range(0, 10000) < 50) {
            GoalPosition = GetRandomPositionInTank();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(transform.position, TankSize);
        Gizmos.DrawSphere(GoalPosition, 0.1f);
    }

    private Vector3 GetRandomPositionInTank() {
        Vector3 pos = transform.position + new Vector3(Random.Range(-TankSize.x, TankSize.x),
                                                            Random.Range(-TankSize.y, TankSize.y),
                                                            Random.Range(-TankSize.z, TankSize.z)) / 2f;
        return pos;
    }
}
