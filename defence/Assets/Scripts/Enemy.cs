using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;

    private Transform target;
    private int waypointIndex = 0;

    void Start()
    {
        target = WayPoint.points[0];
    }

    void Update()
    {
        EnemyMove();
    }

    void EnemyMove()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) <= 0.3f)
        {
            GetNextWayPoint();
        }
    }

    void GetNextWayPoint()
    {
        if(waypointIndex >= WayPoint.points.Length - 1)
        {
            Destroy(gameObject);
            return;
        }

        waypointIndex++;
        target = WayPoint.points[waypointIndex];
    }
}
