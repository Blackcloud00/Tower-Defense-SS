using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform currentEnemy;

    [SerializeField] protected float attackCooldown = 1f;
    protected float lastTimeAttacked;

    [Header("Tower Setup")]
    [SerializeField] protected Transform towerHead;
    [SerializeField] protected float rotationSpeed = 10f;
    private bool canRotate = true;

    [SerializeField] protected float attackRange = 2.5f;
    [SerializeField] protected LayerMask whatIsEnemy;

    protected virtual void Awake()
    {

    }
    protected virtual void Update()
    {
        if (currentEnemy == null)
        {
            currentEnemy = FindRandomEnemyWithinRange();
            return;
        }

        if (CanAttack())
            Attack();

        if (Vector3.Distance(currentEnemy.position, transform.position) > attackRange)
            currentEnemy = null;

        RotateTowardsEnemy();
    }

    protected virtual void Attack()
    {
        //Debug.Log("Tower Attacked! +" + Time.time );
    }

    protected bool CanAttack()
    {
        if (Time.time > lastTimeAttacked + attackCooldown)
        {
            lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }
    protected Transform FindRandomEnemyWithinRange()
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

    public void EnableRotation(bool enable)
    {
        canRotate = enable;
    }
    protected virtual void RotateTowardsEnemy()
    {
        if (currentEnemy == null || !canRotate)
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

    protected Vector3 DirectionToEnemy(Transform startPoint)
    {
        return (currentEnemy.position - startPoint.position).normalized;
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
