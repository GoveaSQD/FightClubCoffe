using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador1 : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb2D;
    private Vector2 movementInput;
    private Animator animator;
    // public int maxHealth = 100;    
    // private int currentHealth;

    void Start(){
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // currentHealth = maxHealth;
        // UIManager.Instance.UpdateHealth(currentHealth);
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude);
    
        OpenCloseInventory();
        
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = movementInput * speed;
    }

    void OpenCloseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            //Debug.log("precionaste i");
            UIManager.Instance.OpenCloseInventory();
        }
    }
}