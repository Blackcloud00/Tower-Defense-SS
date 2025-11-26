using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform currentEnemy;

    [Header("Tower Setup")]
    [SerializeField] private Transform towerHead;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask whatIsEnemy;
    private void Update()
    {
        if (currentEnemy == null)
        {
            currentEnemy = FindRandomEnemyWithinRange();
            return;
        }

        if(Vector3.Distance(currentEnemy.position, transform.position) > attackRange)
            currentEnemy = null;

        RotateTowardsEnemy();
    }

    
    private Transform FindRandomEnemyWithinRange()
    {
        List<Transform> possibleTargets = new List<Transform>();
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy);

        foreach (Collider enemy in enemiesAround)
        {
            possibleTargets.Add(enemy.transform);
        }

        int randomIndex = Random.Range(0, possibleTargets.Count);

        if(possibleTargets.Count <= 0)
            return null;

        return possibleTargets[randomIndex];
    }
    
    private void RotateTowardsEnemy()
    {
        if (currentEnemy == null)
            return;

        //Calculate the vector direction from the tower's head to the current enemy.
        Vector3 directionToEnemy = currentEnemy.position - towerHead.position;

        //Create a Quaternion for the rotation towards the enemy, bases on the diretion vector.
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        //Interpolate smoothly between the tower's current rotation and the look rotation.
        //'rotationSpeed * Time.deltaTime' adjust the speed of rotation to be frame-rate independent.
        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;

        //Apply the interpolated rotation to the tower's head. This step converts the Quaternion back to Euler angles for straightforward application.
        towerHead.rotation = Quaternion.Euler(rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
