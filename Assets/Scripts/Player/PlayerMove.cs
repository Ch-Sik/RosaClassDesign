using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    PlayerMoveState currentState;
    Vector2 inputVector;
    Rigidbody2D rb;

    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        Debug.Log($"Move: {moveInput}");
        inputVector = moveInput;
    }

    public void OnJump()
    {
        InputManager.GetInstance().Test();
    }


    public void Update()    // FixedUpdate여야 했었던가?
    {
        // setVelocity(inputVector.x, rb.velocity.y);
        // rb.velocity = new Vector2(inputVector.x, rb.velocity.y);
    }

    private void setVelocity(float x, float y)
    {
        rb.velocity = new Vector2(x, y);
    }
}