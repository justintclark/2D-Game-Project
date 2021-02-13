using UnityEngine;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    public bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    public bool isDead;
    public bool isAttacking;
    private bool isBlocking;
    private Collider2D sword;
    private int damageTaken;
    public bool damaged;
    public int maxHP;
    public int currentHP;
    private float jumpTimer = 0f;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI swordText;
    public GameObject DeathText;
    public GameObject DeathImage;
    public int AttackDamage;
    private int numberOfSwordsCollected = 0; 


    // Use this for initialization
    void Start ()
    {
        isAttacking = false;
        isDead = false;
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        sword = GetComponent<CapsuleCollider2D>();
        sword.enabled = false;
        damaged = false;
        maxHP = 100;
        currentHP = 100;
        damageTaken = 1;
        isBlocking = false;
        AttackDamage = 1;
        DeathImage.SetActive(false);
        DeathText.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isDead)
        {
            m_timeSinceAttack += Time.deltaTime;
            jumpTimer -= Time.deltaTime;

            Attack();
            Roll();
            BlockDamage();
            Jump();
            Run();
            
            SetUIText();

            m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
        }

        if (!isDead && damaged && !isBlocking)
        {

            StartCoroutine("Flinch");

        }

        if (isDead)
        {
            DeathImage.SetActive(true);
            DeathText.SetActive(true);
            ReloadLevel();
        }
    }

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        m_rolling = false;
    }

    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (m_timeSinceAttack > 0.25f && !isDead)
            {
                m_currentAttack++;
                StartCoroutine("Attacking");

                // Loop back to one after third attack
                if (m_currentAttack > 3)
                    m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (m_timeSinceAttack > 1.0f)
                    m_currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack" + m_currentAttack);

                // Reset timer
                m_timeSinceAttack = 0.0f;
            }
        }
    }

    IEnumerator Attacking()
    {
        isAttacking = true;
        sword.enabled = true;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        sword.enabled = false;
    }

    void BlockDamage()
    {
        if (Input.GetMouseButtonDown(1) && !isDead)
        {
            isBlocking = true;
            damaged = false;
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            damageTaken *= 0;
        }

        else if (Input.GetMouseButtonUp(1) && !isDead)
        {
            isBlocking = false;
            m_animator.SetBool("IdleBlock", false);
            damageTaken = 1;
        }
    }

    void Roll()
    {
        if (Input.GetKeyDown("left shift") && !m_rolling && !isDead)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_rolling = false;
            // m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown("space") && m_grounded && !isDead && jumpTimer <= 0)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
            jumpTimer = 1.25f;
        }

        if (m_grounded && !m_groundSensor.State() && !isDead)
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (!m_grounded && m_groundSensor.State() && !isDead)
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boundary") && !isDead)
        {
            Die();
        }

        if (other.gameObject.CompareTag("heart") && !isDead)
        {
            currentHP += 25;
            if (currentHP > maxHP)
                currentHP = maxHP;
        }

        if (other.gameObject.CompareTag("sword") && !isDead)
        {
            numberOfSwordsCollected = 1 + numberOfSwordsCollected; 
            AttackDamage += numberOfSwordsCollected; 
        }
    }

    void Run()
    {
        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && !isDead)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0 && !isDead)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling && !isDead)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        if (Mathf.Abs(inputX) > Mathf.Epsilon && !isDead)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    public void TakeDamage(int amount)
    {
        damaged = true;
        currentHP -= amount * damageTaken;

        if (currentHP <= 0 && !isDead)
            Die();
    }

    void Die()
    {
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");
        isDead = true;
        damaged = false;
    }

    void SetUIText()
    {
        lifeText.text = currentHP.ToString() + "%";
        swordText.text = "+ " + numberOfSwordsCollected.ToString();
    }

    IEnumerator Flinch()
    {
        yield return new WaitForSeconds(0.2f);
        m_animator.SetTrigger("Hurt");
        damaged = false;
    }

    void ReloadLevel()
    {
        if (Input.GetButton("Jump"))
            SceneManager.LoadScene("MainGame");
    }
}
