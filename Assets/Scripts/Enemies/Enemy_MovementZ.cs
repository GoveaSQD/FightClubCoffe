using UnityEngine;

public class Enemy_MovementZ : MonoBehaviour
{
    public float speed = 5f;
    private int facingDirection = 1;
    public EnemyStatez enemyState, newState;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyStatez.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyStatez.Chasing)
        {
            if(player.position.x < transform.position.x && facingDirection == 1 ||
            player.position.x > transform.position.x && facingDirection == -1)
            {
                Flip();
            }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        }
            // if (direction.x < 0)
            // {
            //     transform.localScale = new Vector3(-1, 1, 1);
            // }
            // else
            // {
            //     transform.localScale = new Vector3(1, 1, 1);
            // }
        //si el personaje gira a la izquierda, voltear el sprite
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(player == null)
            {
            player = collision.transform;
            }
            ChangeState(EnemyStatez.Chasing);
        }
    }
        
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
        rb.linearVelocity = Vector2.zero;
        ChangeState(EnemyStatez.Idle);
        }
    }

    void ChangeState(EnemyStatez newState)
    {
        //Sale de la animacion del estado anterior
        if(enemyState == EnemyStatez.Idle)
            anim.SetBool("isIdle", false);
        else if(enemyState == EnemyStatez.Chasing)
            anim.SetBool("isChasing", false);

        //Entra en la animacion del nuevo estado
        enemyState = newState;
        
        //activa la animacion del nuevo estado
        if(enemyState == EnemyStatez.Idle)
            anim.SetBool("isIdle", true);
        else if(enemyState == EnemyStatez.Chasing)
            anim.SetBool("isChasing", true);
    }
}


public enum EnemyStatez
{
    Idle,
    Chasing,
}