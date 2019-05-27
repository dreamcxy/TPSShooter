using System.Collections;
using System.Collections.Generic;

using UnityEngine;



public class GameOver : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            Debug.Log("game over...");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ragdoll");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyStates>().EnemyDie();
                LeftGame();
            }
        }
    }    
    //离开游戏，摧毁所有，上传状态
    public void LeftGame()
    {
        // SceneManager.LoadScene(0);
    }
}