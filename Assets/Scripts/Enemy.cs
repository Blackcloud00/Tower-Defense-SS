using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour , IDamagable
{
    private NavMeshAgent agent;

    [SerializeField] private Transform centerPoint;
    public int healthPoint = 4;

    [Header("Movement")]
    [SerializeField] private float turnSpeed = 10;
    [SerializeField] private Transform[] waypoints;
    private int waypointIndex = 0;

    [Space]
    private float totalDistance;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);
    }

    private void Start()
    {
        waypoints = FindAnyObjectByType<WaypointManager>().GetWaypoints();

        CollectTotalDistance();
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);

        // Check if the agent is close to current target point
        if (agent.remainingDistance < .5f)
        {
            //Set the destination to the waypoint
            agent.SetDestination(GetNextWaypoint());
        }
    }

    public float DistanceToFinishLine() => totalDistance + agent.remainingDistance;

    private void CollectTotalDistance()
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            float distance = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            totalDistance += distance;
        }
    }
    private void FaceTarget(Vector3 newTarget) 
    { 
        //Calculate the direction from current position to the new target
        Vector3 directionToTarget = newTarget - transform.position;
        directionToTarget.y = 0; // Keep only horizontal direction

        // Create a rotation that points the forward vector up the calculated direction
        Quaternion newRotation = Quaternion.LookRotation(directionToTarget);

        // Smoothly rotate from the current rotation to the target rotation at the defined speed // Time.deltaTime makes it frame rate independent
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime);
    }

    private Vector3 GetNextWaypoint()
    {
        if (waypointIndex >= waypoints.Length)
        {
            //waypointIndex = 0;
            return transform.position;
        }
        Vector3 targetPoint = waypoints[waypointIndex].position;

        if(waypointIndex > 0)
        {
            float distance = Vector3.Distance(waypoints[waypointIndex].position, waypoints[waypointIndex - 1].position);
            totalDistance = totalDistance - distance;
        }

        waypointIndex++;

        return targetPoint;
    }

    public Vector3 GetCenterPoint() => centerPoint.position;

    public void TakeDamage(int damage)
    {
        healthPoint = healthPoint - damage;

        if (healthPoint <= 0)
            Destroy(gameObject);
    }
}


