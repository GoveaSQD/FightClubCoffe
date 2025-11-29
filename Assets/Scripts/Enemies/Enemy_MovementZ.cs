using UnityEngine;

public class Enemy_MovementZ : MonoBehaviour
{
    public float speed = 5f;
    private int facingDirection = 1;
    public EnemyStatez enemyState, newState;

    public float attackRange = 2;
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
        if (enemyState == EnemyStatez.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyStatez.Attacking)
        {
            // Atacar
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Chase()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            ChangeState(EnemyStatez.Attacking);
            return; // Importante salir aquí
        }
        else if (player.position.x < transform.position.x && facingDirection == 1 ||
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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player == null)
            {
                player = collision.transform;
            }
            ChangeState(EnemyStatez.Chasing);
        }
    }
        
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyStatez.Idle);
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