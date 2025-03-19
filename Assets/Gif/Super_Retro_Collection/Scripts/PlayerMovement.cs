// -----------------------------------------------------------------------------------------
// using classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// -----------------------------------------------------------------------------------------
// player movement class
public class PlayerMovement : MonoBehaviour
{
    // static public members
    public static PlayerMovement instance;
    public Joystick joystick;

    // -----------------------------------------------------------------------------------------
    // public members
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    // -----------------------------------------------------------------------------------------
    // private members
    private Vector2 movement;

    // -----------------------------------------------------------------------------------------
    // awake method to initialisation
    void Awake()
    {
        instance = this;
    }

    // -----------------------------------------------------------------------------------------
    // Update is called once per frame
    // This method is responsible for updating the movement vector based on the joystick input.
    // The joystick input is obtained from the joystick component attached to the player game object.
    // The movement vector is then used to update the player's position in the FixedUpdate method.
    void Update()
    {
        // Update the x component of the movement vector with the horizontal input from the joystick.
        // The joystick's Horizontal method returns a value between -1 and 1, where -1 is left, 0 is center,
        // and 1 is right. The movement vector is scaled by the joystick input to ensure the player moves
        // at the same speed regardless of the joystick's sensitivity setting.
        if (joystick.Direction != Vector2.zero)
        {
            movement.x = joystick.Horizontal;

            // Update the y component of the movement vector with the vertical input from the joystick.
            // The joystick's Vertical method returns a value between -1 and 1, where -1 is up, 0 is center,
            // and 1 is down. The movement vector is scaled by the joystick input to ensure the player moves
            // at the same speed regardless of the joystick's sensitivity setting.
            movement.y = joystick.Vertical;
        }
        else
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
        }

    }
    // -----------------------------------------------------------------------------------------
    /// <summary>
    /// This method is called every fixed frame-rate frame, if the MonoBehaviour is enabled.
    /// It is used for physics-related calculations.
    /// </summary>
    /// <remarks>
    /// This method updates the position of the rigidbody by moving it based on the current movement vector
    /// multiplied by the move speed and the time elapsed between frames (time factor). The time factor is calculated
    /// by multiplying the time between frames with the fixed time step. The movement vector is scaled by the move speed
    /// to ensure consistent player movement speed regardless of the joystick's sensitivity setting.
    /// </remarks>
    void FixedUpdate()
    {
        // Move the rigidbody's position based on the current movement vector multiplied by the move speed and the time factor.
        // The new position is calculated by adding the change in position (the product of the movement vector, move speed, and time factor) to the current position of the rigidbody.
        // The change in position is calculated by multiplying the movement vector with the move speed and the time factor.
        // The time factor is calculated by multiplying the time between frames with the fixed time step.
        // The movement vector is scaled by the move speed to ensure consistent player movement speed regardless of the joystick's sensitivity setting.
        rb.MovePosition(rb.position + (movement * moveSpeed * Time.fixedDeltaTime));
    }

}
