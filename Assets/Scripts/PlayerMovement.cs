using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;


public class PlayerMovement : NetworkBehaviour
{
    public float rollSpeed = 50f;         // Speed of roll rotation
    public float bankSpeed = 20f;          // Speed of bank rotation
    public float turnSpeed = 75f;         // Speed of turning
    public float acceleration = 2000f;        // Acceleration rate
    public float deceleration = 30f;        // Deceleration rate
    public float maxSpeed = 2000f;           // Maximum speed
    public float minSpeed = 5f;            // Minimum speed
    public float altitudeLossRate = 0.5f;  // Rate of altitude loss
    private Rigidbody rb;
    private float currentSpeed = 0f;       // Current speed of the player
    private float currentAltitude = 0f;    // Current altitude of the player



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        acceleration = 2000f;
        maxSpeed = 2000f;
        // Get input for rolling and banking
        float rollInput = Input.GetAxis("Roll");
        float bankInput = Input.GetAxis("Bank");

        // Calculate roll and bank rotation
        float roll = rollInput * rollSpeed * Time.deltaTime;
        float bank = bankInput * bankSpeed * Time.deltaTime;

        // Apply roll and bank rotations
        transform.Rotate(Vector3.forward, -roll, Space.Self);
        transform.Rotate(Vector3.right, bank, Space.Self);

        // Get input for turning    
        float turnInput = Input.GetAxis("Vertical");

        // Calculate turn rotation
        float turn = turnInput * turnSpeed * Time.deltaTime;

        // Apply turn rotation
        transform.Rotate(Vector3.up, turn, Space.Self);

        // Get input for accelerating and decelerating
        float throttleInput = Input.GetAxis("Throttle");


        Debug.Log(maxSpeed);      // Calculate speed change
        float speedChange = throttleInput * acceleration;
    
        // Apply speed change
        currentSpeed = Mathf.Clamp(currentSpeed + speedChange, minSpeed, maxSpeed);

        // Move the player forward based on the current speed
        rb.AddForceAtPosition( this.gameObject.transform.forward * currentSpeed, gameObject.transform.position);

        // Check if the player is not moving fast enough
         if (currentSpeed < minSpeed)
         {
             // Calculate altitude loss
             float altitudeLoss = altitudeLossRate * Time.deltaTime;

             // Decrease the altitude
             currentAltitude -= altitudeLoss;

             // Move the player downwards
             transform.Translate(Vector3.down * altitudeLoss);
         }
    }
}

