using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;


public class PlayerMovement : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }



    public void Movement(Vector2 direction)
    {
       
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
