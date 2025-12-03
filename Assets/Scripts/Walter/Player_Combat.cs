using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;

    public Animator anim;
    public bool isAttacking = false;

    // Propiedad para que Jugador1 pueda leer la dirección del ataque.
    public float LastAttackX => lastAttackX;

    private float lastAttackX = 1f;  // 1 = derecha, -1 = izquierda (por defecto derecha)

    public void Attack()
    {
        // 1. Iniciar Ataque
        isAttacking = true;
        anim.SetBool("isAttacking", true);

        // 2. Determinar la dirección del ataque
        // Leer input del jugador SOLO para decidir izquierda/derecha
        float x = Input.GetAxisRaw("Horizontal"); // <-- Aquí se declara 'x'

        // Si hay movimiento horizontal, actualiza la dirección del ataque.
        if (x != 0)
        {
            lastAttackX = Mathf.Sign(x);
        }

        // 3. Enviar la dirección al Animator
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