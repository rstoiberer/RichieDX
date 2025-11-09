using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    // Toggle whether this Bandit reads player input
    [SerializeField] bool playerControlled = true;

    // --- Melee / Hitbox settings ---
    [Header("Attack Settings")]
    [SerializeField] Transform attackOrigin;          // assign a child transform near the weapon/front
    [SerializeField] Vector2 attackSize = new Vector2(1.2f, 0.7f);
    [SerializeField] LayerMask damageLayers;          // set to your "Damageable" (enemy) layer(s)
    [SerializeField] float attackDelay = 0.12f;       // delay from click to strike (sync with anim)
    [SerializeField] float attackActiveTime = 0.06f;  // window where the hitbox is “active”
    [SerializeField] bool killOnHit = true;           // simple: one hit kills

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool m_isDead = false;
    private bool m_isAttacking = false;               // prevents stacking attacks

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

        if (playerControlled && attackOrigin == null)
            Debug.LogWarning("[Bandit] AttackOrigin not assigned on player. Create a child and assign it.");
    }

    void Update()
    {
        // Landing
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Start falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input & movement --
        float inputX = playerControlled ? Input.GetAxis("Horizontal") : 0f;

        // Flip only if controlled
        if (playerControlled)
        {
            if (inputX > 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);
            else if (inputX < 0f)
                transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // Move
        m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        // Air speed for animator
        m_animator.SetFloat("AirSpeed", m_body2d.linearVelocity.y);

        // -- Handle Animations (only when controlled) --
        // Death / Recover (debug toggle)
        if (playerControlled && Input.GetKeyDown(KeyCode.E))
        {
            if (!m_isDead)
                Die(); // go straight to death
            else
            {
                m_animator.SetTrigger("Recover");
                m_isDead = false;
            }
        }
        // Hurt (debug)
        else if (playerControlled && Input.GetKeyDown(KeyCode.Q))
        {
            m_animator.SetTrigger("Hurt");
        }
        // Attack (LMB)
        else if (playerControlled && Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
        // Toggle combat idle
        else if (playerControlled && Input.GetKeyDown(KeyCode.F))
        {
            m_combatIdle = !m_combatIdle;
        }
        // Jump
        else if (playerControlled && Input.GetKeyDown(KeyCode.Space) && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
        // Run (only when actually moving)
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_animator.SetInteger("AnimState", 2);
        }
        // Combat Idle
        else if (m_combatIdle)
        {
            m_animator.SetInteger("AnimState", 1);
        }
        // Idle
        else
        {
            m_animator.SetInteger("AnimState", 0);
        }
    }

    // --- Public damage API so a Bandit can be killed by hits ---
    public void TakeDamage(int dmg = 1)
    {
        if (m_isDead) return;

        // Optional: play Hurt first
        m_animator.SetTrigger("Hurt");

        if (killOnHit)
        {
            Die();
        }
        // else you could track HP here and only Die() at 0
    }

    private void Die()
    {
        if (m_isDead) return;
        m_isDead = true;
        m_animator.SetTrigger("Death");

        // Optionally stop movement & input effects immediately
        m_body2d.linearVelocity = new Vector2(0f, m_body2d.linearVelocity.y);
    }

    // --- Attack logic (no extra scripts; physics overlap "hitbox") ---
    private void TryAttack()
    {
        if (m_isDead || m_isAttacking) return;
        m_animator.SetTrigger("Attack");
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        m_isAttacking = true;

        // small delay to sync with animation swing frame
        yield return new WaitForSeconds(attackDelay);

        // perform overlap(s) while active
        float t = 0f;
        while (t < attackActiveTime)
        {
            DoAttackHitbox();
            // You can do one check, or multiple checks over frames:
            yield return null;
            t += Time.deltaTime;
        }

        m_isAttacking = false;
    }

    private void DoAttackHitbox()
    {
        if (attackOrigin == null) return;

        // Overlap a box in front of the player at attackOrigin
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackOrigin.position,
            attackSize,
            0f,
            damageLayers
        );

        foreach (var col in hits)
        {
            if (col == null) continue;
            // Avoid hitting yourself
            if (col.attachedRigidbody != null && col.attachedRigidbody == m_body2d) continue;

            // If the target has a Bandit component (enemy), damage it
            Bandit other = col.GetComponentInParent<Bandit>();
            if (other != null && other != this)
            {
                // Don’t kill the player by accident
                if (!other.playerControlled)
                {
                    other.TakeDamage(1);
                }
            }
        }
    }

    // Visualize attack box in editor
    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackOrigin.position, attackSize);
    }
}
