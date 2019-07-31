using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

class LevelManager:MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnSettings
    {
        [Header("Position")]
        public Transform[] wayPoints;
        [Header("EnemyPrefeb")]
        public GameObject[] enemyPrefebs;
        [Header("EnemyParent")]
        public Transform enemyParent;
        public Transform enemyGuardPoints;
    }



    public GameObject networkProcess;
    public int level;

    [Header("EnemyAbout")]
    public EnemySpawnSettings enemySpawnSettings;
    
    


    private void Awake()
    {
        
    }

    private void Start()
    {
        level = 0;
    }
    private void Update()
    {
        if(networkProcess.GetComponent<NetWorkProcess>().gameStart == true)
        {
            // 发起关卡请求
            GameObject[] enemies =  GameObject.FindGameObjectsWithTag("Ragdoll");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if(enemies.Length > 0)
            {
                // 正在通关中
                // 同步所有enemy的信息

            }else if(enemies.Length == 0 && players.Length > 0)
            {
                // 通过这一关，进入下一关    
                //StartCoroutine(StopWhile(10));
                level++;
                LevelInfo levelInfo = new LevelInfo();
                levelInfo.level = level;
                string levelInfoJson = JsonConvert.SerializeObject(levelInfo);
                networkProcess.GetComponent<NetWorkProcess>().WriteSocket(levelInfoJson);
                string levelReturnJson = networkProcess.GetComponent<NetWorkProcess>().ReadSocket();
                Debug.LogFormat("levelReturnJson:{0}", levelReturnJson);
                MonsterRoot monsterRoot = JsonConvert.DeserializeObject<MonsterRoot>(levelReturnJson);
                level = monsterRoot.level;
                foreach (var monsterItem in monsterRoot.monsters)
                {
                    if (monsterItem.monsterType == 1)
                    {
                        // 生成机枪兵
                        GameObject enemy = Instantiate(enemySpawnSettings.enemyPrefebs[0]) as GameObject;
                        enemy.GetComponent<Rigidbody>().isKinematic = false;
                        //enemy.transform.SetParent(enemySpawnSettings.enemyParent);
                        enemy.transform.position = new Vector3(
                            enemySpawnSettings.enemyParent.position.x+ Random.Range(0, 10),
                            0, enemySpawnSettings
                            .enemyParent.position.x+Random.Range(0, 10));
                        for (int i = 0; i < monsterItem.monsterGuardPoints.Count; i++)
                        {
                            //GameObject t = new GameObject("t");
                            //t.transform.position = new Vector3(monsterItem.monsterGuardPoints[i][0], 0.24f,
                            //    monsterItem.monsterGuardPoints[i][1]);
                            //Debug.LogFormat("t.transform.position:{0}", t.transform.position);
                            //enemy.GetComponent<EnemyAI>().guardSettings.wayPoints[i] = new WayPointBase(t.transform);
                            enemy.GetComponent<EnemyAI>().guardSettings.wayPoints[i] = new WayPointBase(
                                enemySpawnSettings.enemyGuardPoints.GetComponentsInChildren<Transform>()[Random.Range(0, 10)].transform
                                );
                        }
                    }
                    else
                    {
                        // 生成爆破兵
                    }
                }
            }
        }
        else if(networkProcess.GetComponent<NetWorkProcess>().gameStart == false)
        {
            
        }
    }



    // 布置场景, 生成敌人
    void PrepareLevelScene()
    {

    }

    IEnumerator StopWhile(float delta)
    {
        yield return new WaitForSeconds(delta);
    }

}

