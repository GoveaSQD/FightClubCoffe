using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float speed = 5f;
    private int facingDirection = 1;
    public EnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (enemyState == EnemyState.Chasing && player != null)
        {
            // Volteo
            if (player.position.x < transform.position.x && facingDirection == 1 ||
                player.position.x > transform.position.x && facingDirection == -1)
            {
                Flip();
            }

            // Movimiento
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        else if (enemyState == EnemyState.Idle)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * facingDirection,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player == null)
                player = collision.transform;

            ChangeState(EnemyState.Chasing);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero;
            player = null;
            ChangeState(EnemyState.Idle);
        }
    }

    void ChangeState(EnemyState newState)
    {
        // Apagar estado anterior
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);

        // Cambiar estado
        enemyState = newState;

        // Encender nuevo estado
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
    }
}

public enum EnemyState
{
    Idle,
    Chasing
}