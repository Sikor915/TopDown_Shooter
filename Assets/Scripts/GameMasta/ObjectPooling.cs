using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ObjectPooling : Singleton<ObjectPooling>
{
    [Header("Pooled Projectiles")]
    public List<GameObject> pooledProjectiles;
    [Description("The projectile to be pooled")]
    public GameObject projectileToPool;
    public int amountToPoolProjectiles;

    [Header("Pooled Enemies")]
    public List<EnemyController> pooledEnemies;
    [Description("The enemy to be pooled")]
    public EnemyController enemyToPool;
    public int amountToPoolEnemies;

    void Start()
    {
        pooledProjectiles = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPoolProjectiles; i++)
        {
            tmp = Instantiate(projectileToPool);
            tmp.SetActive(false);
            pooledProjectiles.Add(tmp);
        }

        pooledEnemies = new List<EnemyController>();
        for (int i = 0; i < amountToPoolEnemies; i++)
        {
            tmp = Instantiate(enemyToPool.gameObject);
            tmp.SetActive(false);
            pooledEnemies.Add(tmp.GetComponent<EnemyController>());
        }
    }

    public GameObject GetPooledProjectile()
    {
        for (int i = 0; i < pooledProjectiles.Count; i++)
        {
            if (!pooledProjectiles[i].activeInHierarchy)
            {
                return pooledProjectiles[i];
            }
        }
        return null;
    }

    public void ReturnProjectileToPool(GameObject projectile)
    {
        projectile.SetActive(false);
    }

    public GameObject GetPooledEnemy()
    {
        for (int i = 0; i < pooledEnemies.Count; i++)
        {
            if (!pooledEnemies[i].gameObject.activeInHierarchy)
            {
                return pooledEnemies[i].gameObject;
            }
        }
        return null;
    }

    public void ReturnEnemyToPool(EnemyController enemy)
    {
        enemy.Reset();
        enemy.GetComponent<BasicEnemyAI>().DeactivateEnemy();
        enemy.gameObject.SetActive(false);
    }
}
