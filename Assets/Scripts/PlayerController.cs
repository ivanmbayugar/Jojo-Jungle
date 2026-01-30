using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerJump jump;
    public PlayerCombat combat;
    public PlayerHealth health;

    void Update()
    {
        if (health.isDead) return;

        movement.CheckGrounded();
        movement.MoveHandler();
        combat.ShootHandler();
        jump.JumpInput();
        health.CheckKnockBack();
    }

    void FixedUpdate()
    {
        if (health.isDead) return;

        movement.Movement();
    }
}
