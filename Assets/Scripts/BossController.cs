using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossController : MonoBehaviour
{

    public float attackTimer;
    public float distToPlayer;
    public GameObject player;
    public GameObject distCheck;
    public GameObject gameWinText;
    public int bossDamage;
    public int maxHP;
    public int currentHP;
    private Animator anim;
    private bool bossAttacking;
    private bool facingRight;
    private bool bossDead;
    private int receivedDamage = 20;

    // Start is called before the first frame update
    void Start()
    {
        attackTimer = 3.0f;
        anim = GetComponent<Animator>();
        bossAttacking = false;
        currentHP = maxHP;
        gameWinText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossDead && !player.GetComponent<HeroKnight>().isDead)
        {
            ChasePlayer();
            BossTakeDamage();

            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                BossAttack();
                attackTimer = 3.0f;
            }

            distToPlayer = Vector2.Distance(player.transform.position, distCheck.transform.position);
        }

        if (!bossDead && currentHP <= 0)
                BossDie();

        if (bossDead)
            gameWinText.SetActive(true);
    }

    void BossAttack()
    {
        if (distToPlayer <= 2.2f)
        {
            int atk = Random.Range(1, 3);

            StartCoroutine("BossAttacking");

            if (atk == 1)
                anim.SetTrigger("BossAttack2");

            if (atk == 2)
                anim.SetTrigger("BossAttack4");

            if (atk == 3)
                anim.SetTrigger("BossAttack5");
        }
    }

    IEnumerator BossAttacking()
    {
        bossAttacking = true;
        yield return new WaitForSeconds(0.75f);
        player.GetComponent<HeroKnight>().TakeDamage(bossDamage);
        bossAttacking = false;
    }

    void ChasePlayer()
    {
        if (distToPlayer <= 10)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 0.01f);
            anim.SetTrigger("BossWalk");
        }

        if (player.transform.position.x > transform.position.x && facingRight) //if the target is to the right of enemy and the enemy is not facing right
            Flip();
        if (player.transform.position.x < transform.position.x && !facingRight)
            Flip();
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }


    void BossTakeDamage()
    {
        if (!bossDead && !player.GetComponent<HeroKnight>().isDead && player.GetComponent<HeroKnight>().isAttacking && distToPlayer <= 2.0f)
        {
            currentHP -= receivedDamage * player.GetComponent<HeroKnight>().AttackDamage;
        }
    }

    void BossDie()
    {
        anim.SetBool("BossIsDead", true);
        bossDead = true;
        Destroy(gameObject, 1.5f);
    }



    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("Boundary"))
        {
            BossDie();
        }
    }
}
