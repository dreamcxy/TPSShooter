using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [System.Serializable]
    public class RespawnSettings
    {
        public WayPointBase[] waypoints;
        public Transform[] respawnPlace;
        public GameObject[] enemyPrefeb;
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
        // for (int i = 0; i < respawnSettings.waypoints.Length; i++)
        // {
        //     respawnSettings.waypoints[i].waitTime = Random.Range(1.1f, 5.0f);
        // }
        lastRespawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - lastRespawnTime > respawnSettings.respawnRate)
        {
            RespawnEnemy();
        }
    }
    void RespawnEnemy()
    {
        enemyCurrentAmount = GameObject.FindGameObjectsWithTag("Ammo").Length;
        if (enemyCurrentAmount <= respawnSettings.maxEnemyAmount)
        {   
            GameObject newEnemy = Instantiate<GameObject>(respawnSettings.enemyPrefeb[zoffets % respawnSettings.enemyPrefeb.Length], respawnSettings.respawnPlace[zoffets%respawnSettings.respawnPlace.Length].position, respawnSettings.respawnPlace[zoffets%respawnSettings.respawnPlace.Length].rotation, transform);
            zoffets ++;
            if (zoffets > 5)
            {
                zoffets = 0;
            }
            Destroy(newEnemy, 5);
            // EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();
            // for (int i = 0; i < enemyAI.guardSettings.wayPoints.Length; i++)
            // {
            //     enemyAI.guardSettings.wayPoints[i] = respawnSettings.waypoints[(i + zoffets) % respawnSettings.waypoints.Length];
            // }
            // EnemyStates enemyStates = newEnemy.GetComponent<EnemyStates>();
        }
    }

    public void AmountOne()
    {
        enemyCurrentAmount--;
        if (enemyCurrentAmount < 0)
        {
            enemyCurrentAmount = 0;
        }
    }
}