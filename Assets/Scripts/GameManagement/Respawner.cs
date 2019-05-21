using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [System.Serializable]
    public class RespawnSettings
    {
        public WayPointBase[] wanpoints;
        public Transform respawnPlace;
        public GameObject enemyPrefeb;
        public int maxEnemyAmount = 10;
        public float respawnRate = 15.0f;
    }
    [SerializeField]
    public RespawnSettings respawnSettings;
    int enemyCurrentAmount = 0;
    float lastRespawnTime;
    int zoffets = 0;

    private void Start()
    {
        for (int i = 0; i < respawnSettings.wanpoints.Length; i++)
        {
            respawnSettings.wanpoints[i].waitTime = Random.Range(1.1f, 5.0f);
        }
        lastRespawnTime = Time.time;
    }

    private void Update()
    {

    }
    void RespawnEnemy()
    {
        if (enemyCurrentAmount <= respawnSettings.maxEnemyAmount)
        {
            GameObject newEnemy = Instantiate<GameObject>(respawnSettings.enemyPrefeb, respawnSettings.respawnPlace.position, respawnSettings.respawnPlace.rotation, transform);
            EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();
            for (int i = 0; i < enemyAI.guardSettings.wayPoints.Length; i++)
            {
                enemyAI.guardSettings.wayPoints[i] = respawnSettings.wanpoints[(i + zoffets) % respawnSettings.wanpoints.Length];
            }
            EnemyStates enemyStates = newEnemy.GetComponent<EnemyStates>();
            
        }
    }
}