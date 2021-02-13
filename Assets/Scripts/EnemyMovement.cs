using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public int currentWaypoint = 0;
    public Transform player;
    public Transform target;
    private float rangeToDest = 1f;
    private float rangeToPlayer = 5f;
    private Animator anim;
    private bool facingRight;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, player.position) <= rangeToPlayer)
            Chase();
        else
            Patrol();
    }

    void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, 0.01f);
        anim.SetTrigger("Walk");

        if (player.transform.position.x > transform.position.x) //if the target is to the right of enemy and the enemy is not facing right
            transform.eulerAngles = new Vector2(0, 0);
        if (player.transform.position.x < transform.position.x)
            transform.eulerAngles = new Vector2(0, 180);

    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, 0.01f);
        target = waypoints[currentWaypoint];


        if (Vector2.Distance(transform.position, target.position) <= rangeToDest)
        {
            if (currentWaypoint == 1)
                currentWaypoint = 0;
            else
                currentWaypoint = 1;
        }

        if (currentWaypoint == 1)
            transform.eulerAngles = new Vector2(0, 0);
        else
            transform.eulerAngles = new Vector2(0, 180);
        anim.SetTrigger("Walk");
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

}