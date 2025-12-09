using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Enemy currentEnemy;


    [SerializeField] protected float attackCooldown = 1f;
    protected float lastTimeAttacked;

    [Header("Tower Setup")]
    [SerializeField] protected EnemyType enemyPriorityType = EnemyType.None;
    [SerializeField] protected Transform towerHead;
    [SerializeField] protected float rotationSpeed = 10f;
    private bool canRotate = true;

    [SerializeField] protected float attackRange = 2.5f;
    [SerializeField] protected LayerMask whatIsEnemy;

    [Space]
    [Tooltip("Enabling this allows tower to change target beetwen attacks")]
    [SerializeField] private bool dynamicTargetChange;

    private float targetCheckInterval = .1f;
    private float lastTimeCheckedTarget;

    protected virtual void Awake()
    {
        EnableRotation(true);
    }
    protected virtual void Update()
    {
        UpdateTargetIfNeeded();

        if (currentEnemy == null)
        {
            currentEnemy = FindEnemyWithinRange();
            return;
        }

        if (CanAttack())
            Attack();

        if (Vector3.Distance(currentEnemy.GetCenterPoint(), transform.position) > attackRange)
            currentEnemy = null;

        RotateTowardsEnemy();
    }

    private void UpdateTargetIfNeeded()
    {
        if (dynamicTargetChange == false)
            return;

        if(Time.time > lastTimeCheckedTarget + targetCheckInterval)
        {
            lastTimeCheckedTarget = Time.time;
            currentEnemy = FindEnemyWithinRange();
        }
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
    protected Enemy FindEnemyWithinRange()
    {
        List<Enemy> priorityTargets = new List<Enemy>();
        List<Enemy> possibleTargets = new List<Enemy>();

        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy);

        foreach (Collider enemy in enemiesAround)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            EnemyType newEnemyType = newEnemy.GetEnemyType();

            if (newEnemyType == enemyPriorityType)
                priorityTargets.Add(newEnemy);
            else
                possibleTargets.Add(newEnemy);
        }

        Enemy newTarget = GetMostAdvancedEnemy(possibleTargets);

        if (priorityTargets.Count > 0)
            return GetMostAdvancedEnemy(priorityTargets);

        if(possibleTargets.Count > 0)
            return GetMostAdvancedEnemy(possibleTargets);

        return null;
    }

    private static Enemy GetMostAdvancedEnemy(List<Enemy> targets)
    {
        Enemy mostAdvancedEnemy = null;
        float minRemainingDistance = float.MaxValue;

        foreach (Enemy enemy in targets)
        {
            float remainingDistance = enemy.DistanceToFinishLine();

            if (remainingDistance < minRemainingDistance)
            {
                minRemainingDistance = remainingDistance;
                mostAdvancedEnemy = enemy;
            }
        }

        return mostAdvancedEnemy;
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
        Vector3 directionToEnemy = DirectionToEnemy(towerHead);

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
        return (currentEnemy.GetCenterPoint() - startPoint.position).normalized;
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
