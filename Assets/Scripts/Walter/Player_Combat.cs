using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackRate = 2f;
    public int damage = 1;

    private float nextAttackTime = 0f;
    private Jugador1 player;
    private Vector3 originalOffset;

    private void Awake()
    {
        player = GetComponent<Jugador1>();
        anim = GetComponent<Animator>();

        if (attackPoint != null)
            originalOffset = attackPoint.localPosition;
        else
            Debug.LogError("ERROR: 'attackPoint' no está asignado.");
    }

    private void Update()
    {
        // NO atacar si se mueve
        if (player != null && player.isMoving)
            return;

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Attack();
                nextAttackTime = Time.time + (1f / attackRate);
            }
        }
    }

    public void Attack()
    {
        if (attackPoint == null) return;

        anim.SetBool("isAttacking", true);

        // Ajustar hitbox al lado correcto
        float dir = player.isFacingRight ? 1f : -1f;

        attackPoint.localPosition = new Vector3(
            Mathf.Abs(originalOffset.x) * dir,
            originalOffset.y,
            originalOffset.z
        );

        // Detectar enemigos
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hits)
        {
            Enemy_Health eh = enemy.GetComponent<Enemy_Health>();
            if (eh != null)
                eh.ChangeHealth(-damage);
        }
    }

    // 🔥 ESTE ES EL NOMBRE EXACTO QUE DEBE TENER EN TU ANIMACIÓN
    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
