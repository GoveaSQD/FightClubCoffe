using UnityEngine;

public class Enemy_MovementZ : MonoBehaviour
{
    public float speed = 5f;
    public float attackRange = 2;
    public float attackCooldown = 2;
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer;
    private int facingDirection = 1;
    public EnemyStatez enemyState, newState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyStatez.Idle);
    }

    void Update()
    {
        CheckForPlayer();
        if(attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        if (enemyState == EnemyStatez.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyStatez.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Chase()
    {
        if (player.position.x < transform.position.x && facingDirection == 1 ||
                player.position.x > transform.position.x && facingDirection == -1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }   
    
    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;

            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldownTimer <= 0)
        {
            attackCooldownTimer = attackCooldown;
            ChangeState(EnemyStatez.Attacking);
        }
        else if (Vector2.Distance(transform.position, player.position) > attackRange && enemyState != EnemyStatez.Attacking)
        {
            ChangeState(EnemyStatez.Chasing);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyStatez.Idle);
        }
        }
    }

    void ChangeState(EnemyStatez newState)
    {
        // Sale de la animación del estado anterior
        if (enemyState == EnemyStatez.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyStatez.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyStatez.Attacking)
            anim.SetBool("isAttacking", false);

        // Entra en la animación del nuevo estado
        enemyState = newState;
        
        // Activa la animación del nuevo estado
        if (enemyState == EnemyStatez.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyStatez.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyStatez.Attacking)
        {
            anim.SetBool("isAttacking", true);
            // VOLTEAR HACIA EL JUGADOR AL INICIAR EL ATAQUE
            FacePlayerForAttack();
        }
    }

    private void OnDrawGizmosSlected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }
    // NUEVO MÉTODO: Voltear hacia el jugador al comenzar el ataque
    void FacePlayerForAttack()
    {
        if (player == null) return;

        // Verificar si necesita voltearse para quedar cara a cara
        if ((player.position.x < transform.position.x && facingDirection == -1) ||
            (player.position.x > transform.position.x && facingDirection == 1))
        {
            Flip();
        }
    }
}

public enum EnemyStatez
{
    Idle,
    Chasing,
    Attacking
}