using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates:MonoBehaviour{
    [Range(0, 100)]
    public float health = 100.0f;
    public int ID;
    public string playerName{get;set;}
    public string password{get; set;}
    public WeaponHandler weaponHandler;
    public bool isInTank;

    Animator animator;

    private PlayerUI playerUI;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        animator = GetComponent<Animator>();
    }

    public void ApplyDamage(float number){
        health -= number;
        playerUI.damgeReact.GetComponent<CanvasGroup>().alpha = 1;
        if (health <= 0)
        {
            animator.SetBool("die",true);
            Destroy(this.gameObject, 3);
            Debug.Log("You are dead...");
            GameObject.FindGameObjectWithTag("Finish").GetComponent<GameOver>().LeftGame();
        }
        if (health > 100)
        {
            health = 100;
        }
    }
}