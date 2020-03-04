using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public float Speed = 1f;
    public float NeighbourDistance = 2.0f;

    private float rotationSpeed = 4.0f;
    private Vector3 averageHeading;
    private Vector3 averagePosition;

    // Start is called before the first frame update
    void Start()
    {
        Speed = Random.Range(0.5f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        bool turning = false;
        Vector3 difference = transform.position - GlobalFlock.GlobalFlockPosition;
        if (difference.x > GlobalFlock.GlobalFlockPosition.x + GlobalFlock.TankSizeStatic.x / 2f ||
            difference.x < GlobalFlock.GlobalFlockPosition.x - GlobalFlock.TankSizeStatic.x / 2f ||
            difference.y > GlobalFlock.GlobalFlockPosition.y + GlobalFlock.TankSizeStatic.y / 2f ||
            difference.y < GlobalFlock.GlobalFlockPosition.y - GlobalFlock.TankSizeStatic.y / 2f ||
            difference.z > GlobalFlock.GlobalFlockPosition.z + GlobalFlock.TankSizeStatic.z / 2f ||
            difference.z < GlobalFlock.GlobalFlockPosition.z - GlobalFlock.TankSizeStatic.z / 2f)
            turning = true;
        else
            turning = false;

        if (turning)
        {
            Vector3 direction = GlobalFlock.GlobalFlockPosition - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            Speed = Random.Range(0.5f, 1f);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
                ApplyRules();
        }
        transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    private void ApplyRules()
    {
        GameObject[] gos;
        gos = GlobalFlock.AllFish;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;

        Vector3 goalPos = GlobalFlock.GoalPosition;
        float dist;

        int groupSize = 0;
        foreach(GameObject go in gos)
        {
            if(go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if(dist <= NeighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if(dist < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.Speed;
                }
            }
        }

        if(groupSize > 0)
        {
            vcentre = vcentre / groupSize + (goalPos - this.transform.position);
            Speed = gSpeed / groupSize;

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
        }
    }
}
