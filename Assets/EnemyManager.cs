using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform respawn;
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;


    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemy;

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0)
        {
            CreateEnemy();
            spawnTimer = spawnCooldown;
        }
    }

    private void CreateEnemy()
    {
        GameObject newEnemy = Instantiate(basicEnemy, respawn.position, Quaternion.identity);
    }
}
