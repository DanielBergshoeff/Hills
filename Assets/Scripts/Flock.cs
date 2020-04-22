using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public GlobalFlock MyFlock;

    public float Speed = 0.5f;
    public float MinSpeed = 1f;
    public float MaxSpeed = 2f;
    public float RotationSpeed = 4f;
    private Vector3 averageHeading;
    private Vector3 averagePosition;

    private float neighbourDistance = 5f;
    private float minDistance = 0.1f;

    private bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        Speed = Random.Range(MinSpeed, MaxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!MyFlock.MyCollider.bounds.Contains(transform.position))
            turning = true;
        else 
            turning = false;

        if (turning) {
            Vector3 direction = MyFlock.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);

            Speed = Random.Range(MinSpeed, MaxSpeed);
            Speed += MyFlock.SpeedAdjustment;
        }
        else {
            if (Random.Range(0, 5) < 1) {
                ApplyRules();
            }
        }
        
        transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    private void ApplyRules() {
        int groupSize = 0;
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;
        float sqrDist = 0f;

        foreach(Flock flock in MyFlock.AllFish) {
            if (flock == this)
                continue;

            sqrDist = (flock.transform.position - transform.position).sqrMagnitude;
            if (sqrDist > neighbourDistance * neighbourDistance)
                continue;

            vcentre += flock.transform.position;
            groupSize++;

            if(sqrDist < minDistance * minDistance) {
                vavoid += transform.position - flock.transform.position;
            }
            
            gSpeed = gSpeed + flock.Speed;
        }

        if (groupSize <= 0)
            return;

        vcentre = vcentre / groupSize + (MyFlock.GoalPosition - transform.position);
        Speed = gSpeed / groupSize;
        Speed += MyFlock.SpeedAdjustment;

        Vector3 direction = (vcentre + vavoid) - transform.position;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);
    }
}
