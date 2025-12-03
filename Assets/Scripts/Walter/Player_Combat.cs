using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;

    public Vector2 lastAttackDirection = Vector2.right; 
    public Animator anim;

    public bool isAttacking = false;

    public void Attack()
    {
        isAttacking = true;
        anim.SetBool("isAttacking", true);

        // Obtener la dirección horizontal actual del animator
        float dirX = anim.GetFloat("Horizontal");

        // Si no hay input, usa la última dirección almacenada
        if (dirX == 0)
            dirX = lastAttackDirection.x;

        // Almacenar la dirección final solo en eje X
        lastAttackDirection = new Vector2(dirX, 0);

        // Pasar la dirección al animator
        anim.SetFloat("AttackX", lastAttackDirection.x);
    }

    // Este método lo llamas desde un Animation Event al final del ataque
    public void FinishAttacking()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);

        // Detección de golpe
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            weaponRange,
            enemyLayer
        );

        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>()?.ChangeHealth(-damage);
        }
    }

    // Para visualizar el rango de ataque en el editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
