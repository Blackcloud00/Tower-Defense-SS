using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Crossbow : Tower
{
    private Crossbow_Visuals visuals;

    [Header("Crossbow details")]
    [SerializeField] private int damage;
    [SerializeField] private Transform gunPoint;

    protected override void Awake()
    {
        base.Awake();

        visuals = GetComponent<Crossbow_Visuals>();
    }

    protected override void Attack()
    {

        Vector3 directionToEnemy = DirectionToEnemy(gunPoint);

        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity))
        {
            towerHead.forward = directionToEnemy;

            visuals.PlayAttackVFX(gunPoint.position, hitInfo.point);
            visuals.PlayReloadFX(attackCooldown);

            IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();

            if (damagable != null)
                damagable.TakeDamage(damage);
        }
    }
}
