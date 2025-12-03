using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;

    public Animator anim;
    public bool isAttacking = false;

    private float lastAttackX = 1f;  // 1 = derecha, -1 = izquierda (por defecto derecha)

    public void Attack()
    {
        isAttacking = true;
        anim.SetBool("isAttacking", true);

        // Leer input del jugador SOLO para decidir izquierda/derecha
        float x = Input.GetAxisRaw("Horizontal");

        // Si no se está moviendo, usamos la última dirección usada para atacar
        if (x != 0)
            lastAttackX = Mathf.Sign(x);

        // Enviamos solo esto al animator, nada más
        anim.SetFloat("AttackX", lastAttackX);
    }

    // Evento de animación
    public void FinishAttacking()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);

        // Detectar enemigos
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>()?.ChangeHealth(-damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
