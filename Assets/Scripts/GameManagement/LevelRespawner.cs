// 根据关卡放置enemy

using UnityEngine;


class LevelRespawner : MonoBehaviour
{

    [System.Serializable]
    public class LevelRespawnSettings
    {
        public WayPointBase[] wayPoints;
        public Transform[] respawnPlace;
        public GameObject[] enemyPrefeb;

        public float level;

    }

    [SerializeField]
    public LevelRespawnSettings levelRespawnSettings;

    int enemyCurrentAmount = 0;
    int zoffets = 0;
    int curLevle = 1;

    private void Start()
    {

    }

    private void Update()
    {
        if (enemyCurrentAmount == 0)
        {
            if (true)
            {
                curLevle++;
                RespawnEnemyBasedLevel(curLevle);
            }
        }
    }

    void RespawnEnemyBasedLevel(int level)
    {
        for (int i = 0; i < level; i++)
        {
            GameObject newEnemy = Instantiate<GameObject>(levelRespawnSettings.enemyPrefeb[zoffets % levelRespawnSettings.enemyPrefeb.Length]
            , levelRespawnSettings.respawnPlace[zoffets % levelRespawnSettings.respawnPlace.Length].position,
             levelRespawnSettings.respawnPlace[zoffets % levelRespawnSettings.respawnPlace.Length].rotation, transform);
            zoffets++;
        }

    }
}