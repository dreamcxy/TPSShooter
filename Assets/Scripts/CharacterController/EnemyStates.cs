using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyStates:MonoBehaviour{
    [Range(0, 100f)]   
    public float health = 100f;

    public Respawner thisRespawner;

    Animator animator;
    EnemyAI enemyAI;
    EnemyMovement enemyMovement;
    CharacterController characterController;
    private Rigidbody[] bodyParts;
    private Slider bloodSlider;
    bool isAlive = true;

    private void Start() {
        animator  = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        bodyParts = GetComponentsInChildren<Rigidbody>();
        enemyMovement = GetComponent<EnemyMovement>();
        characterController = GetComponent<CharacterController>();
        bloodSlider = GetComponentInChildren<Slider>();
        Debug.Log(bloodSlider);
        isAlive = true;
        EnableRagdoll(false);
    }
    private void Update() {
        health = Mathf.Clamp(health, 0, 100);
        if (bloodSlider)
        {
            bloodSlider.value = Mathf.Round(health);    
        }
        
    }

    public void ApplyDamage(float number, int direction = 0){
        if (isAlive)
        {
            animator.SetTrigger("TakeDamage");
            health -= number;
            if (health < 0)
            {
                health = 0;
                EnemyDie();
            }
            if (health > 100)
            {
                health = 100;
            }
        }else{
            
        }
        return;
    }

    public void EnemyDie(){
        animator.SetBool("die", true);
        isAlive = false;
        enemyAI.enabled = false;
        enemyMovement.enabled = false;
        characterController.enabled = false;

        EnableRagdoll(true);
        // animator.enabled = false;
        Destroy(this.gameObject, 3f);
        if (thisRespawner)
        {
            thisRespawner.AmountOne();
        }

    }

    void EnableRagdoll(bool value){
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].isKinematic = !value;
        }
    }

}
