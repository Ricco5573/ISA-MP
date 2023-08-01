using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;


public class PlayerMovement : NetworkBehaviour
{
    private NetworkVariable<int> throttle = new NetworkVariable<int>();
    private const int startThrottle = 50;

    private float horizontalVelocity;
    private int lift = 100; //this is to simulate the concept of lift. if you are moving fast enough in the horizontal plane, you will get lift. If not, you will lose lift and start to fall.


    private NetworkVariable<int> health = new NetworkVariable<int>();
    private const int maxHealth = 100;
    [SerializeField]
    private GameObject centerOBJ, bulletPos, bullet;
    [SerializeField]
    private GameObject cameraOBJ;
    private float speedmodifier = 1;
    private float torqueForce = 2;


    private Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        throttle.Value = startThrottle;
        health.Value = maxHealth;
    }
    private void Update()
    {
        Movement();
        if (Input.GetAxis("Fire1") >= 0.1f)
        {
           Instantiate(bullet, bulletPos.transform.position, transform.rotation);

        }
    }

    private void Movement()
    {

        torqueForce = 100 / (throttle.Value + 6) + 1; //the faster you move, the slower you turn. the plus 6 and plus 1 respectively are to bind it between the numbers of 15 and 1. So that you cant not turn, and cant turn too fast
        float accel = Input.GetAxis("Throttle");
        if (accel != 0)
        {
            if (accel > 0 && throttle.Value < 100)
            {
                throttle.Value++;
            }
            else if (accel < 0 && throttle.Value > 1)
            {
                throttle.Value--;
            }
        }

        float banking = Input.GetAxis("Bank");
        if (banking != 0)
        {
            rb.AddTorque(centerOBJ.transform.up * torqueForce * banking);
        }
        float tilt = Input.GetAxis("Tilt");
        if (tilt != 0)
        {
            rb.AddTorque(centerOBJ.transform.right * torqueForce * tilt);
        }

        horizontalVelocity = rb.velocity.x + rb.velocity.z;
        if (horizontalVelocity > 1 && lift < 100 && rb.velocity.y < 5)
        {
            lift++;
        }
        else if (horizontalVelocity < 1 && lift > 0)
        {
            lift--;

        }
        else if (rb.velocity.y > 10 && lift > 0)
        {
            lift--;
        }
        rb.AddForce(transform.forward * throttle.Value * 19000 * speedmodifier, ForceMode.Force);


        if (lift < 20)
        {
            rb.useGravity = true;
            speedmodifier = 0.1f;

        }
        else if (lift > 50)
        {
            rb.useGravity = false;
            speedmodifier = 1;

        }
        cameraOBJ.transform.RotateAround(centerOBJ.transform.position,
                                cameraOBJ.transform.up,
                                -Input.GetAxis("Mouse X") * 5);

        cameraOBJ.transform.RotateAround(centerOBJ.transform.position,
                                        cameraOBJ.transform.right,
                                        Input.GetAxis("Mouse Y") * 5);
        //not my code, grabbed off of stackoverflow: https://stackoverflow.com/questions/54852001/rotate-camera-around-a-gameobject-on-mouse-drag-in-unity

    }
}

