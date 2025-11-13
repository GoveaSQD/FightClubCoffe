using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador1 : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb2D;
    private float horizontal;
    private float vertical;
    Vector2 movementInput;
    private Animator animator;

    void Start(){
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", Mathf.Abs(movementInput.x));
        animator.SetFloat("Vertical", Mathf.Abs(movementInput.y));

        rb2D.linearVelocity = new Vector2(horizontal, vertical) * speed;

        CheckFlip();
    }

        private void CheckFlip()
    {
        if (movementInput.x > 0 && transform.localScale.x < 0 || movementInput.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        // if (movementInput.x > 0 && transform.localScale.x < 0 || movementInput.x < 0 && transform.localScale.x > 0)
        // {
        //  transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.Y);
        // }
    }
    void FixedUpdate()
    {
        rb2D.linearVelocity = movementInput * speed;
    }
}