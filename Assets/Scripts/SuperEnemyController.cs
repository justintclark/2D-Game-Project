using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEnemyController : MonoBehaviour
{
    public int MaxHP;
    public int CurrentHP;
    private Animator anim;
    private bool dead;
    public GameObject player;
    public int attackDamage;
    HeroKnight playerHealth;
    public float timer = 2.0f;
    public int receivedDamage;
    public float distToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        CurrentHP = MaxHP;
        dead = false;
        playerHealth = player.GetComponent<HeroKnight>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EnemyAttack();

        if (!dead && CurrentHP <= 0)
            Die();

        timer -= Time.deltaTime;

        distToPlayer = Vector2.Distance(player.transform.position, transform.position);
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            if (player.GetComponent<HeroKnight>().isAttacking)
            {
                CurrentHP -= receivedDamage * player.GetComponent<HeroKnight>().AttackDamage;
            }
        }

        if (other.gameObject.CompareTag("Boundary"))
            Die();
    }

    void EnemyAttack()
    {
        if (!dead && distToPlayer <= 2.5f && timer <= 0)
        {
            anim.SetTrigger("SpearAttack");

            if (playerHealth.currentHP > 0 && distToPlayer <= 2.5f)
                playerHealth.TakeDamage(attackDamage);

            if (timer <= 0)
                timer = 2.0f;
        }
    }


    void Die()
    {
        dead = true;

        anim.SetBool("SuperDie", true);

        Destroy(gameObject, 1f);
    }
}
