using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;

    public Animator anim;
    private Jugador1 jugador1;

    // 💥 CORRECCIÓN: Usar el mismo nombre para la declaración y el uso
    private Vector3 originalAttackPointPosition; 

    void Start()
    {
        jugador1 = GetComponent<Jugador1>();
        
        // 🚀 Inicialización: Guarda la posición local original
        if (attackPoint != null)
        {
            originalAttackPointPosition = attackPoint.localPosition;
        }
        else
        {
            Debug.LogError("Player_Combat requiere que la referencia 'attackPoint' esté asignada en el Inspector.");
        }
    }

    // *** CAMBIO CLAVE 4: Recibir la dirección de ataque ***
    public void Attack()
    {
        if (jugador1 == null || attackPoint == null) return;

        // 1. Obtener la dirección actual del sprite del Jugador1
        float direction = jugador1.isFacingRight ? 1f : -1f;

        // 2. Mover el AttackPoint a la izquierda o derecha
        // Esto cambia la posición local X del attackPoint para reflejar la dirección del jugador.
        attackPoint.localPosition = new Vector3(
            originalAttackPointPosition.x * direction, 
            originalAttackPointPosition.y, 
            originalAttackPointPosition.z
        );

        anim.SetBool("isAttacking", true);
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        if(enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>()?.ChangeHealth(-damage);
        }
    }
}