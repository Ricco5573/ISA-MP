using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    public int damage;

    private void Start()
    {
        rb= GetComponent<Rigidbody>();
        Debug.Log(rb.velocity);
    }


    private void Update()
    {
        rb.AddForce(this.transform.forward * 9000, ForceMode.Force);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Terrain")
        {
            Destroy(gameObject);
        }
        else if (collision.collider.tag == "Player")
        {
            //trigger damage.
            Destroy(gameObject);
        }
    }
}
