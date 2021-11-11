using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static bool battleOver;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask player;
    public Movement movement;
    public bool block = false;
    public float health = 100;
    public Animator anim;
    public float nextAttackTime = 0f;
    public float attackRate = 1f;
    // Update is called once per frame
    private void Start()
    {
        battleOver = false;
        nextAttackTime = Time.time;
        
    }
    void Update()
    {
        if (!battleOver)
        {
            

            if (movement.team == 0)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    block = true;
                    anim.SetBool("Block", block);
                }
                else if (Input.GetKeyDown(KeyCode.Z) && !block)
                {
                    if (Time.time >= nextAttackTime)
                    {
                        anim.SetTrigger("Attack");
                        nextAttackTime = Time.time + 1f / attackRate;
                    }

                }
                if (Input.GetKeyUp(KeyCode.X))
                {
                    block = false;
                    anim.SetBool("Block", block);
                }
            }
            if (movement.team == 1)
            {
                if (Input.GetKeyDown(KeyCode.N))
                {
                    block = true;
                    anim.SetBool("Block", block);
                }
                else if (Input.GetKeyDown(KeyCode.M) && !block)
                {
                    if (Time.time >= nextAttackTime)
                    {
                        anim.SetTrigger("Attack");
                        nextAttackTime = Time.time + 1f / attackRate;
                    }

                }
                if (Input.GetKeyUp(KeyCode.N))
                {
                    block = false;
                    anim.SetBool("Block", block);
                }
            }
            
        }
    }

    void Attack()
    {
        //animation
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, player);
       foreach(Collider2D enemy in hitEnemies)
        {
            if(enemy.name != gameObject.name && enemy.gameObject.GetComponent<Movement>() != null)
            {
                if (gameObject.transform.localScale.x != enemy.transform.localScale.x)
                {
                    if (!enemy.GetComponent<PlayerCombat>().block)
                    {
                        enemy.GetComponent<PlayerCombat>().TakeDamage(10f);
                    }
                }
                else
                {
                    enemy.GetComponent<PlayerCombat>().TakeDamage(15f);
                }
                
            }
        }
       
    }
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (movement.team == 0)
        {
            if (health <= 0)
            {
                anim.SetTrigger("Death");
                battleOver = true;
                Chessboard.instance.winnerCombat = Chessboard.instance.BlackTeamFighter;
                Chessboard.instance.loserCombat = Chessboard.instance.whiteTeamFighter;
                Chessboard.instance.chessPieces[Chessboard.instance.winnerCombat.x, Chessboard.instance.winnerCombat.y].health = GameObject.Find("1 Team").GetComponent<Movement>().health;

                StartCoroutine(sceneManager.instance.LoadChess());
            }
        }
        if (movement.team == 1)
        {
            if (health <= 0)
            {
                anim.SetTrigger("Death");
                battleOver = true;
                Chessboard.instance.winnerCombat = Chessboard.instance.whiteTeamFighter;
                Chessboard.instance.loserCombat = Chessboard.instance.BlackTeamFighter;
                Chessboard.instance.chessPieces[Chessboard.instance.winnerCombat.x, Chessboard.instance.winnerCombat.y].health = GameObject.Find("0 Team").GetComponent<Movement>().health;

                StartCoroutine(sceneManager.instance.LoadChess());
            }
        }
        if (health > 0)
        {
            anim.SetTrigger("Hurt");
        }
    }
}
