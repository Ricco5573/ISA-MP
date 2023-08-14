
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementClient : NetworkBehaviour
{
    private int throttle;
    private const int startThrottle = 50;

    private float horizontalVelocity;
    private int lift = 100; //this is to simulate the concept of lift. if you are moving fast enough in the horizontal plane, you will get lift. If not, you will lose lift and start to fall.


    private int health;
    private const int maxHealth = 100;
    [SerializeField]
    private GameObject centerOBJ, bulletPos, bullet;
    [SerializeField]
    private GameObject cameraOBJ;
    private float speedmodifier = 1;
    private float torqueForce = 2;
    private BattleManager bat;

    private Rigidbody rb;



    private void Start()
    {
        if (!IsHost)
        {
            Debug.Log("Client Spawned");
        }
        else
        {
            Debug.LogError("This isnt the Client");
        }
        rb = GetComponent<Rigidbody>();
        throttle = startThrottle;
        health = maxHealth;
        bat = GameObject.FindObjectOfType<BattleManager>();
    }
    private void Update()
    {

        if (!IsHost)
        {
            Movement();

        }

    }

    private void Movement()
    {

    
        if (!IsHost)
        {

            torqueForce = 100 / (throttle + 6) + 1; //the faster you move, the slower you turn. the plus 6 and plus 1 respectively are to bind it between the numbers of 15 and 1. So that you cant not turn, and cant turn too fast
            float accel = Input.GetAxis("Throttle");
            if (accel != 0)
            {
                if (accel > 0 && throttle < 100)
                {
                    throttle++;
                }
                else if (accel < 0 && throttle > 1)
                {
                    throttle--;
                }
            }

            float banking = Input.GetAxis("Bank");

            float tilt = Input.GetAxis("Tilt");
            SendMoveRequestServerRpc(banking, tilt);
            cameraOBJ.transform.RotateAround(centerOBJ.transform.position,
                                    cameraOBJ.transform.up,
                                    -Input.GetAxis("Mouse X") * 5);

            cameraOBJ.transform.RotateAround(centerOBJ.transform.position,
                                            cameraOBJ.transform.right,
                                            Input.GetAxis("Mouse Y") * 5);
            //not my code, grabbed off of stackoverflow: https://stackoverflow.com/questions/54852001/rotate-camera-around-a-gameobject-on-mouse-drag-in-unity
            if (Input.GetAxis("Fire1") >= 0.1f)
            {
                ShootServerRpc();

            }
        }
        
    }
    [ServerRpc (RequireOwnership = false)]
    private void SendMoveRequestServerRpc(float bank, float tilt)
    {
        if (bank != 0)
        {
            rb.AddTorque(centerOBJ.transform.up * torqueForce * bank);
        }
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
        rb.AddForce(transform.forward * throttle * 19000 * speedmodifier, ForceMode.Force);


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
    }
    [ServerRpc (RequireOwnership = false)]
    private void ShootServerRpc()
    {
        GameObject b = Instantiate(bullet, bulletPos.transform.position, transform.rotation);
        b.GetComponent<NetworkObject>().Spawn();
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            health -= 3;
            if (health <= 0)
            {
                bat.HostWinServerRpc();
            }
        }
        else if (collision.gameObject.tag == "Terrain")
        {
            bat.HostWinServerRpc();
        }
    }
}


