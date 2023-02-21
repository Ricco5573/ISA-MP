using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;


public class PlayerMovement : NetworkBehaviour
{

    private int maxSpeed;
    private int speed = 50;
    private Rigidbody rb;
    private int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && speed < 100)
        {
            speed++;
        }
        else if(Input.GetKey(KeyCode.LeftControl) && speed > 0)
        {
            speed--;
        }

        rb.velocity.Set(speed, 0, 0);
        Debug.Log(rb.velocity);
        Debug.Log(speed);
    }

    [ClientRpc]
    void MoveClientRpc(int value)
    {

    }

    [ServerRpc]
    void MoveServerRpc(int value)
    {

    }
}
